using AsmComp.Core.Hierarchy;
using AsmComp.Core.MEF;
using JetBrains.Annotations;
using Microsoft;
using Mono.Cecil;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

namespace AsmComp.Core.Comparers;

[Export(typeof(IDotNetMetadataComparer))]
internal class FieldComparer : IDotNetMetadataComparer {
    [CanBeNull]
    public HierarchicalDirectory? Compare(
        [MaybeNull][CanBeNull][AllowNull] object x,
        [MaybeNull][CanBeNull][AllowNull] object y
    ) {
        Assumes.NotNull(x);
        Assumes.NotNull(y);

        if (x is not FieldDefinition && y is not FieldDefinition) {
            return null;
        }

        FieldDefinition left = (FieldDefinition)x;
        FieldDefinition right = (FieldDefinition)y;
        var hierarchicalDirectory = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Field");

        if (left.Name != right.Name) {
            ReportChange("Name", left.Name, right.Name);
        }
        else {
            ReportExact("Name", left.Name, right.Name);
        }

        // Compare Field Type
        if (left.FieldType.FullName != right.FieldType.FullName) {
            ReportChange("Type", left.FieldType.FullName, right.FieldType.FullName);
        }
        else {
            ReportExact("Type", left.FieldType.FullName, right.FieldType.FullName);
        }

        var leftAttributes = left.CustomAttributes;
        var rightAttributes = right.CustomAttributes;
        if (leftAttributes.Count != rightAttributes.Count ||
            !leftAttributes.All(attr => rightAttributes.Any(rAttr => rAttr.AttributeType.FullName == attr.AttributeType.FullName))) {
            ReportChange("Attributes", string.Join(", ", leftAttributes.Select(a => a.AttributeType.FullName)), string.Join(", ", rightAttributes.Select(a => a.AttributeType.FullName)));
        }
        else {
            ReportExact("Attributes", string.Join(", ", leftAttributes.Select(a => a.AttributeType.FullName)), string.Join(", ", rightAttributes.Select(a => a.AttributeType.FullName)));
        }

        if ((left.Constant?.ToString() ?? "null") != (right.Constant?.ToString() ?? "null")) {
            ReportChange("Constant", left.Constant?.ToString() ?? "null", right.Constant?.ToString() ?? "null");
        }
        else {
            ReportExact("Constant", left.Constant?.ToString() ?? "null", right.Constant?.ToString() ?? "null");
        }

        if (left.IsStatic != right.IsStatic) {
            ReportChange("Static", left.IsStatic.ToString(), right.IsStatic.ToString());
        }
        else {
            ReportExact("Static", left.IsStatic.ToString(), right.IsStatic.ToString());
        }

        if (left.IsPublic != right.IsPublic) {
            ReportChange("Public", left.IsPublic.ToString(), right.IsPublic.ToString());
        }
        else {
            ReportExact("Public", left.IsPublic.ToString(), right.IsPublic.ToString());
        }

        if (left.IsPrivate != right.IsPrivate) {
            ReportChange("Private", left.IsPrivate.ToString(), right.IsPrivate.ToString());
        }
        else {
            ReportExact("Private", left.IsPrivate.ToString(), right.IsPrivate.ToString());
        }

        if (left.IsFamily != right.IsFamily) {
            ReportChange("Family", left.IsFamily.ToString(), right.IsFamily.ToString());
        }
        else {
            ReportExact("Family", left.IsFamily.ToString(), right.IsFamily.ToString());
        }

        if (left.IsAssembly != right.IsAssembly) {
            ReportChange("Assembly", left.IsAssembly.ToString(), right.IsAssembly.ToString());
        }
        else {
            ReportExact("Assembly", left.IsAssembly.ToString(), right.IsAssembly.ToString());
        }

        return hierarchicalDirectory;

        void ReportChange(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.Field, left: x, right: y, reason: reason));
        }

        void ReportExact(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Exact, HierarchicalObjectValueKind.Field, left: x, right: y, reason: reason));
        }
    }
}
