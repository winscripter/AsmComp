using AsmComp.Core.Comparers;
using AsmComp.Core.Hierarchy;
using AsmComp.Core.Utilities;
using Mono.Cecil;

namespace AsmComp.Core;

/// <summary>Compares assemblies and returns a hierarchy of differences between the two.</summary>
/// <remarks>AsmComp means 'Assembly Comparer', not 'Assembly Composer'.</remarks>
public static class AssemblyComposer {
    private static readonly AssemblyComparer s_assemblyComparer = new();

    private static HierarchicalDirectory CreateFileQuickOverview(FileMetadata x, FileMetadata y) {
        var hierarchicalDirectory = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "FileMetadata");

        if (x.Name != y.Name) {
            ReportChange("Name", x.Name ?? "null", y.Name ?? "null");
        }
        else {
            ReportExact("Name", x.Name ?? "null", y.Name ?? "null");
        }

        if (x.Data != y.Data) {
            ReportChange("Data", x.Data.ToHexString().Truncate(16), x.Data.ToHexString().Truncate(16));
        }
        else {
            ReportExact("Data", x.Data.ToHexString().Truncate(16), x.Data.ToHexString().Truncate(16));
        }

        return hierarchicalDirectory;

        void ReportChange(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.File, x, y, reason));
        }

        void ReportExact(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Exact, HierarchicalObjectValueKind.File, x, y, reason));
        }
    }

    public static HierarchicalDirectory OpenAndCompose(string file1, string file2) {
        FileMetadata fx = FileMetadata.Open(file1);
        FileMetadata fy = FileMetadata.Open(file2);

        var dir = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Root");
        dir._hierarchicalDirectories.Add(CreateFileQuickOverview(fx, fy));

        AssemblyDefinition adx = AssemblyDefinition.ReadAssembly(file1);
        AssemblyDefinition ady = AssemblyDefinition.ReadAssembly(file2);
        dir._hierarchicalDirectories.Add(s_assemblyComparer.Compare(adx, ady)!);

        return dir;
    }
}
