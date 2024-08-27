namespace AsmComp.Core.Hierarchy;

/// <summary>
/// Represents the type of a value stored in the hierarchical object.
/// </summary>
public enum HierarchicalObjectValueKind {
    /// <summary>
    /// An enumeration.
    /// </summary>
    Enum,

    /// <summary>
    /// A member of an enumeration.
    /// </summary>
    EnumMember,

    /// <summary>
    /// A custom attribute.
    /// </summary>
    CustomAttribute,

    /// <summary>
    /// A multicast delegate.
    /// </summary>
    Delegate,

    /// <summary>
    /// An event (including its accessors and body.)
    /// </summary>
    Event,

    /// <summary>
    /// A field (including its name, flags, and initial value if present.)
    /// </summary>
    Field,

    /// <summary>
    /// An interface.
    /// </summary>
    Interface,

    /// <summary>
    /// An interface member.
    /// </summary>
    InterfaceMember,

    /// <summary>
    /// A module.
    /// </summary>
    Module,

    /// <summary>
    /// A property (including its accessors and body.)
    /// </summary>
    Property,

    /// <summary>
    /// A record.
    /// </summary>
    Record,

    /// <summary>
    /// A type.
    /// </summary>
    Type,

    /// <summary>
    /// A parameter.
    /// </summary>
    Parameter,

    /// <summary>
    /// A method.
    /// </summary>
    Method,

    /// <summary>
    /// An instruction
    /// </summary>
    Instruction,
    Variable,
    Resource,
    Assembly,
    File
}
