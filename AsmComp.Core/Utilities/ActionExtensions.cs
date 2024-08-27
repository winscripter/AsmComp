namespace AsmComp.Core.Utilities;

internal static class ActionExtensions {
    public static void Try(this Action action) {
        try {
            action();
        }
        catch {
        }
    }
}
