using JetBrains.Annotations;
using System.Diagnostics.CodeAnalysis;
using JBNotNull = JetBrains.Annotations.NotNullAttribute;
using CANotNull = System.Diagnostics.CodeAnalysis.NotNullAttribute;

namespace AsmComp.Core.Utilities;

internal static class ByteArrayExtensions {
    [JBNotNull]
    [return: CANotNull]
    public static string ToHexString([CanBeNull] [AllowNull] this byte[] bytes) {
        if (bytes == null) {
            return "null";
        }
        return string.Join(" ", bytes.Select(b => b.ToString("x")));
    }
}
