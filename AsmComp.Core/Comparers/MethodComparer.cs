using AsmComp.Core.Hierarchy;
using AsmComp.Core.MEF;
using AsmComp.Core.Utilities;
using JetBrains.Annotations;
using Microsoft;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

namespace AsmComp.Core.Comparers;

[Export(typeof(IDotNetMetadataComparer))]
internal class MethodComparer : IDotNetMetadataComparer {
    [CanBeNull]
    public HierarchicalDirectory? Compare(
        [MaybeNull][CanBeNull][AllowNull] object x,
        [MaybeNull][CanBeNull][AllowNull] object y
    ) {
        Assumes.NotNull(x);
        Assumes.NotNull(y);

        if (x is not MethodDefinition && y is not MethodDefinition) {
            return null;
        }

        MethodDefinition left = (MethodDefinition)x;
        MethodDefinition right = (MethodDefinition)y;
        var hierarchicalDirectory = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Method");

        CompareBody(left.Body, right.Body);
        hierarchicalDirectory._hierarchicalDirectories.Add(DispenseMethodFlags(left, right));

        if (left.Name != right.Name) {
            ReportChange("Name", left.Name, right.Name);
        }
        else {
            ReportExact("Name", left.Name, right.Name);
        }

        if (left.ReturnType.GetTypeNameLikeIL() != right.ReturnType.GetTypeNameLikeIL()) {
            ReportChange("ReturnType", left.ReturnType.GetTypeNameLikeIL(), right.ReturnType.GetTypeNameLikeIL());
        }
        else {
            ReportExact("ReturnType", left.ReturnType.GetTypeNameLikeIL(), right.ReturnType.GetTypeNameLikeIL());
        }

        if (left.AggressiveInlining != right.AggressiveInlining) {
            ReportChange("AggressiveInlining", left.AggressiveInlining.ToString(), right.AggressiveInlining.ToString());
        }
        else {
            ReportExact("AggressiveInlining", left.AggressiveInlining.ToString(), right.AggressiveInlining.ToString());
        }

        if (left.AggressiveOptimization != right.AggressiveOptimization) {
            ReportChange("AggressiveOptimization", left.AggressiveOptimization.ToString(), right.AggressiveOptimization.ToString());
        }
        else {
            ReportExact("AggressiveOptimization", left.AggressiveOptimization.ToString(), right.AggressiveOptimization.ToString());
        }

        if (left.ExplicitThis != right.ExplicitThis) {
            ReportChange("ExplicitThis", left.ExplicitThis.ToString(), right.ExplicitThis.ToString());
        }
        else {
            ReportExact("ExplicitThis", left.ExplicitThis.ToString(), right.ExplicitThis.ToString());
        }

        if (left.HasThis != right.HasThis) {
            ReportChange("HasThis", left.HasThis.ToString(), right.HasThis.ToString());
        }
        else {
            ReportExact("HasThis", left.HasThis.ToString(), right.HasThis.ToString());
        }

        if (left.MetadataToken != right.MetadataToken) {
            ReportChange("MDToken", left.MetadataToken.ToUInt32().ToString(), right.MetadataToken.ToUInt32().ToString());
        }
        else {
            ReportExact("MDToken", left.MetadataToken.ToUInt32().ToString(), right.MetadataToken.ToUInt32().ToString());
        }

        var attributeData = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "CustomAttributes");
        int attribCountX = left.CustomAttributes.Count;
        int attribCountY = right.CustomAttributes.Count;
        if (attribCountX > attribCountY) {
            for (int i = 0; i < attribCountY; i++) {
                attributeData._hierarchicalDirectories.Add(CustomAttributeComparer.Compare(left.CustomAttributes[i], right.CustomAttributes[i]));
            }
            for (int i = attribCountY; i < attribCountX; i++) {
                attributeData._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.CustomAttribute, left: "...", right: "...", reason: "CustomAttribute"));
            }
        }
        else if (attribCountY > attribCountX) {
            for (int i = 0; i < attribCountX; i++) {
                attributeData._hierarchicalDirectories.Add(CustomAttributeComparer.Compare(left.CustomAttributes[i], right.CustomAttributes[i]));
            }
            for (int i = attribCountX; i < attribCountY; i++) {
                attributeData._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.CustomAttribute, left: "...", right: "...", reason: "CustomAttribute"));
            }
        }
        else {
            for (int i = 0; i < attribCountX; i++) {
                attributeData._hierarchicalDirectories.Add(CustomAttributeComparer.Compare(left.CustomAttributes[i], right.CustomAttributes[i]));
            }
        }
        hierarchicalDirectory._hierarchicalDirectories.Add(attributeData);

        return hierarchicalDirectory;

        void ReportChange(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.Method, left: x, right: y, reason: reason));
        }

        void ReportExact(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Exact, HierarchicalObjectValueKind.Method, left: x, right: y, reason: reason));
        }

        void CompareBody(MethodBody leftBody, MethodBody rightBody) {
            int leftCount = leftBody.Instructions.Count;
            int rightCount = rightBody.Instructions.Count;

            var body = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "MethodBody");
            var bodyData = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Instructions");

            if (leftCount > rightCount) {
                for (int i = 0; i < rightCount; i++) {
                    bodyData._hierarchicalDirectories.Add(DispenseInstructionDifferences(leftBody.Instructions[i], rightBody.Instructions[i]));
                }
                for (int i = rightCount - 1; i < leftCount; i++) {
                    bodyData._hierarchicalObjects.Add(
                        new HierarchicalObject(HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.Instruction, left: $"{leftBody.Instructions[i].OpCode.Code}", right: "[none]", reason: "Instruction"));
                }
            }
            else if (rightCount > leftCount) {
                for (int i = 0; i < leftCount; i++) {
                    bodyData._hierarchicalDirectories.Add(DispenseInstructionDifferences(leftBody.Instructions[i], rightBody.Instructions[i]));
                }
                for (int i = leftCount - 1; i < rightCount; i++) {
                    bodyData._hierarchicalObjects.Add(
                        new HierarchicalObject(HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.Instruction, left: "[none]", right: $"{rightBody.Instructions[i].OpCode.Code}", reason: "Instruction"));
                }
            }
            else {
                for (int i = 0; i < leftCount; i++) {
                    bodyData._hierarchicalDirectories.Add(DispenseInstructionDifferences(leftBody.Instructions[i], rightBody.Instructions[i]));
                }
            }

            bodyData._hierarchicalDirectories.Add(body);

            if (leftBody.InitLocals != rightBody.InitLocals) {
                ReportChange("InitLocals", leftBody.InitLocals.ToString(), rightBody.InitLocals.ToString());
            }
            else {
                ReportExact("InitLocals", leftBody.InitLocals.ToString(), rightBody.InitLocals.ToString());
            }

            if (leftBody.LocalVarToken.ToUInt32() != rightBody.LocalVarToken.ToUInt32()) {
                ReportChange("LVTMetadataToken", leftBody.LocalVarToken.ToUInt32().ToString(), rightBody.LocalVarToken.ToUInt32().ToString());
            }
            else {
                ReportExact("LVTMetadataToken", leftBody.LocalVarToken.ToUInt32().ToString(), rightBody.LocalVarToken.ToUInt32().ToString());
            }

            if (leftBody.MaxStackSize != rightBody.MaxStackSize) {
                ReportChange("MaxStack", leftBody.MaxStackSize.ToString(), rightBody.MaxStackSize.ToString());
            }
            else {
                ReportExact("MaxStack", leftBody.MaxStackSize.ToString(), rightBody.MaxStackSize.ToString());
            }

            int leftVariableCount = leftBody.Variables.Count;
            int rightVariableCount = rightBody.Variables.Count;

            var variableData = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Variables");

            if (leftVariableCount > rightVariableCount) {
                for (int i = 0; i < rightVariableCount; i++) {
                    variableData._hierarchicalDirectories.Add(DispenseVariableDifferences(leftBody.Variables[i], rightBody.Variables[i]));
                }
                for (int i = rightVariableCount - 1; i < leftVariableCount; i++) {
                    variableData._hierarchicalObjects.Add(
                        new HierarchicalObject(HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.Variable, left: $"{leftBody.Variables[i].Index}", right: "[none]", reason: "Instruction"));
                }
            }
            else if (rightVariableCount > leftVariableCount) {
                for (int i = 0; i < leftVariableCount; i++) {
                    variableData._hierarchicalDirectories.Add(DispenseVariableDifferences(leftBody.Variables[i], rightBody.Variables[i]));
                }
                for (int i = leftVariableCount - 1; i < rightVariableCount; i++) {
                    variableData._hierarchicalObjects.Add(
                        new HierarchicalObject(HierarchicalObjectKind.Substitute, HierarchicalObjectValueKind.Variable, left: "[none]", right: $"{rightBody.Variables[i].Index}", reason: "Instruction"));
                }
            }
            else {
                for (int i = 0; i < leftVariableCount; i++) {
                    variableData._hierarchicalDirectories.Add(DispenseVariableDifferences(leftBody.Variables[i], rightBody.Variables[i]));
                }
            }
            body._hierarchicalDirectories.Add(variableData);
            hierarchicalDirectory._hierarchicalDirectories.Add(body);

            void ReportChange(string reason, string x, string y) {
                body._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Change, HierarchicalObjectValueKind.Method, left: x, right: y, reason: reason));
            }

            void ReportExact(string reason, string x, string y) {
                body._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Exact, HierarchicalObjectValueKind.Method, left: x, right: y, reason: reason));
            }
        }
    }

    private static HierarchicalDirectory DispenseInstructionDifferences(Instruction instructionLeft, Instruction instructionRight) {
        var dir = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: $"Instruction;{instructionLeft.OpCode.Code},{instructionRight.OpCode.Code}");

        if (instructionLeft.OpCode.Code != instructionRight.OpCode.Code) {
            ReportChangeSealed("Code", instructionLeft.OpCode.Code.ToString(), instructionRight.OpCode.Code.ToString());
        }
        else {
            ReportExactSealed("Code", instructionLeft.OpCode.Code.ToString(), instructionRight.OpCode.Code.ToString());
        }

        if (instructionLeft.Offset != instructionRight.Offset) {
            ReportChangeSealed("Offset", instructionLeft.Offset.ToString(), instructionRight.Offset.ToString());
        }
        else {
            ReportExactSealed("Offset", instructionLeft.Offset.ToString(), instructionRight.Offset.ToString());
        }

        if (instructionLeft.Operand != instructionRight.Operand) {
            ReportChangeSealed("Operand", "...", "...");
        }
        else {
            ReportExactSealed("Operand", "...", "...");
        }

        return dir;

        void ReportChangeSealed(string reason, string x, string y) {
            dir._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.Instruction, left: x, right: y, reason: reason));
        }

        void ReportExactSealed(string reason, string x, string y) {
            dir._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Exact, HierarchicalObjectValueKind.Instruction, left: x, right: y, reason: reason));
        }
    }

    private static HierarchicalDirectory DispenseVariableDifferences(VariableDefinition x, VariableDefinition y) {
        var dir = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Variable");

        if (x.IsPinned != y.IsPinned) {
            ReportChange("IsPinned", x.IsPinned.ToString(), y.IsPinned.ToString());
        }
        else {
            ReportExact("IsPinned", x.IsPinned.ToString(), y.IsPinned.ToString());
        }

        if (x.VariableType.GetTypeNameLikeIL() != y.VariableType.GetTypeNameLikeIL()) {
            ReportChange("VariableType", x.VariableType.GetTypeNameLikeIL(), y.VariableType.GetTypeNameLikeIL());
        }
        else {
            ReportExact("VariableType", x.VariableType.GetTypeNameLikeIL(), y.VariableType.GetTypeNameLikeIL());
        }

        if (x.Index != y.Index) {
            ReportChange("Index", x.Index.ToString(), y.Index.ToString());
        }
        else {
            ReportExact("Index", x.Index.ToString(), y.Index.ToString());
        }

        return dir;

        void ReportChange(string reason, string x, string y) {
            dir._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.Variable, x, y, reason));
        }

        void ReportExact(string reason, string x, string y) {
            dir._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Exact, HierarchicalObjectValueKind.Variable, x, y, reason));
        }
    }

    private static HierarchicalDirectory DispenseMethodFlags(MethodDefinition left, MethodDefinition right) {
        var dir = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "MethodFlags");

        if (left.IsAbstract != right.IsAbstract) {
            ReportChange("Abstract", left.IsAbstract.ToString(), right.IsAbstract.ToString());
        }
        else {
            ReportExact("Abstract", left.IsAbstract.ToString(), right.IsAbstract.ToString());
        }

        if (left.IsAddOn != right.IsAddOn) {
            ReportChange("AddOn", left.IsAddOn.ToString(), right.IsAddOn.ToString());
        }
        else {
            ReportExact("AddOn", left.IsAddOn.ToString(), right.IsAddOn.ToString());
        }

        if (left.IsAssembly != right.IsAssembly) {
            ReportChange("Assembly", left.IsAssembly.ToString(), right.IsAssembly.ToString());
        }
        else {
            ReportExact("Assembly", left.IsAssembly.ToString(), right.IsAssembly.ToString());
        }

        if (left.IsCheckAccessOnOverride != right.IsCheckAccessOnOverride) {
            ReportChange("CheckAccessOnOverride", left.IsCheckAccessOnOverride.ToString(), right.IsCheckAccessOnOverride.ToString());
        }
        else {
            ReportExact("CheckAccessOnOverride", left.IsCheckAccessOnOverride.ToString(), right.IsCheckAccessOnOverride.ToString());
        }

        if (left.IsCompilerControlled != right.IsCompilerControlled) {
            ReportChange("CompilerControlled", left.IsCompilerControlled.ToString(), right.IsCompilerControlled.ToString());
        }
        else {
            ReportExact("CompilerControlled", left.IsCompilerControlled.ToString(), right.IsCompilerControlled.ToString());
        }

        if (left.IsConstructor != right.IsConstructor) {
            ReportChange("Constructor", left.IsConstructor.ToString(), right.IsConstructor.ToString());
        }
        else {
            ReportExact("Constructor", left.IsConstructor.ToString(), right.IsConstructor.ToString());
        }

        if (left.IsDefinition != right.IsDefinition) {
            ReportChange("Definition", left.IsDefinition.ToString(), right.IsDefinition.ToString());
        }
        else {
            ReportExact("Definition", left.IsDefinition.ToString(), right.IsDefinition.ToString());
        }

        if (left.IsFamily != right.IsFamily) {
            ReportChange("Family", left.IsFamily.ToString(), right.IsFamily.ToString());
        }
        else {
            ReportExact("Family", left.IsFamily.ToString(), right.IsFamily.ToString());
        }

        if (left.IsFamilyAndAssembly != right.IsFamilyAndAssembly) {
            ReportChange("FamilyAndAssembly", left.IsFamilyAndAssembly.ToString(), right.IsFamilyAndAssembly.ToString());
        }
        else {
            ReportExact("FamilyAndAssembly", left.IsFamilyAndAssembly.ToString(), right.IsFamilyAndAssembly.ToString());
        }

        if (left.IsFamilyOrAssembly != right.IsFamilyOrAssembly) {
            ReportChange("FamilyOrAssembly", left.IsFamilyOrAssembly.ToString(), right.IsFamilyOrAssembly.ToString());
        }
        else {
            ReportExact("FamilyOrAssembly", left.IsFamilyOrAssembly.ToString(), right.IsFamilyOrAssembly.ToString());
        }

        if (left.IsFinal != right.IsFinal) {
            ReportChange("Final", left.IsFinal.ToString(), right.IsFinal.ToString());
        }
        else {
            ReportExact("Final", left.IsFinal.ToString(), right.IsFinal.ToString());
        }

        if (left.IsFire != right.IsFire) {
            ReportChange("Fire", left.IsFire.ToString(), right.IsFire.ToString());
        }
        else {
            ReportExact("Fire", left.IsFire.ToString(), right.IsFire.ToString());
        }

        if (left.IsForwardRef != right.IsForwardRef) {
            ReportChange("ForwardRef", left.IsForwardRef.ToString(), right.IsForwardRef.ToString());
        }
        else {
            ReportExact("ForwardRef", left.IsForwardRef.ToString(), right.IsForwardRef.ToString());
        }

        if (left.IsGenericInstance != right.IsGenericInstance) {
            ReportChange("GenericInstance", left.IsGenericInstance.ToString(), right.IsGenericInstance.ToString());
        }
        else {
            ReportExact("GenericInstance", left.IsGenericInstance.ToString(), right.IsGenericInstance.ToString());
        }

        if (left.IsGetter != right.IsGetter) {
            ReportChange("Getter", left.IsGetter.ToString(), right.IsGetter.ToString());
        }
        else {
            ReportExact("Getter", left.IsGetter.ToString(), right.IsGetter.ToString());
        }

        if (left.IsHideBySig != right.IsHideBySig) {
            ReportChange("HideBySig", left.IsHideBySig.ToString(), right.IsHideBySig.ToString());
        }
        else {
            ReportExact("HideBySig", left.IsHideBySig.ToString(), right.IsHideBySig.ToString());
        }

        if (left.IsIL != right.IsIL) {
            ReportChange("IL", left.IsIL.ToString(), right.IsIL.ToString());
        }
        else {
            ReportExact("IL", left.IsIL.ToString(), right.IsIL.ToString());
        }

        if (left.IsInternalCall != right.IsInternalCall) {
            ReportChange("InternalCall", left.IsInternalCall.ToString(), right.IsInternalCall.ToString());
        }
        else {
            ReportExact("InternalCall", left.IsInternalCall.ToString(), right.IsInternalCall.ToString());
        }

        if (left.IsManaged != right.IsManaged) {
            ReportChange("Managed", left.IsManaged.ToString(), right.IsManaged.ToString());
        }
        else {
            ReportExact("Managed", left.IsManaged.ToString(), right.IsManaged.ToString());
        }

        if (left.IsNative != right.IsNative) {
            ReportChange("Native", left.IsNative.ToString(), right.IsNative.ToString());
        }
        else {
            ReportExact("Native", left.IsNative.ToString(), right.IsNative.ToString());
        }

        if (left.IsNewSlot != right.IsNewSlot) {
            ReportChange("NewSlot", left.IsNewSlot.ToString(), right.IsNewSlot.ToString());
        }
        else {
            ReportExact("NewSlot", left.IsNewSlot.ToString(), right.IsNewSlot.ToString());
        }

        if (left.IsOther != right.IsOther) {
            ReportChange("Other", left.IsOther.ToString(), right.IsOther.ToString());
        }
        else {
            ReportExact("Other", left.IsOther.ToString(), right.IsOther.ToString());
        }

        if (left.IsPInvokeImpl != right.IsPInvokeImpl) {
            ReportChange("PInvokeImpl", left.IsPInvokeImpl.ToString(), right.IsPInvokeImpl.ToString());
        }
        else {
            ReportExact("PInvokeImpl", left.IsPInvokeImpl.ToString(), right.IsPInvokeImpl.ToString());
        }

        if (left.IsPreserveSig != right.IsPreserveSig) {
            ReportChange("PreserveSig", left.IsPreserveSig.ToString(), right.IsPreserveSig.ToString());
        }
        else {
            ReportExact("PreserveSig", left.IsPreserveSig.ToString(), right.IsPreserveSig.ToString());
        }

        if (left.IsPrivate != right.IsPrivate) {
            ReportChange("Private", left.IsPrivate.ToString(), right.IsPrivate.ToString());
        }
        else {
            ReportExact("Private", left.IsPrivate.ToString(), right.IsPrivate.ToString());
        }

        if (left.IsPublic != right.IsPublic) {
            ReportChange("Public", left.IsPublic.ToString(), right.IsPublic.ToString());
        }
        else {
            ReportExact("Public", left.IsPublic.ToString(), right.IsPublic.ToString());
        }

        if (left.IsRemoveOn != right.IsRemoveOn) {
            ReportChange("RemoveOn", left.IsRemoveOn.ToString(), right.IsRemoveOn.ToString());
        }
        else {
            ReportExact("RemoveOn", left.IsRemoveOn.ToString(), right.IsRemoveOn.ToString());
        }

        if (left.IsReuseSlot != right.IsReuseSlot) {
            ReportChange("ReuseSlot", left.IsReuseSlot.ToString(), right.IsReuseSlot.ToString());
        }
        else {
            ReportExact("ReuseSlot", left.IsReuseSlot.ToString(), right.IsReuseSlot.ToString());
        }

        if (left.IsRuntime != right.IsRuntime) {
            ReportChange("Runtime", left.IsRuntime.ToString(), right.IsRuntime.ToString());
        }
        else {
            ReportExact("Runtime", left.IsRuntime.ToString(), right.IsRuntime.ToString());
        }

        if (left.IsRuntimeSpecialName != right.IsRuntimeSpecialName) {
            ReportChange("RuntimeSpecialName", left.IsRuntimeSpecialName.ToString(), right.IsRuntimeSpecialName.ToString());
        }
        else {
            ReportExact("RuntimeSpecialName", left.IsRuntimeSpecialName.ToString(), right.IsRuntimeSpecialName.ToString());
        }

        if (left.IsSetter != right.IsSetter) {
            ReportChange("Setter", left.IsSetter.ToString(), right.IsSetter.ToString());
        }
        else {
            ReportExact("Setter", left.IsSetter.ToString(), right.IsSetter.ToString());
        }

        if (left.IsSpecialName != right.IsSpecialName) {
            ReportChange("SpecialName", left.IsSpecialName.ToString(), right.IsSpecialName.ToString());
        }
        else {
            ReportExact("SpecialName", left.IsSpecialName.ToString(), right.IsSpecialName.ToString());
        }

        if (left.IsStatic != right.IsStatic) {
            ReportChange("Static", left.IsStatic.ToString(), right.IsStatic.ToString());
        }
        else {
            ReportExact("Static", left.IsStatic.ToString(), right.IsStatic.ToString());
        }

        if (left.IsSynchronized != right.IsSynchronized) {
            ReportChange("Synchronized", left.IsSynchronized.ToString(), right.IsSynchronized.ToString());
        }
        else {
            ReportExact("Synchronized", left.IsSynchronized.ToString(), right.IsSynchronized.ToString());
        }

        if (left.IsUnmanaged != right.IsUnmanaged) {
            ReportChange("Unmanaged", left.IsUnmanaged.ToString(), right.IsUnmanaged.ToString());
        }
        else {
            ReportExact("Unmanaged", left.IsUnmanaged.ToString(), right.IsUnmanaged.ToString());
        }

        if (left.IsUnmanagedExport != right.IsUnmanagedExport) {
            ReportChange("UnmanagedExport", left.IsUnmanagedExport.ToString(), right.IsUnmanagedExport.ToString());
        }
        else {
            ReportExact("UnmanagedExport", left.IsUnmanagedExport.ToString(), right.IsUnmanagedExport.ToString());
        }

        if (left.IsVirtual != right.IsVirtual) {
            ReportChange("Virtual", left.IsVirtual.ToString(), right.IsVirtual.ToString());
        }
        else {
            ReportExact("Virtual", left.IsVirtual.ToString(), right.IsVirtual.ToString());
        }

        if (left.IsWindowsRuntimeProjection != right.IsWindowsRuntimeProjection) {
            ReportChange("WindowsRuntimeProjection", left.IsWindowsRuntimeProjection.ToString(), right.IsWindowsRuntimeProjection.ToString());
        }
        else {
            ReportExact("WindowsRuntimeProjection", left.IsWindowsRuntimeProjection.ToString(), right.IsWindowsRuntimeProjection.ToString());
        }

        return dir;

        void ReportChange(string reason, string x, string y) {
            dir._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.Method, x, y, reason));
        }

        void ReportExact(string reason, string x, string y) {
            dir._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Exact, HierarchicalObjectValueKind.Method, x, y, reason));
        }
    }
}
