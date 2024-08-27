using AsmComp.Core.Hierarchy;
using Mono.Cecil;
using JBNotNull = JetBrains.Annotations.NotNullAttribute;
using CANotNull = System.Diagnostics.CodeAnalysis.NotNullAttribute;

namespace AsmComp.Core.Utilities;

internal static class ParameterComparer {
    public static HierarchicalDirectory CompareSingle(
        [JBNotNull][CANotNull] ParameterDefinition left, [JBNotNull][CANotNull] ParameterDefinition right) {
        var hierarchicalDirectory = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Parameter");

        if (left.Constant != right.Constant) {
            ReportChange("Constant", left.Constant.ToString() ?? "null", right.Constant.ToString() ?? "null");
        }
        else {
            ReportExact("Constant", left.Constant.ToString() ?? "null", right.Constant.ToString() ?? "null");
        }

        var attributeData = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "ParameterAttributes");
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

        if (left.HasDefault != right.HasDefault) {
            ReportChange("HasDefault", left.HasDefault.ToString(), right.HasDefault.ToString());
        }
        else {
            ReportExact("HasDefault", left.HasDefault.ToString(), right.HasDefault.ToString());
        }

        if (left.HasFieldMarshal != right.HasFieldMarshal) {
            ReportChange("HasFieldMarshal", left.HasFieldMarshal.ToString(), right.HasFieldMarshal.ToString());
        }
        else {
            ReportExact("HasFieldMarshal", left.HasFieldMarshal.ToString(), right.HasFieldMarshal.ToString());
        }

        if (left.HasMarshalInfo != right.HasMarshalInfo) {
            ReportChange("HasMarshalInfo", left.HasMarshalInfo.ToString(), right.HasMarshalInfo.ToString());
        }
        else {
            ReportExact("HasMarshalInfo", left.HasMarshalInfo.ToString(), right.HasMarshalInfo.ToString());
        }

        if (left.Index != right.Index) {
            ReportChange("Index", left.Index.ToString(), right.Index.ToString());
        }
        else {
            ReportExact("Index", left.Index.ToString(), right.Index.ToString());
        }

        if (left.IsIn != right.IsIn) {
            ReportChange("In", left.IsIn.ToString(), right.IsIn.ToString());
        }
        else {
            ReportExact("In", left.IsIn.ToString(), right.IsIn.ToString());
        }

        if (left.IsLcid != right.IsLcid) {
            ReportChange("Lcid", left.IsLcid.ToString(), right.IsLcid.ToString());
        }
        else {
            ReportExact("Lcid", left.IsLcid.ToString(), right.IsLcid.ToString());
        }

        if (left.IsOptional != right.IsOptional) {
            ReportChange("Optional", left.IsOptional.ToString(), right.IsOptional.ToString());
        }
        else {
            ReportExact("Optional", left.IsOptional.ToString(), right.IsOptional.ToString());
        }

        if (left.IsOut != right.IsOut) {
            ReportChange("Out", left.IsOut.ToString(), right.IsOut.ToString());
        }
        else {
            ReportExact("Out", left.IsOut.ToString(), right.IsOut.ToString());
        }

        if (left.IsReturnValue != right.IsReturnValue) {
            ReportChange("IsReturnValue", left.IsReturnValue.ToString(), right.IsReturnValue.ToString());
        }
        else {
            ReportExact("IsReturnValue", left.IsReturnValue.ToString(), right.IsReturnValue.ToString());
        }

        if (left.MarshalInfo.NativeType != right.MarshalInfo.NativeType) {
            ReportChange("Marshal", left.MarshalInfo.NativeType.ToString(), right.MarshalInfo.NativeType.ToString());
        }
        else {
            ReportExact("Marshal", left.MarshalInfo.NativeType.ToString(), right.MarshalInfo.NativeType.ToString());
        }

        if (left.MetadataToken.ToUInt32() != right.MetadataToken.ToUInt32()) {
            ReportChange("MDToken", left.MetadataToken.ToUInt32().ToString(), right.MetadataToken.ToUInt32().ToString());
        }
        else {
            ReportExact("MDToken", left.MetadataToken.ToUInt32().ToString(), right.MetadataToken.ToUInt32().ToString());
        }

        if (left.Name != right.Name) {
            ReportChange("Name", left.Name, right.Name);
        }
        else {
            ReportExact("Name", left.Name, right.Name);
        }

        if (left.ParameterType.GetTypeNameLikeIL() != right.ParameterType.GetTypeNameLikeIL()) {
            ReportChange("Type", left.ParameterType.GetTypeNameLikeIL(), right.ParameterType.GetTypeNameLikeIL());
        }
        else {
            ReportExact("Type", left.ParameterType.GetTypeNameLikeIL(), right.ParameterType.GetTypeNameLikeIL());
        }

        if (left.Sequence != right.Sequence) {
            ReportChange("Sequence", left.Sequence.ToString(), right.Sequence.ToString());
        }
        else {
            ReportExact("Sequence", left.Sequence.ToString(), right.Sequence.ToString());
        }

        return hierarchicalDirectory;

        void ReportChange(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.Parameter, x, y, reason));
        }

        void ReportExact(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Exact, HierarchicalObjectValueKind.Parameter, x, y, reason));
        }
    }

    public static HierarchicalDirectory CompareAll(
         [JBNotNull][CANotNull] IEnumerable<ParameterDefinition> left,
         [JBNotNull][CANotNull] IEnumerable<ParameterDefinition> right) {
        var attributeData = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Parameters");
        int attribCountX = left.Count();
        int attribCountY = right.Count();
        if (attribCountX > attribCountY) {
            for (int i = 0; i < attribCountY; i++) {
                attributeData._hierarchicalDirectories.Add(CompareSingle(left.ElementAt(i), right.ElementAt(i)));
            }
            for (int i = attribCountY; i < attribCountX; i++) {
                attributeData._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.Parameter, left: "...", right: "...", reason: "Parameters"));
            }
        }
        else if (attribCountY > attribCountX) {
            for (int i = 0; i < attribCountX; i++) {
                attributeData._hierarchicalDirectories.Add(CompareSingle(left.ElementAt(i), right.ElementAt(i)));
            }
            for (int i = attribCountX; i < attribCountY; i++) {
                attributeData._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.Parameter, left: "...", right: "...", reason: "Parameters"));
            }
        }
        else {
            for (int i = 0; i < attribCountX; i++) {
                attributeData._hierarchicalDirectories.Add(CompareSingle(left.ElementAt(i), right.ElementAt(i)));
            }
        }
        return attributeData;
    }
}
