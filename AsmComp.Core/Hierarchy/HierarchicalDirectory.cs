using AsmComp.Core.Utilities;

namespace AsmComp.Core.Hierarchy;

/// <summary>
/// Represents a hierarchical directory.
/// </summary>
public class HierarchicalDirectory {
    internal readonly InternalList<HierarchicalObject> _hierarchicalObjects;
    internal readonly InternalList<HierarchicalDirectory> _hierarchicalDirectories;
    internal readonly string _type;

    internal HierarchicalDirectory(InternalList<HierarchicalObject> hierarchicalObjects, InternalList<HierarchicalDirectory> hierarchicalDirectories, string type) {
        _hierarchicalObjects = hierarchicalObjects;
        _hierarchicalDirectories = hierarchicalDirectories;
        _type = type;
    }

    /// <summary>
    /// Checks whether this directory has child directories.
    /// </summary>
    public bool HasDescendantDirectories => _hierarchicalDirectories.Count != 0;

    /// <summary>
    /// Checks whether this directory has child objects.
    /// </summary>
    public bool HasDescendantObjects => _hierarchicalObjects.Count != 0;

    /// <summary>
    /// Represents descendant objects.
    /// </summary>
    public IEnumerable<HierarchicalObject> Objects => _hierarchicalObjects;

    /// <summary>
    /// Represents descendant directories.
    /// </summary>
    public IEnumerable<HierarchicalDirectory> Directories => _hierarchicalDirectories;

    /// <summary>
    /// Represents the type of the directory.
    /// </summary>
    public string Type => _type;
    
    public int CountAll(HierarchicalObjectKind kind) {
        int actualCount = 0;
        foreach (HierarchicalDirectory directory in _hierarchicalDirectories) {
            actualCount += CountInDirectory(directory);
        }

        return actualCount;

        int CountInDirectory(HierarchicalDirectory dir) {
            if (dir == null) {
                return 0;
            }
            int totalCount = 0;
            foreach (HierarchicalDirectory dir2 in dir.Directories) {
                totalCount += CountInDirectory(dir2);
            }
            foreach (HierarchicalObject obj in dir.Objects) {
                if (obj.Kind == kind) {
                    totalCount++;
                }
            }
            return totalCount;
        }
    }
}
