using Mono.Cecil;

namespace AsmComp.Core.Utilities;

internal delegate bool FlagComparerCallback(MethodAttributes attributes);

internal static class MethodDefinitionExtensions {
    private static readonly (FlagComparerCallback, Access)[] s_flagComparers = {
        ((attr) => {
            return attr.HasFlag(MethodAttributes.FamORAssem);
        }, new Access(Access.AccessModifier.ProtectedInternal)),

        ((attr) => {
            return attr.HasFlag(MethodAttributes.Public);
        }, new Access(Access.AccessModifier.Public)),

        ((attr) => {
            return attr.HasFlag(MethodAttributes.Assembly);
        }, new Access(Access.AccessModifier.Internal)),

        ((attr) => {
            return attr.HasFlag(MethodAttributes.Private);
        }, new Access(Access.AccessModifier.Private)),

        ((attr) => {
            return attr.HasFlag(MethodAttributes.Family);
        }, new Access(Access.AccessModifier.Protected)),

        ((attr) => {
            return attr.HasFlag(MethodAttributes.FamANDAssem);
        }, new Access(Access.AccessModifier.PrivateProtected))
    };

    public static Access AccessOf(this MethodDefinition method) {
        return s_flagComparers.First(fc => fc.Item1(method.Attributes)).Item2;
    }
}
