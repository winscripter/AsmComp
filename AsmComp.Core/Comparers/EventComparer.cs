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
internal class EventComparer : IDotNetMetadataComparer {
    private static readonly MethodComparer s_methodComparer = new();

    [CanBeNull]
    public HierarchicalDirectory? Compare(
        [MaybeNull] [CanBeNull] [AllowNull] object x,
        [MaybeNull] [CanBeNull] [AllowNull] object y
    ) {
        Assumes.NotNull(x);
        Assumes.NotNull(y);
        if (x is not EventDefinition && y is not EventDefinition) {
            return null;
        }
        EventDefinition left = (EventDefinition)x;
        EventDefinition right = (EventDefinition)y;
        var hierarchicalDirectory = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Event");
        
        if (left.Name != right.Name) {
            ReportChange("Name", left.Name, right.Name);
        }
        else {
            ReportExact("Name", left.Name, right.Name);
        }

        if (left.AddMethod?.IsAddOn != right.AddMethod?.IsAddOn) {
            ReportChange("AddAccessor", (left.AddMethod?.IsAddOn ?? false).ToString(), (right.AddMethod?.IsAddOn ?? false).ToString());
        }
        else {
            ReportExact("AddAccessor", (left.AddMethod?.IsAddOn ?? false).ToString(), (right.AddMethod?.IsAddOn ?? false).ToString());
        }

        if (left.RemoveMethod?.IsRemoveOn != right.RemoveMethod?.IsRemoveOn) {
            ReportChange("RemoveAccessor", (left.RemoveMethod?.IsRemoveOn ?? false).ToString(), (right.RemoveMethod?.IsRemoveOn ?? false).ToString());
        }
        else {
            ReportExact("RemoveAccessor", (left.RemoveMethod?.IsRemoveOn ?? false).ToString(), (right.RemoveMethod?.IsRemoveOn ?? false).ToString());
        }

        if (left.InvokeMethod?.IsFire != right.InvokeMethod?.IsFire) {
            ReportChange("FireAccessor", (left.InvokeMethod?.IsFire ?? false).ToString(), (right.InvokeMethod?.IsFire ?? false).ToString());
        }
        else {
            ReportExact("FireAccessor", (left.InvokeMethod?.IsFire ?? false).ToString(), (right.InvokeMethod?.IsFire ?? false).ToString());
        }

        if (left.AccessOfAdd()?.Modifier != right.AccessOfAdd()?.Modifier) {
            ReportChange("AccessOfAdd", left.AccessOfAdd()?.Modifier.ToString() ?? "null", right.AccessOfAdd()?.Modifier.ToString() ?? "null");
        }
        else {
            ReportExact("AccessOfAdd", left.AccessOfAdd()?.Modifier.ToString() ?? "null", right.AccessOfAdd()?.Modifier.ToString() ?? "null");
        }

        if (left.AccessOfRemove()?.Modifier != right.AccessOfRemove()?.Modifier) {
            ReportChange("AccessOfRemove", left.AccessOfRemove()?.Modifier.ToString() ?? "null", right.AccessOfRemove()?.Modifier.ToString() ?? "null");
        }
        else {
            ReportExact("AccessOfRemove", left.AccessOfRemove()?.Modifier.ToString() ?? "null", right.AccessOfRemove()?.Modifier.ToString() ?? "null");
        }

        if (left.AccessOfFire()?.Modifier != right.AccessOfFire()?.Modifier) {
            ReportChange("AccessOfFire", left.AccessOfFire()?.Modifier.ToString() ?? "null", right.AccessOfFire()?.Modifier.ToString() ?? "null");
        }
        else {
            ReportExact("AccessOfFire", left.AccessOfFire()?.Modifier.ToString() ?? "null", right.AccessOfFire()?.Modifier.ToString() ?? "null");
        }

        if (left.AddMethod is MethodDefinition amdx && right.AddMethod is MethodDefinition amdy) {
            hierarchicalDirectory._hierarchicalDirectories.Add(s_methodComparer.Compare(amdx, amdy)!);
        }

        if (left.RemoveMethod is MethodDefinition rmdx && right.AddMethod is MethodDefinition rmdy) {
            hierarchicalDirectory._hierarchicalDirectories.Add(s_methodComparer.Compare(rmdx, rmdy)!);
        }

        if (left.InvokeMethod is MethodDefinition fmdx && right.AddMethod is MethodDefinition fmdy) {
            hierarchicalDirectory._hierarchicalDirectories.Add(s_methodComparer.Compare(fmdx, fmdy)!);
        }

        return hierarchicalDirectory;

        void ReportChange(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.Event, left: x, right: y, reason: reason));
        }

        void ReportExact(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.Event, left: x, right: y, reason: reason));
        }
    }
}
