namespace AsmComp.Core.Hierarchy;

/// <summary>
/// Represents an object part of the diff hierarchy.
/// </summary>
public class HierarchicalObject {
    /// <summary>
    /// Represents the type of this hierarchical object.
    /// </summary>
    public HierarchicalObjectKind Kind { get; init; }

    /// <summary>
    /// Represents the type of the value of this hierarchical object.
    /// </summary>
    public HierarchicalObjectValueKind ValueKind { get; init; }

    /// <summary>
    /// Specifies the string representation of the left value.
    /// </summary>
    public string Left { get; init; }

    /// <summary>
    /// Specifies the string representation of the right value.
    /// </summary>
    public string Right { get; init; }

    /// <summary>
    /// Specifies what <see cref="Left"/> and <see cref="Right"/> even are.
    /// </summary>
    public string Reason { get; init; }

    internal HierarchicalObject(HierarchicalObjectKind kind, HierarchicalObjectValueKind valueKind, string left, string right, string reason) {
        Kind = kind;
        ValueKind = valueKind;
        Left = left;
        Right = right;
        Reason = reason;
    }
}
