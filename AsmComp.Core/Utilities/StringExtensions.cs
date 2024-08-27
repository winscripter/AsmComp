namespace AsmComp.Core.Utilities;

internal static class StringExtensions {
    // Source: https://stackoverflow.com/a/2776689/21072788 [CC BY-SA 4.0]
    public static string Truncate(this string value, int maxLength) {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..maxLength];
    }
}
