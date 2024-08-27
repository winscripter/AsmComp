using Mono.Cecil;

namespace AsmComp.Core.Utilities;

internal static class AccessRelatedExtensions {
    public static Access? AccessOfAdd(this EventDefinition eventDefinition) {
        if (eventDefinition.AddMethod is MethodDefinition methodDefinition) {
            if (!methodDefinition.IsAddOn) {
                return null;
            }
            return methodDefinition.AccessOf();
        }
        return null;
    }

    public static Access? AccessOfRemove(this EventDefinition eventDefinition) {
        if (eventDefinition.RemoveMethod is MethodDefinition methodDefinition) {
            if (!methodDefinition.IsRemoveOn) {
                return null;
            }
            return methodDefinition.AccessOf();
        }
        return null;
    }

    public static Access? AccessOfFire(this EventDefinition eventDefinition) {
        if (eventDefinition.InvokeMethod is MethodDefinition methodDefinition) {
            if (!methodDefinition.IsFire) {
                return null;
            }
            return methodDefinition.AccessOf();
        }
        return null;
    }
}
