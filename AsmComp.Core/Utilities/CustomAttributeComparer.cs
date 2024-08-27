using AsmComp.Core.Comparers;
using AsmComp.Core.Hierarchy;
using Mono.Cecil;

namespace AsmComp.Core.Utilities;

internal static class CustomAttributeComparer {
    private static readonly MethodComparer s_methodComparer = new();

    public static HierarchicalDirectory Compare(CustomAttribute x, CustomAttribute y) {
        var dir = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "CustomAttribute");

        var result = s_methodComparer.Compare(x.Constructor, y.Constructor);
        dir._hierarchicalDirectories.Add(result!);

        if (x.AttributeType != y.AttributeType) {
            ReportChange("AttributeType", x.AttributeType.ToString(), y.AttributeType.ToString());
        }
        else {
            ReportExact("AttributeType", x.AttributeType.ToString(), y.AttributeType.ToString());
        }

        int caLengthX = x.ConstructorArguments.Count;
        int caLengthY = y.ConstructorArguments.Count;

        if (caLengthX > caLengthY) {
            var hierarchicalDirectory = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "ConstructorArguments");
            for (int i = 0; i < caLengthY; i++) {
                hierarchicalDirectory._hierarchicalDirectories.Add(CompareArgument(x.ConstructorArguments[i], y.ConstructorArguments[i]));
            }
            for (int i = caLengthY; i < caLengthX; i++) {
                hierarchicalDirectory._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.CustomAttribute, left: x.ConstructorArguments[i].Type.GetTypeNameLikeIL(), right: "...", "NamedArgument"));
            }
            dir._hierarchicalDirectories.Add(hierarchicalDirectory);
        }
        else if (caLengthY > caLengthX) {
            var hierarchicalDirectory = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "ConstructorArguments");
            for (int i = 0; i < caLengthX; i++) {
                hierarchicalDirectory._hierarchicalDirectories.Add(CompareArgument(x.ConstructorArguments[i], y.ConstructorArguments[i]));
            }
            for (int i = caLengthX; i < caLengthY; i++) {
                hierarchicalDirectory._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.CustomAttribute, left: "...", right: y.ConstructorArguments[i].Type.GetTypeNameLikeIL(), "NamedArgument"));
            }
            dir._hierarchicalDirectories.Add(hierarchicalDirectory);
        }
        else {
            var hierarchicalDirectory = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "ConstructorArguments");
            for (int i = 0; i < caLengthX; i++) {
                hierarchicalDirectory._hierarchicalDirectories.Add(CompareArgument(x.ConstructorArguments[i], y.ConstructorArguments[i]));
            }
            dir._hierarchicalDirectories.Add(hierarchicalDirectory);
        }

        int fieldLengthX = x.Fields.Count;
        int fieldLengthY = y.Fields.Count;

        if (fieldLengthX > fieldLengthY) {
            var hierarchicalDirectory = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Fields");
            for (int i = 0; i < fieldLengthX; i++) {
                hierarchicalDirectory._hierarchicalDirectories.Add(CompareNamedArgument(x.Fields[i], y.Fields[i]));
            }
            for (int i = fieldLengthY; i < fieldLengthX; i++) {
                hierarchicalDirectory._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.CustomAttribute, left: x.ConstructorArguments[i].Type.GetTypeNameLikeIL(), right: "...", "NamedArgument"));
            }
            dir._hierarchicalDirectories.Add(hierarchicalDirectory);
        }
        else if (fieldLengthY > fieldLengthX) {
            var hierarchicalDirectory = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Fields");
            for (int i = 0; i < fieldLengthX; i++) {
                hierarchicalDirectory._hierarchicalDirectories.Add(CompareNamedArgument(x.Fields[i], y.Fields[i]));
            }
            for (int i = fieldLengthX; i < fieldLengthY; i++) {
                hierarchicalDirectory._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.CustomAttribute, left: "...", right: y.ConstructorArguments[i].Type.GetTypeNameLikeIL(), "NamedArgument"));
            }
            dir._hierarchicalDirectories.Add(hierarchicalDirectory);
        }
        else {
            var hierarchicalDirectory = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Fields");
            for (int i = 0; i < fieldLengthX; i++) {
                hierarchicalDirectory._hierarchicalDirectories.Add(CompareNamedArgument(x.Fields[i], y.Fields[i]));
            }
            dir._hierarchicalDirectories.Add(hierarchicalDirectory);
        }

        fieldLengthX = x.Properties.Count;
        fieldLengthY = y.Properties.Count;

        try {
            if (fieldLengthX > fieldLengthY) {
                var hierarchicalDirectory = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Properties");
                for (int i = 0; i < fieldLengthY; i++) {
                    hierarchicalDirectory._hierarchicalDirectories.Add(CompareNamedArgument(x.Properties[i], y.Properties[i]));
                }
                for (int i = fieldLengthY; i < fieldLengthX; i++) {
                    hierarchicalDirectory._hierarchicalObjects.Add(
                        new HierarchicalObject(
                            HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.CustomAttribute, left: x.Properties[i].Argument.Type.GetTypeNameLikeIL(), right: "...", "Properties"));
                }
                dir._hierarchicalDirectories.Add(hierarchicalDirectory);
            }
            else if (fieldLengthY > fieldLengthX) {
                var hierarchicalDirectory = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Properties");
                for (int i = 0; i < fieldLengthX; i++) {
                    hierarchicalDirectory._hierarchicalDirectories.Add(CompareNamedArgument(x.Properties[i], y.Properties[i]));
                }
                for (int i = fieldLengthX; i < fieldLengthY; i++) {
                    hierarchicalDirectory._hierarchicalObjects.Add(
                        new HierarchicalObject(
                            HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.CustomAttribute, left: "...", right: y.ConstructorArguments[i].Type.GetTypeNameLikeIL(), "NamedArgument"));
                }
                dir._hierarchicalDirectories.Add(hierarchicalDirectory);
            }
            else {
                var hierarchicalDirectory = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Properties");
                for (int i = 0; i < fieldLengthX; i++) {
                    hierarchicalDirectory._hierarchicalDirectories.Add(CompareNamedArgument(x.Properties[i], y.Properties[i]));
                }
                dir._hierarchicalDirectories.Add(hierarchicalDirectory);
            }
        }
        catch {
        }

        if (x.GetBlob().ToHexString() != y.GetBlob().ToHexString()) {
            ReportChange("Blob", "...", "...");
        }
        else {
            ReportExact("Blob", "...", "...");
        }

        return dir;

        void ReportExact(string reason, string x, string y) {
            dir._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Exact, HierarchicalObjectValueKind.CustomAttribute, x, y, reason));
        }

        void ReportChange(string reason, string x, string y) {
            dir._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.CustomAttribute, x, y, reason));
        }
    }

    private static HierarchicalDirectory CompareNamedArgument(CustomAttributeNamedArgument x, CustomAttributeNamedArgument y) {
        var dir = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "NamedArgument");

        dir._hierarchicalDirectories.Add(CompareArgument(x.Argument, y.Argument));
        
        if (x.Name != y.Name) {
            ReportChange("Name", x.Name, y.Name);
        }
        else {
            ReportExact("Name", x.Name, y.Name);
        }

        return dir;

        void ReportExact(string reason, string x, string y) {
            dir._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Exact, HierarchicalObjectValueKind.CustomAttribute, x, y, reason));
        }

        void ReportChange(string reason, string x, string y) {
            dir._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.CustomAttribute, x, y, reason));
        }
    }

    private static HierarchicalDirectory CompareArgument(CustomAttributeArgument x, CustomAttributeArgument y) {
        var dir = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "NamedArgument");

        if (x.Value != y.Value) {
            ReportChange("Value", x.Value.ToString() ?? "null", y.Value.ToString() ?? "null");
        }
        else {
            ReportExact("Value", x.Value.ToString() ?? "null", y.Value.ToString() ?? "null");
        }

        if (x.Type.GetTypeNameLikeIL() != y.Type.GetTypeNameLikeIL()) {
            ReportChange("Type", x.Type.GetTypeNameLikeIL(), y.Type.GetTypeNameLikeIL());
        }
        else {
            ReportExact("Type", x.Type.GetTypeNameLikeIL(), y.Type.GetTypeNameLikeIL());
        }

        return dir;

        void ReportExact(string reason, string x, string y) {
            dir._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Exact, HierarchicalObjectValueKind.CustomAttribute, x, y, reason));
        }

        void ReportChange(string reason, string x, string y) {
            dir._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.CustomAttribute, x, y, reason));
        }
    }
}
