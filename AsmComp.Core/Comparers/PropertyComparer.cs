using AsmComp.Core.Hierarchy;
using AsmComp.Core.MEF;
using AsmComp.Core.Utilities;
using JetBrains.Annotations;
using Microsoft;
using Mono.Cecil;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

namespace AsmComp.Core.Comparers;

[Export(typeof(IDotNetMetadataComparer))]
internal class PropertyComparer : IDotNetMetadataComparer {
    private static readonly MethodComparer s_methodComparer = new();

    [CanBeNull]
    public HierarchicalDirectory? Compare(
        [MaybeNull][CanBeNull][AllowNull] object x,
        [MaybeNull][CanBeNull][AllowNull] object y
    ) {
        Assumes.NotNull(x);
        Assumes.NotNull(y);

        if (x is not PropertyDefinition && y is not PropertyDefinition) {
            return null;
        }

        PropertyDefinition left = (PropertyDefinition)x;
        PropertyDefinition right = (PropertyDefinition)y;
        var hierarchicalDirectory = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Property");

        if (left.Name != right.Name) {
            ReportChange("Name", left.Name, right.Name);
        }
        else {
            ReportExact("Name", left.Name, right.Name);
        }

        var leftGetMethod = left.GetMethod;
        var rightGetMethod = right.GetMethod;
        var leftSetMethod = left.SetMethod;
        var rightSetMethod = right.SetMethod;

        CompareMethod("Getter", leftGetMethod, rightGetMethod);
        CompareMethod("Setter", leftSetMethod, rightSetMethod);

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

        if (left.IsDefinition != right.IsDefinition) {
            ReportChange("Definition", left.IsDefinition.ToString(), right.IsDefinition.ToString());
        }
        else {
            ReportExact("Definition", left.IsDefinition.ToString(), right.IsDefinition.ToString());
        }

        if (left.IsRuntimeSpecialName != right.IsRuntimeSpecialName) {
            ReportChange("RuntimeSpecialName", left.IsRuntimeSpecialName.ToString(), right.IsRuntimeSpecialName.ToString());
        }
        else {
            ReportExact("RuntimeSpecialName", left.IsRuntimeSpecialName.ToString(), right.IsRuntimeSpecialName.ToString());
        }

        if (left.IsSpecialName != right.IsSpecialName) {
            ReportChange("SpecialName", left.IsSpecialName.ToString(), right.IsSpecialName.ToString());
        }
        else {
            ReportExact("SpecialName", left.IsSpecialName.ToString(), right.IsSpecialName.ToString());
        }

        if (left.IsWindowsRuntimeProjection != right.IsWindowsRuntimeProjection) {
            ReportChange("WindowsRuntimeProjection", left.IsWindowsRuntimeProjection.ToString(), right.IsWindowsRuntimeProjection.ToString());
        }
        else {
            ReportExact("WindowsRuntimeProjection", left.IsWindowsRuntimeProjection.ToString(), right.IsWindowsRuntimeProjection.ToString());
        }

        if (left.Constant != right.Constant) {
            ReportChange("Constant", left.Constant?.ToString() ?? "null", right.Constant?.ToString() ?? "null");
        }
        else {
            ReportExact("Constant", left.Constant?.ToString() ?? "null", right.Constant?.ToString() ?? "null");
        }

        if (left.MetadataToken.ToUInt32() != right.MetadataToken.ToUInt32()) {
            ReportChange("MDToken", left.MetadataToken.ToUInt32().ToString(), right.MetadataToken.ToUInt32().ToString());
        }
        else {
            ReportExact("MDToken", left.MetadataToken.ToUInt32().ToString(), right.MetadataToken.ToUInt32().ToString());
        }

        if (left.HasParameters != right.HasParameters) {
            ReportChange("HasParameters", left.HasParameters.ToString(), right.HasParameters.ToString());
        }
        else {
            ReportExact("HasParameters", left.HasParameters.ToString(), right.HasParameters.ToString());
            if (left.HasParameters && right.HasParameters) {
                hierarchicalDirectory._hierarchicalDirectories.Add(ParameterComparer.CompareAll(left.Parameters, right.Parameters));
            }
        }

        if (left.PropertyType.GetTypeNameLikeIL() != right.PropertyType.GetTypeNameLikeIL()) {
            ReportChange("Type", left.PropertyType.GetTypeNameLikeIL(), right.PropertyType.GetTypeNameLikeIL());
        }
        else {
            ReportExact("Type", left.PropertyType.GetTypeNameLikeIL(), right.PropertyType.GetTypeNameLikeIL());
        }

        return hierarchicalDirectory;

        void ReportChange(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.Property, left: x, right: y, reason: reason));
        }

        void ReportExact(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Exact, HierarchicalObjectValueKind.Property, left: x, right: y, reason: reason));
        }

        void CompareMethod(string methodType, MethodDefinition? leftMethod, MethodDefinition? rightMethod) {
            if (leftMethod == null && rightMethod == null) {
                ReportExact($"{methodType} Method", "null", "null");
            }
            else if (leftMethod == null || rightMethod == null) {
                ReportChange($"{methodType} Method", leftMethod?.ToString() ?? "null", rightMethod?.ToString() ?? "null");
            }
            else {
                var methodComparer = s_methodComparer;
                var methodDirectory = methodComparer.Compare(leftMethod, rightMethod);
                if (methodDirectory != null) {
                    hierarchicalDirectory._hierarchicalDirectories.Add(methodDirectory);
                }
            }
        }
    }
}
