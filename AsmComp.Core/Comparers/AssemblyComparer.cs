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
internal class AssemblyComparer : IDotNetMetadataComparer {
    private static readonly ModuleComparer s_moduleComparer = new();

    [CanBeNull]
    public HierarchicalDirectory? Compare(
        [MaybeNull][CanBeNull][AllowNull] object x,
        [MaybeNull][CanBeNull][AllowNull] object y
    ) {
        Assumes.NotNull(x);
        Assumes.NotNull(y);

        if (x is not AssemblyDefinition && y is not AssemblyDefinition) {
            return null;
        }

        AssemblyDefinition left = (AssemblyDefinition)x;
        AssemblyDefinition right = (AssemblyDefinition)y;
        var hierarchicalDirectory = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Assembly");

        if (left.FullName != right.FullName) {
            ReportChange("FullName", left.FullName, right.FullName);
        }
        else {
            ReportExact("FullName", left.FullName, right.FullName);
        }

        if (left.MetadataToken.ToUInt32() != right.MetadataToken.ToUInt32()) {
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
        hierarchicalDirectory._hierarchicalDirectories.Add(ModuleComparer.CompareAssemblyNameReference(left.Name, right.Name));

        var moduleData = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Modules");
        int moduleCountX = left.Modules.Count;
        int moduleCountY = right.Modules.Count;
        if (moduleCountX > moduleCountY) {
            for (int i = 0; i < moduleCountY; i++) {
                moduleData._hierarchicalDirectories.Add(s_moduleComparer.Compare(left.Modules[i], right.Modules[i])!);
            }
            for (int i = moduleCountY; i < moduleCountX; i++) {
                moduleData._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.Assembly, left: "...", right: "...", reason: "Modules"));
            }
        }
        else if (moduleCountY > moduleCountX) {
            for (int i = 0; i < moduleCountX; i++) {
                moduleData._hierarchicalDirectories.Add(s_moduleComparer.Compare(left.Modules[i], right.Modules[i])!);
            }
            for (int i = moduleCountX; i < moduleCountY; i++) {
                moduleData._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.Assembly, left: "...", right: "...", reason: "Modules"));
            }
        }
        else {
            for (int i = 0; i < moduleCountX; i++) {
                moduleData._hierarchicalDirectories.Add(s_moduleComparer.Compare(left.Modules[i], right.Modules[i])!);
            }
        }
        hierarchicalDirectory._hierarchicalDirectories.Add(moduleData);

        return hierarchicalDirectory;

        void ReportChange(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.Assembly, x, y, reason));
        }

        void ReportExact(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Exact, HierarchicalObjectValueKind.Assembly, x, y, reason));
        }
    }
}
