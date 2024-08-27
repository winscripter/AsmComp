namespace AsmComp.Core.Hierarchy;

/// <summary>
/// Represents the type of a hierarchical object.
/// </summary>
public enum HierarchicalObjectKind {
    /// <summary>
    /// Right side has the new object.
    /// </summary>
    Substitute,

    /// <summary>
    /// Left side has the new object.
    /// </summary>
    Remove,

    /// <summary>
    /// Both elements are in the exact order but they're distinct.
    /// </summary>
    Change,

    /// <summary>
    /// Both elements are in the exact order and are the same.
    /// </summary>
    Exact
}
