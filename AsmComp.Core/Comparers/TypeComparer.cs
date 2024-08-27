using AsmComp.Core.Hierarchy;
using AsmComp.Core.MEF;
using JetBrains.Annotations;
using Microsoft;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.Composition;
using Mono.Cecil;
using AsmComp.Core.Utilities;

namespace AsmComp.Core.Comparers;

[Export(typeof(IDotNetMetadataComparer))]
internal class TypeComparer : IDotNetMetadataComparer {
    private static readonly FieldComparer s_fieldComparer = new();
    private static readonly PropertyComparer s_propertyComparer = new();
    private static readonly MethodComparer s_methodComparer = new();
    private static readonly EventComparer s_eventComparer = new();

    [CanBeNull]
    public HierarchicalDirectory? Compare(
        [MaybeNull][CanBeNull][AllowNull] object x,
        [MaybeNull][CanBeNull][AllowNull] object y
    ) {
        Assumes.NotNull(x);
        Assumes.NotNull(y);

        if (x is not TypeDefinition && y is not TypeDefinition) {
            return null;
        }

        TypeDefinition left = (TypeDefinition)x;
        TypeDefinition right = (TypeDefinition)y;

        var hierarchicalDirectory = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Type");
        if (left.BaseType?.GetTypeNameLikeIL() != right.BaseType?.GetTypeNameLikeIL()) {
            ReportChange("BaseType", left.BaseType?.GetTypeNameLikeIL() ?? "null", right.GetTypeNameLikeIL() ?? "null");
        }
        else {
            ReportExact("BaseType", left.BaseType?.GetTypeNameLikeIL() ?? "null", right.GetTypeNameLikeIL() ?? "null");
        }

        if (left.ClassSize != right.ClassSize) {
            ReportChange("ClassSize", left.ClassSize.ToString(), right.ClassSize.ToString());
        }
        else {
            ReportExact("ClassSize", left.ClassSize.ToString(), right.ClassSize.ToString());
        }

        if (left.ContainsGenericParameter != right.ContainsGenericParameter) {
            ReportChange("ContainsGenericParameter", left.ContainsGenericParameter.ToString(), right.ContainsGenericParameter.ToString());
        }
        else {
            ReportExact("ContainsGenericParameter", left.ContainsGenericParameter.ToString(), right.ContainsGenericParameter.ToString());
        }

        if (left.DeclaringType?.GetTypeNameLikeIL() != right.DeclaringType?.GetTypeNameLikeIL()) {
            ReportChange("DeclaringType", left.DeclaringType?.GetTypeNameLikeIL() ?? "null", right.DeclaringType?.GetTypeNameLikeIL() ?? "null");
        }
        else {
            ReportExact("DeclaringType", left.DeclaringType?.GetTypeNameLikeIL() ?? "null", right.DeclaringType?.GetTypeNameLikeIL() ?? "null");
        }

        if (left.GetTypeNameLikeIL() != right.GetTypeNameLikeIL()) {
            ReportChange("Name", left.GetTypeNameLikeIL(), right.GetTypeNameLikeIL());
        }
        else {
            ReportExact("Name", left.GetTypeNameLikeIL(), right.GetTypeNameLikeIL());
        }

        if (left.MetadataToken.ToUInt32() != right.MetadataToken.ToUInt32()) {
            ReportChange("MDToken", left.MetadataToken.ToUInt32().ToString(), right.MetadataToken.ToUInt32().ToString());
        }
        else {
            ReportExact("MDToken", left.MetadataToken.ToUInt32().ToString(), right.MetadataToken.ToUInt32().ToString());
        }

        if (left.MetadataType != right.MetadataType) {
            ReportChange("MDType", left.MetadataType.ToString(), right.MetadataType.ToString());
        }
        else {
            ReportExact("MDType", left.MetadataType.ToString(), right.MetadataType.ToString());
        }

        if (left.Name != right.Name) {
            ReportChange("SimpleName", left.Name, right.Name);
        }
        else {
            ReportExact("SimpleName", left.Name, right.Name);
        }

        if (left.Namespace != right.Namespace) {
            ReportChange("Namespace", left.Namespace, right.Namespace);
        }
        else {
            ReportExact("Namespace", left.Namespace, right.Namespace);
        }

        if (left.PackingSize != right.PackingSize) {
            ReportChange("PackingSize", left.PackingSize.ToString(), right.PackingSize.ToString());
        }
        else {
            ReportExact("PackingSize", left.PackingSize.ToString(), right.PackingSize.ToString());
        }


        if (left.IsAbstract != right.IsAbstract) {
            ReportChange("Abstract", left.IsAbstract.ToString(), right.IsAbstract.ToString());
        }
        else {
            ReportExact("Abstract", left.IsAbstract.ToString(), right.IsAbstract.ToString());
        }

        if (left.IsAnsiClass != right.IsAnsiClass) {
            ReportChange("AnsiClass", left.IsAnsiClass.ToString(), right.IsAnsiClass.ToString());
        }
        else {
            ReportExact("AnsiClass", left.IsAnsiClass.ToString(), right.IsAnsiClass.ToString());
        }

        if (left.IsAutoClass != right.IsAutoClass) {
            ReportChange("AutoClass", left.IsAutoClass.ToString(), right.IsAutoClass.ToString());
        }
        else {
            ReportExact("AutoClass", left.IsAutoClass.ToString(), right.IsAutoClass.ToString());
        }

        if (left.IsAutoLayout != right.IsAutoLayout) {
            ReportChange("AutoLayout", left.IsAutoLayout.ToString(), right.IsAutoLayout.ToString());
        }
        else {
            ReportExact("AutoLayout", left.IsAutoLayout.ToString(), right.IsAutoLayout.ToString());
        }

        if (left.IsBeforeFieldInit != right.IsBeforeFieldInit) {
            ReportChange("BeforeFieldInit", left.IsBeforeFieldInit.ToString(), right.IsBeforeFieldInit.ToString());
        }
        else {
            ReportExact("BeforeFieldInit", left.IsBeforeFieldInit.ToString(), right.IsBeforeFieldInit.ToString());
        }

        if (left.IsByReference != right.IsByReference) {
            ReportChange("ByReference", left.IsByReference.ToString(), right.IsByReference.ToString());
        }
        else {
            ReportExact("ByReference", left.IsByReference.ToString(), right.IsByReference.ToString());
        }

        if (left.IsClass != right.IsClass) {
            ReportChange("Class", left.IsClass.ToString(), right.IsClass.ToString());
        }
        else {
            ReportExact("Class", left.IsClass.ToString(), right.IsClass.ToString());
        }

        if (left.IsDefinition != right.IsDefinition) {
            ReportChange("Definition", left.IsDefinition.ToString(), right.IsDefinition.ToString());
        }
        else {
            ReportExact("Definition", left.IsDefinition.ToString(), right.IsDefinition.ToString());
        }

        if (left.IsEnum != right.IsEnum) {
            ReportChange("Enum", left.IsEnum.ToString(), right.IsEnum.ToString());
        }
        else {
            ReportExact("Enum", left.IsEnum.ToString(), right.IsEnum.ToString());
        }

        if (left.IsExplicitLayout != right.IsExplicitLayout) {
            ReportChange("ExplicitLayout", left.IsExplicitLayout.ToString(), right.IsExplicitLayout.ToString());
        }
        else {
            ReportExact("ExplicitLayout", left.IsExplicitLayout.ToString(), right.IsExplicitLayout.ToString());
        }

        if (left.IsFunctionPointer != right.IsFunctionPointer) {
            ReportChange("FunctionPointer", left.IsFunctionPointer.ToString(), right.IsFunctionPointer.ToString());
        }
        else {
            ReportExact("FunctionPointer", left.IsFunctionPointer.ToString(), right.IsFunctionPointer.ToString());
        }

        if (left.IsGenericInstance != right.IsGenericInstance) {
            ReportChange("GenericInstance", left.IsGenericInstance.ToString(), right.IsGenericInstance.ToString());
        }
        else {
            ReportExact("GenericInstance", left.IsGenericInstance.ToString(), right.IsGenericInstance.ToString());
        }

        if (left.IsGenericParameter != right.IsGenericParameter) {
            ReportChange("GenericParameter", left.IsGenericParameter.ToString(), right.IsGenericParameter.ToString());
        }
        else {
            ReportExact("GenericParameter", left.IsGenericParameter.ToString(), right.IsGenericParameter.ToString());
        }

        if (left.IsImport != right.IsImport) {
            ReportChange("Import", left.IsImport.ToString(), right.IsImport.ToString());
        }
        else {
            ReportExact("Import", left.IsImport.ToString(), right.IsImport.ToString());
        }

        if (left.IsInterface != right.IsInterface) {
            ReportChange("Interface", left.IsInterface.ToString(), right.IsInterface.ToString());
        }
        else {
            ReportExact("Interface", left.IsInterface.ToString(), right.IsInterface.ToString());
        }

        if (left.IsNested != right.IsNested) {
            ReportChange("Nested", left.IsNested.ToString(), right.IsNested.ToString());
        }
        else {
            ReportExact("Nested", left.IsNested.ToString(), right.IsNested.ToString());
        }

        if (left.IsNestedAssembly != right.IsNestedAssembly) {
            ReportChange("NestedAssembly", left.IsNestedAssembly.ToString(), right.IsNestedAssembly.ToString());
        }
        else {
            ReportExact("NestedAssembly", left.IsNestedAssembly.ToString(), right.IsNestedAssembly.ToString());
        }

        if (left.IsNestedFamily != right.IsNestedFamily) {
            ReportChange("NestedFamily", left.IsNestedFamily.ToString(), right.IsNestedFamily.ToString());
        }
        else {
            ReportExact("NestedFamily", left.IsNestedFamily.ToString(), right.IsNestedFamily.ToString());
        }

        if (left.IsNestedFamilyAndAssembly != right.IsNestedFamilyAndAssembly) {
            ReportChange("NestedFamilyAndAssembly", left.IsNestedFamilyAndAssembly.ToString(), right.IsNestedFamilyAndAssembly.ToString());
        }
        else {
            ReportExact("NestedFamilyAndAssembly", left.IsNestedFamilyAndAssembly.ToString(), right.IsNestedFamilyAndAssembly.ToString());
        }

        if (left.IsNestedFamilyOrAssembly != right.IsNestedFamilyOrAssembly) {
            ReportChange("NestedFamilyOrAssembly", left.IsNestedFamilyOrAssembly.ToString(), right.IsNestedFamilyOrAssembly.ToString());
        }
        else {
            ReportExact("NestedFamilyOrAssembly", left.IsNestedFamilyOrAssembly.ToString(), right.IsNestedFamilyOrAssembly.ToString());
        }

        if (left.IsNestedPrivate != right.IsNestedPrivate) {
            ReportChange("NestedPrivate", left.IsNestedPrivate.ToString(), right.IsNestedPrivate.ToString());
        }
        else {
            ReportExact("NestedPrivate", left.IsNestedPrivate.ToString(), right.IsNestedPrivate.ToString());
        }

        if (left.IsNestedPublic != right.IsNestedPublic) {
            ReportChange("NestedPublic", left.IsNestedPublic.ToString(), right.IsNestedPublic.ToString());
        }
        else {
            ReportExact("NestedPublic", left.IsNestedPublic.ToString(), right.IsNestedPublic.ToString());
        }

        if (left.IsNotPublic != right.IsNotPublic) {
            ReportChange("NotPublic", left.IsNotPublic.ToString(), right.IsNotPublic.ToString());
        }
        else {
            ReportExact("NotPublic", left.IsNotPublic.ToString(), right.IsNotPublic.ToString());
        }

        if (left.IsOptionalModifier != right.IsOptionalModifier) {
            ReportChange("OptionalModifier", left.IsOptionalModifier.ToString(), right.IsOptionalModifier.ToString());
        }
        else {
            ReportExact("OptionalModifier", left.IsOptionalModifier.ToString(), right.IsOptionalModifier.ToString());
        }

        if (left.IsPinned != right.IsPinned) {
            ReportChange("Pinned", left.IsPinned.ToString(), right.IsPinned.ToString());
        }
        else {
            ReportExact("Pinned", left.IsPinned.ToString(), right.IsPinned.ToString());
        }

        if (left.IsPointer != right.IsPointer) {
            ReportChange("Pointer", left.IsPointer.ToString(), right.IsPointer.ToString());
        }
        else {
            ReportExact("Pointer", left.IsPointer.ToString(), right.IsPointer.ToString());
        }

        if (left.IsPrimitive != right.IsPrimitive) {
            ReportChange("Primitive", left.IsPrimitive.ToString(), right.IsPrimitive.ToString());
        }
        else {
            ReportExact("Primitive", left.IsPrimitive.ToString(), right.IsPrimitive.ToString());
        }

        if (left.IsPublic != right.IsPublic) {
            ReportChange("Public", left.IsPublic.ToString(), right.IsPublic.ToString());
        }
        else {
            ReportExact("Public", left.IsPublic.ToString(), right.IsPublic.ToString());
        }

        if (left.IsRequiredModifier != right.IsRequiredModifier) {
            ReportChange("RequiredModifier", left.IsRequiredModifier.ToString(), right.IsRequiredModifier.ToString());
        }
        else {
            ReportExact("RequiredModifier", left.IsRequiredModifier.ToString(), right.IsRequiredModifier.ToString());
        }

        if (left.IsRuntimeSpecialName != right.IsRuntimeSpecialName) {
            ReportChange("RuntimeSpecialName", left.IsRuntimeSpecialName.ToString(), right.IsRuntimeSpecialName.ToString());
        }
        else {
            ReportExact("RuntimeSpecialName", left.IsRuntimeSpecialName.ToString(), right.IsRuntimeSpecialName.ToString());
        }

        if (left.IsSealed != right.IsSealed) {
            ReportChange("Sealed", left.IsSealed.ToString(), right.IsSealed.ToString());
        }
        else {
            ReportExact("Sealed", left.IsSealed.ToString(), right.IsSealed.ToString());
        }

        if (left.IsSentinel != right.IsSentinel) {
            ReportChange("Sentinel", left.IsSentinel.ToString(), right.IsSentinel.ToString());
        }
        else {
            ReportExact("Sentinel", left.IsSentinel.ToString(), right.IsSentinel.ToString());
        }

        if (left.IsSequentialLayout != right.IsSequentialLayout) {
            ReportChange("SequentialLayout", left.IsSequentialLayout.ToString(), right.IsSequentialLayout.ToString());
        }
        else {
            ReportExact("SequentialLayout", left.IsSequentialLayout.ToString(), right.IsSequentialLayout.ToString());
        }

        if (left.IsSerializable != right.IsSerializable) {
            ReportChange("Serializable", left.IsSerializable.ToString(), right.IsSerializable.ToString());
        }
        else {
            ReportExact("Serializable", left.IsSerializable.ToString(), right.IsSerializable.ToString());
        }

        if (left.IsSpecialName != right.IsSpecialName) {
            ReportChange("SpecialName", left.IsSpecialName.ToString(), right.IsSpecialName.ToString());
        }
        else {
            ReportExact("SpecialName", left.IsSpecialName.ToString(), right.IsSpecialName.ToString());
        }

        if (left.IsUnicodeClass != right.IsUnicodeClass) {
            ReportChange("UnicodeClass", left.IsUnicodeClass.ToString(), right.IsUnicodeClass.ToString());
        }
        else {
            ReportExact("UnicodeClass", left.IsUnicodeClass.ToString(), right.IsUnicodeClass.ToString());
        }

        if (left.IsValueType != right.IsValueType) {
            ReportChange("ValueType", left.IsValueType.ToString(), right.IsValueType.ToString());
        }
        else {
            ReportExact("ValueType", left.IsValueType.ToString(), right.IsValueType.ToString());
        }

        if (left.IsWindowsRuntime != right.IsWindowsRuntime) {
            ReportChange("WindowsRuntime", left.IsWindowsRuntime.ToString(), right.IsWindowsRuntime.ToString());
        }
        else {
            ReportExact("WindowsRuntime", left.IsWindowsRuntime.ToString(), right.IsWindowsRuntime.ToString());
        }

        if (left.IsWindowsRuntimeProjection != right.IsWindowsRuntimeProjection) {
            ReportChange("WindowsRuntimeProjection", left.IsWindowsRuntimeProjection.ToString(), right.IsWindowsRuntimeProjection.ToString());
        }
        else {
            ReportExact("WindowsRuntimeProjection", left.IsWindowsRuntimeProjection.ToString(), right.IsWindowsRuntimeProjection.ToString());
        }

        int leftFieldCount = left.Fields.Count;
        int rightFieldCount = right.Fields.Count;
        var fieldData = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Fields");
        if (leftFieldCount > rightFieldCount) {
            for (int i = 0; i < rightFieldCount; i++) {
                fieldData._hierarchicalDirectories.Add(s_fieldComparer.Compare(left.Fields[i], left.Fields[i])!);
            }
            for (int i = rightFieldCount - 1; i < leftFieldCount; i++) {
                fieldData._hierarchicalObjects.Add(new HierarchicalObject(HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.Field, left: "...", right: "[none]", reason: "Field"));
            }
        }
        else if (rightFieldCount > leftFieldCount) {
            for (int i = 0; i < leftFieldCount; i++) {
                fieldData._hierarchicalDirectories.Add(s_fieldComparer.Compare(left.Fields[i], right.Fields[i])!);
            }
            for (int i = leftFieldCount - 1; i < rightFieldCount; i++) {
                fieldData._hierarchicalObjects.Add(new HierarchicalObject(HierarchicalObjectKind.Substitute, HierarchicalObjectValueKind.Field, left: "[none]", right: "...", reason: "Field"));
            }
        }
        else {
            for (int i = 0; i < leftFieldCount; i++) {
                fieldData._hierarchicalDirectories.Add(s_fieldComparer.Compare(left.Fields[i], right.Fields[i])!);
            }
        }
        hierarchicalDirectory._hierarchicalDirectories.Add(fieldData);

        int leftPropertiesCount = left.Properties.Count;
        int rightPropertiesCount = left.Properties.Count;
        var propertyData = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Properties");
        if (leftPropertiesCount > rightPropertiesCount) {
            for (int i = 0; i < rightPropertiesCount; i++) {
                propertyData._hierarchicalDirectories.Add(s_propertyComparer.Compare(left.Properties[i], left.Properties[i])!);
            }
            for (int i = rightPropertiesCount - 1; i < leftPropertiesCount; i++) {
                propertyData._hierarchicalObjects.Add(new HierarchicalObject(HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.Property, left: "...", right: "[none]", reason: "Property"));
            }
        }
        else if (rightPropertiesCount > leftPropertiesCount) {
            for (int i = 0; i < leftPropertiesCount; i++) {
                propertyData._hierarchicalDirectories.Add(s_propertyComparer.Compare(left.Properties[i], right.Properties[i])!);
            }
            for (int i = leftPropertiesCount - 1; i < rightPropertiesCount; i++) {
                propertyData._hierarchicalObjects.Add(new HierarchicalObject(HierarchicalObjectKind.Substitute, HierarchicalObjectValueKind.Property, left: "[none]", right: "...", reason: "Property"));
            }
        }
        else {
            for (int i = 0; i < leftPropertiesCount; i++) {
                propertyData._hierarchicalDirectories.Add(s_propertyComparer.Compare(left.Properties[i], right.Properties[i])!);
            }
        }
        hierarchicalDirectory._hierarchicalDirectories.Add(propertyData);

        int leftMethodsCount = left.Methods.Count;
        int rightMethodsCount = left.Methods.Count;
        var methodData = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Methods");
        if (leftMethodsCount > rightMethodsCount) {
            for (int i = 0; i < rightMethodsCount; i++) {
                methodData._hierarchicalDirectories.Add(s_methodComparer.Compare(left.Methods[i], left.Methods[i])!);
            }
            for (int i = rightMethodsCount - 1; i < leftMethodsCount; i++) {
                methodData._hierarchicalObjects.Add(new HierarchicalObject(HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.Method, left: "...", right: "[none]", reason: "Method"));
            }
        }
        else if (rightMethodsCount > leftMethodsCount) {
            for (int i = 0; i < leftMethodsCount; i++) {
                methodData._hierarchicalDirectories.Add(s_methodComparer.Compare(left.Methods[i], right.Methods[i])!);
            }
            for (int i = leftMethodsCount - 1; i < rightMethodsCount; i++) {
                methodData._hierarchicalObjects.Add(new HierarchicalObject(HierarchicalObjectKind.Substitute, HierarchicalObjectValueKind.Method, left: "[none]", right: "...", reason: "Method"));
            }
        }
        else {
            for (int i = 0; i < leftMethodsCount; i++) {
                methodData._hierarchicalDirectories.Add(s_methodComparer.Compare(left.Methods[i], right.Methods[i])!);
            }
        }
        hierarchicalDirectory._hierarchicalDirectories.Add(methodData);

        int leftEventsCount = left.Events.Count;
        int rightEventsCount = left.Events.Count;
        var eventData = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Events");
        if (leftEventsCount > rightEventsCount) {
            for (int i = 0; i < rightEventsCount; i++) {
                eventData._hierarchicalDirectories.Add(s_eventComparer.Compare(left.Events[i], left.Events[i])!);
            }
            for (int i = rightEventsCount - 1; i < leftEventsCount; i++) {
                eventData._hierarchicalObjects.Add(new HierarchicalObject(HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.Event, left: "...", right: "[none]", reason: "Event"));
            }
        }
        else if (rightEventsCount > leftEventsCount) {
            for (int i = 0; i < leftEventsCount; i++) {
                eventData._hierarchicalDirectories.Add(s_eventComparer.Compare(left.Events[i], right.Events[i])!);
            }
            for (int i = leftEventsCount - 1; i < rightEventsCount; i++) {
                eventData._hierarchicalObjects.Add(new HierarchicalObject(HierarchicalObjectKind.Substitute, HierarchicalObjectValueKind.Event, left: "[none]", right: "...", reason: "Event"));
            }
        }
        else {
            for (int i = 0; i < leftEventsCount; i++) {
                eventData._hierarchicalDirectories.Add(s_eventComparer.Compare(left.Events[i], right.Events[i])!);
            }
        }
        hierarchicalDirectory._hierarchicalDirectories.Add(eventData);

        int leftTypesCount = left.NestedTypes.Count;
        int rightTypesCount = left.NestedTypes.Count;
        var typeData = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Types");
        if (leftTypesCount > rightTypesCount) {
            for (int i = 0; i < rightTypesCount; i++) {
                typeData._hierarchicalDirectories.Add(Compare(left.NestedTypes[i], left.NestedTypes[i])!);
            }
            for (int i = rightTypesCount - 1; i < leftTypesCount; i++) {
                typeData._hierarchicalObjects.Add(new HierarchicalObject(HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.Type, left: "...", right: "[none]", reason: "Type"));
            }
        }
        else if (rightTypesCount > leftTypesCount) {
            for (int i = 0; i < leftTypesCount; i++) {
                typeData._hierarchicalDirectories.Add(Compare(left.NestedTypes[i], right.NestedTypes[i])!);
            }
            for (int i = leftTypesCount - 1; i < rightTypesCount; i++) {
                typeData._hierarchicalObjects.Add(new HierarchicalObject(HierarchicalObjectKind.Substitute, HierarchicalObjectValueKind.Type, left: "[none]", right: "...", reason: "Type"));
            }
        }
        else {
            for (int i = 0; i < leftTypesCount; i++) {
                typeData._hierarchicalDirectories.Add(Compare(left.NestedTypes[i], right.NestedTypes[i])!);
            }
        }
        hierarchicalDirectory._hierarchicalDirectories.Add(typeData);

        int leftCustomAttributesCount = left.CustomAttributes.Count;
        int rightCustomAttributesCount = left.CustomAttributes.Count;
        var attributeData = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "CustomAttributes");
        if (leftCustomAttributesCount > rightCustomAttributesCount) {
            for (int i = 0; i < rightCustomAttributesCount; i++) {
                attributeData._hierarchicalDirectories.Add(CustomAttributeComparer.Compare(left.CustomAttributes[i], left.CustomAttributes[i])!);
            }
            for (int i = rightCustomAttributesCount - 1; i < leftCustomAttributesCount; i++) {
                attributeData._hierarchicalObjects.Add(new HierarchicalObject(HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.CustomAttribute, left: "...", right: "[none]", reason: "CustomAttribute"));
            }
        }
        else if (rightCustomAttributesCount > leftCustomAttributesCount) {
            for (int i = 0; i < leftCustomAttributesCount; i++) {
                attributeData._hierarchicalDirectories.Add(CustomAttributeComparer.Compare(left.CustomAttributes[i], right.CustomAttributes[i])!);
            }
            for (int i = leftCustomAttributesCount - 1; i < rightCustomAttributesCount; i++) {
                attributeData._hierarchicalObjects.Add(new HierarchicalObject(HierarchicalObjectKind.Substitute, HierarchicalObjectValueKind.CustomAttribute, left: "[none]", right: "...", reason: "CustomAttribute"));
            }
        }
        else {
            for (int i = 0; i < leftCustomAttributesCount; i++) {
                attributeData._hierarchicalDirectories.Add(CustomAttributeComparer.Compare(left.CustomAttributes[i], right.CustomAttributes[i])!);
            }
        }
        hierarchicalDirectory._hierarchicalDirectories.Add(attributeData);

        return hierarchicalDirectory;

        void ReportExact(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Exact, HierarchicalObjectValueKind.Type, x, y, reason));
        }

        void ReportChange(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.Type, x, y, reason));
        }
    }
}
