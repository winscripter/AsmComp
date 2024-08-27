using System.Diagnostics.CodeAnalysis;

namespace AsmComp.Core;

/// <summary>
/// Represents an access modifier.
/// </summary>
internal readonly struct Access : IEqualityComparer<Access>
{
    /// <summary>
    /// Represents the type of an access modifier.
    /// </summary>
    public enum AccessModifier
    {
        /// <summary>
        /// The access modifier is public.
        /// </summary>
        Public,

        /// <summary>
        /// The access modifier is protected.
        /// </summary>
        Protected,

        /// <summary>
        /// The access modifier is private.
        /// </summary>
        Private,

        /// <summary>
        /// The access modifier is internal.
        /// </summary>
        Internal,

        /// <summary>
        /// The access modifier is protected internal.
        /// </summary>
        ProtectedInternal,

        /// <summary>
        /// The access modifier is private protected.
        /// </summary>
        PrivateProtected
    }

    /// <summary>
    /// Represents an access modifier similar to IL code.
    /// </summary>
    public enum AccessModifierIL
    {
        /// <summary>
        /// The access modifier is public.
        /// </summary>
        Public,

        /// <summary>
        /// The access modifier is family (protected in C#).
        /// </summary>
        Family,

        /// <summary>
        /// The access modifier is private.
        /// </summary>
        Private,

        /// <summary>
        /// The access modifier is internal (assembly in C#).
        /// </summary>
        Assembly,

        /// <summary>
        /// The access modifier is famorassem (protected internal in C#).
        /// </summary>
        FamORAssem,

        /// <summary>
        /// The access modifier is famandassem (private protected in C#).
        /// </summary>
        FamANDAssem
    }

    private readonly AccessModifier _accessModifier;
    private readonly Lazy<AccessModifierIL> _accessModifierIL;

    internal Access(AccessModifier accessModifier)
    {
        _accessModifier = accessModifier;
        _accessModifierIL = new Lazy<AccessModifierIL>(() => MapILModifier(accessModifier));
    }

    private static AccessModifierIL MapILModifier(AccessModifier accessModifier)
    {
        return accessModifier switch
        {
            AccessModifier.Public => AccessModifierIL.Public,
            AccessModifier.Protected => AccessModifierIL.Family,
            AccessModifier.Internal => AccessModifierIL.Assembly,
            AccessModifier.Private => AccessModifierIL.Private,
            AccessModifier.ProtectedInternal => AccessModifierIL.FamORAssem,
            AccessModifier.PrivateProtected => AccessModifierIL.FamANDAssem,
            _ => throw new InvalidOperationException($"Unexpected modifier {accessModifier}")
        };
    }

    /// <summary>
    /// Specifies the access modifier.
    /// </summary>
    public readonly AccessModifier Modifier => _accessModifier;

    /// <summary>
    /// Returns the access modifier similar to the one used in IL code.
    /// </summary>
    /// <returns><see cref="AccessModifierIL"/></returns>
    public readonly AccessModifierIL GetILModifier() => _accessModifierIL.Value;

    /// <summary>
    /// Represents the access modifier as a string.
    /// </summary>
    /// <returns>A string that represents the access modifier.</returns>
    public readonly override string ToString()
    {
        return Modifier.ToString();
    }

    /// <summary>
    /// Checks whether the access modifier is public.
    /// </summary>
    public readonly bool IsPublic => _accessModifier.Equals(AccessModifier.Public);

    /// <summary>
    /// Checks whether the access modifier is private.
    /// </summary>
    public readonly bool IsPrivate => _accessModifier.Equals(AccessModifier.Private);

    /// <summary>
    /// Checks whether the access modifier is internal.
    /// </summary>
    public readonly bool IsAssembly => _accessModifier.Equals(AccessModifier.Internal);

    /// <summary>
    /// Checks whether the access modifier is internal.
    /// </summary>
    public readonly bool IsInternal => IsAssembly;

    /// <summary>
    /// Checks whether the access modifier is protected.
    /// </summary>
    public readonly bool IsFamily => _accessModifier.Equals(AccessModifier.Protected);

    /// <summary>
    /// Checks whether the access modifier is protected.
    /// </summary>
    public readonly bool IsProtected => IsFamily;

    /// <summary>
    /// Checks whether the access modifier is protected internal.
    /// </summary>
    public readonly bool IsFamORAssem => _accessModifier.Equals(AccessModifier.ProtectedInternal);

    /// <summary>
    /// Checks whether the access modifier is protected internal.
    /// </summary>
    public readonly bool IsProtectedInternal => IsFamORAssem;

    /// <summary>
    /// Checks whether the access modifier is private protected.
    /// </summary>
    public readonly bool IsFamANDAssem => _accessModifier.Equals(AccessModifier.PrivateProtected);

    /// <summary>
    /// Checks whether the access modifier is private protected.
    /// </summary>
    public readonly bool IsPrivateProtected => IsFamANDAssem;

    /// <summary>
    /// Returns an access modifier string similar to the one used in IL code.
    /// </summary>
    /// <returns>Access modifier.</returns>
    public readonly string GetILModifierString() => _accessModifierIL.Value.ToString();

    /// <inheritdoc cref="IEqualityComparer{T}.Equals(T, T)" />
    public bool Equals(Access x, Access y)
    {
        return x.Modifier == y.Modifier;
    }

    /// <inheritdoc cref="IEqualityComparer{T}.GetHashCode(T)" />
    public int GetHashCode([DisallowNull] Access obj)
    {
        return obj.GetHashCode();
    }

    /// <summary>
    /// Checks whether the access modifier may not be compatible with older versions
    /// of .NET.
    /// </summary>
    public readonly bool IsNew => IsFamORAssem || IsFamANDAssem;

    /// <inheritdoc cref="object.GetHashCode()" />
    public override int GetHashCode()
    {
        return this.Modifier.GetHashCode();
    }
}
