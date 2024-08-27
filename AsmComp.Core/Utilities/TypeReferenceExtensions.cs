using Mono.Cecil;
using System.Text;

namespace AsmComp.Core.Utilities;

internal static class TypeReferenceExtensions {
    public static string GetTypeNameLikeIL(this TypeReference typeReference) {
        var builder = new StringBuilder();
        builder.Append('[');
        builder.Append(typeReference.Resolve().Module.Assembly.Name.Name);
        builder.Append(']');
        builder.Append(typeReference.FullName);
        return builder.ToString();
    }
}
