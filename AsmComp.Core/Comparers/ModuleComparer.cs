using AsmComp.Core.Hierarchy;
using AsmComp.Core.MEF;
using AsmComp.Core.Utilities;
using JetBrains.Annotations;
using Microsoft;
using Mono.Cecil;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

namespace AsmComp.Core.Comparers;

[Export(typeof(IDotNetMetadataComparer))]
internal class ModuleComparer : IDotNetMetadataComparer {
    private static readonly TypeComparer s_typeComparer = new();

    [CanBeNull]
    public HierarchicalDirectory? Compare(
        [MaybeNull][CanBeNull][AllowNull] object x,
        [MaybeNull][CanBeNull][AllowNull] object y) {
        Assumes.NotNull(x);
        Assumes.NotNull(y);

        if (x is not ModuleDefinition && y is not ModuleDefinition) {
            return null;
        }

        ModuleDefinition left = (ModuleDefinition)x;
        ModuleDefinition right = (ModuleDefinition)y;
        var hierarchicalDirectory = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Module");

        if (left.Architecture != right.Architecture) {
            ReportChange("Architecture", left.Architecture.ToString(), right.Architecture.ToString());
        }
        else {
            ReportExact("Architecture", left.Architecture.ToString(), right.Architecture.ToString());
        }

        var attributeData = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "CustomAttributes");
        int attribCountX = left.CustomAttributes.Count;
        int attribCountY = right.CustomAttributes.Count;
        if (attribCountX > attribCountY) {
            for (int i = 0; i < attribCountY; i++) {
                attributeData._hierarchicalDirectories.Add(CustomAttributeComparer.Compare(left.CustomAttributes[i], right.CustomAttributes[i]));
            }
            for (int i = attribCountY; i < attribCountX; i++) {
                attributeData._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.CustomAttribute, left: "...", right: "...", reason: "CustomAttribute"));
            }
        }
        else if (attribCountY > attribCountX) {
            for (int i = 0; i < attribCountX; i++) {
                attributeData._hierarchicalDirectories.Add(CustomAttributeComparer.Compare(left.CustomAttributes[i], right.CustomAttributes[i]));
            }
            for (int i = attribCountX; i < attribCountY; i++) {
                attributeData._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.CustomAttribute, left: "...", right: "...", reason: "CustomAttribute"));
            }
        }
        else {
            for (int i = 0; i < attribCountX; i++) {
                attributeData._hierarchicalDirectories.Add(CustomAttributeComparer.Compare(left.CustomAttributes[i], right.CustomAttributes[i]));
            }
        }
        hierarchicalDirectory._hierarchicalDirectories.Add(attributeData);

        var assemblyReferences = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "AssemblyReferences");
        int refCountX = left.AssemblyReferences.Count;
        int refCountY = right.AssemblyReferences.Count;
        if (refCountX > refCountY) {
            for (int i = 0; i < refCountY; i++) {
                assemblyReferences._hierarchicalDirectories.Add(CompareAssemblyNameReference(left.AssemblyReferences[i], right.AssemblyReferences[i]));
            }
            for (int i = refCountY; i < refCountX; i++) {
                assemblyReferences._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.Module, left: "...", right: "...", reason: "AssemblyReferences"));
            }
        }
        else if (refCountY > refCountX) {
            for (int i = 0; i < refCountX; i++) {
                assemblyReferences._hierarchicalDirectories.Add(CompareAssemblyNameReference(left.AssemblyReferences[i], right.AssemblyReferences[i]));
            }
            for (int i = refCountX; i < refCountY; i++) {
                assemblyReferences._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.Module, left: "...", right: "...", reason: "AssemblyReferences"));
            }
        }
        else {
            for (int i = 0; i < attribCountX; i++) {
                assemblyReferences._hierarchicalDirectories.Add(CompareAssemblyNameReference(left.AssemblyReferences[i], right.AssemblyReferences[i]));
            }
        }

        hierarchicalDirectory._hierarchicalDirectories.Add(assemblyReferences);
        hierarchicalDirectory._hierarchicalDirectories.Add(attributeData);
        hierarchicalDirectory._hierarchicalDirectories.Add(CompareCharacteristics(left, right));

        if (left.Kind != right.Kind) {
            ReportChange("Kind", left.Kind.ToString(), right.Kind.ToString());
        }
        else {
            ReportExact("Kind", left.Kind.ToString(), right.Kind.ToString());
        }

        if (left.MetadataToken.ToUInt32() != right.MetadataToken.ToUInt32()) {
            ReportChange("MDToken", left.MetadataToken.ToUInt32().ToString(), right.MetadataToken.ToUInt32().ToString());
        }
        else {
            ReportExact("MDToken", left.MetadataToken.ToUInt32().ToString(), right.MetadataToken.ToUInt32().ToString());
        }

        if (left.Mvid != right.Mvid) {
            ReportChange("MVID", left.Mvid.ToString(), right.Mvid.ToString());
        }
        else {
            ReportExact("MVID", left.Mvid.ToString(), right.Mvid.ToString());
        }

        if (left.Name != right.Name) {
            ReportChange("Name", left.Name, right.Name);
        }
        else {
            ReportExact("Name", left.Name, right.Name);
        }

        if (left.Runtime != right.Runtime) {
            ReportChange("Runtime", left.Runtime.ToString(), right.Runtime.ToString());
        }
        else {
            ReportExact("Runtime", left.Runtime.ToString(), right.Runtime.ToString());
        }

        if (left.RuntimeVersion != right.RuntimeVersion) {
            ReportChange("RuntimeVersion", left.RuntimeVersion, right.RuntimeVersion);
        }
        else {
            ReportExact("RuntimeVersion", left.RuntimeVersion, right.RuntimeVersion);
        }

        hierarchicalDirectory._hierarchicalDirectories.Add(CompareTypeSystem(left.TypeSystem, right.TypeSystem));

        var resourceData = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Resources");
        int resourceCountX = left.Resources.Count;
        int resourceCountY = right.Resources.Count;
        if (resourceCountX > resourceCountY) {
            for (int i = 0; i < resourceCountY; i++) {
                resourceData._hierarchicalDirectories.Add(CompareResource(left.Resources[i], right.Resources[i]));
            }
            for (int i = resourceCountY; i < resourceCountX; i++) {
                resourceData._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.Resource, left: "...", right: "...", reason: "Resource"));
            }
        }
        else if (resourceCountY > resourceCountX) {
            for (int i = 0; i < resourceCountX; i++) {
                resourceData._hierarchicalDirectories.Add(CompareResource(left.Resources[i], right.Resources[i]));
            }
            for (int i = resourceCountX; i < resourceCountY; i++) {
                resourceData._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.Resource, left: "...", right: "...", reason: "Resource"));
            }
        }
        else {
            for (int i = 0; i < resourceCountX; i++) {
                resourceData._hierarchicalDirectories.Add(CompareResource(left.Resources[i], right.Resources[i]));
            }
        }
        hierarchicalDirectory._hierarchicalDirectories.Add(resourceData);

        var typeData = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Types");
        int typeCountX = left.Types.Count;
        int typeCountY = right.Types.Count;
        if (typeCountX > typeCountY) {
            for (int i = 0; i < typeCountY; i++) {
                typeData._hierarchicalDirectories.Add(s_typeComparer.Compare(left.Types[i], right.Types[i])!);
            }
            for (int i = typeCountY; i < typeCountX; i++) {
                typeData._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.Type, left: "...", right: "...", reason: "Type"));
            }
        }
        else if (typeCountY > typeCountX) {
            for (int i = 0; i < typeCountX; i++) {
                typeData._hierarchicalDirectories.Add(s_typeComparer.Compare(left.Types[i], right.Types[i])!);
            }
            for (int i = typeCountX; i < typeCountY; i++) {
                typeData._hierarchicalObjects.Add(
                    new HierarchicalObject(
                        HierarchicalObjectKind.Remove, HierarchicalObjectValueKind.Type, left: "...", right: "...", reason: "Type"));
            }
        }
        else {
            for (int i = 0; i < typeCountX; i++) {
                typeData._hierarchicalDirectories.Add(s_typeComparer.Compare(left.Types[i], right.Types[i])!);
            }
        }
        hierarchicalDirectory._hierarchicalDirectories.Add(typeData);

        return hierarchicalDirectory;

        void ReportChange(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.Module, left: x, right: y, reason: reason));
        }

        void ReportExact(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Exact, HierarchicalObjectValueKind.Module, left: x, right: y, reason: reason));
        }
    }

    internal static HierarchicalDirectory CompareAssemblyNameReference(AssemblyNameReference x, AssemblyNameReference y) {
        var hierarchicalDirectory = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "AssemblyNameReference");

        if (x.Culture != y.Culture) {
            ReportChange("Culture", x.Culture, y.Culture);
        }
        else {
            ReportExact("Culture", x.Culture, y.Culture);
        }

        if (x.FullName != y.FullName) {
            ReportChange("FullName", x.FullName, y.FullName);
        }
        else {
            ReportExact("FullName", x.FullName, y.FullName);
        }

        if (x.HashAlgorithm != y.HashAlgorithm) {
            ReportChange("HashAlgorithm", x.HashAlgorithm.ToString(), y.HashAlgorithm.ToString());
        }
        else {
            ReportExact("HashAlgorithm", x.HashAlgorithm.ToString(), y.HashAlgorithm.ToString());
        }

        if (x.HasPublicKey != y.HasPublicKey) {
            ReportChange("HasPublicKey", x.HasPublicKey.ToString(), y.HasPublicKey.ToString());
        }
        else {
            ReportExact("HasPublicKey", x.HasPublicKey.ToString(), y.HasPublicKey.ToString());
        }

        if (x.IsRetargetable != y.IsRetargetable) {
            ReportChange("IsRetargetable", x.IsRetargetable.ToString(), y.IsRetargetable.ToString());
        }
        else {
            ReportExact("IsRetargetable", x.IsRetargetable.ToString(), y.IsRetargetable.ToString());
        }

        if (x.IsSideBySideCompatible != y.IsSideBySideCompatible) {
            ReportChange("IsSideBySideCompatible", x.IsSideBySideCompatible.ToString(), y.IsSideBySideCompatible.ToString());
        }
        else {
            ReportExact("IsSideBySideCompatible", x.IsSideBySideCompatible.ToString(), y.IsSideBySideCompatible.ToString());
        }

        if (x.IsWindowsRuntime != y.IsWindowsRuntime) {
            ReportChange("IsWindowsRuntime", x.IsWindowsRuntime.ToString(), y.IsWindowsRuntime.ToString());
        }
        else {
            ReportExact("IsWindowsRuntime", x.IsWindowsRuntime.ToString(), y.IsWindowsRuntime.ToString());
        }

        if (x.MetadataToken.ToUInt32() != y.MetadataToken.ToUInt32()) {
            ReportChange("MDToken", x.MetadataToken.ToUInt32().ToString(), y.MetadataToken.ToUInt32().ToString());
        }
        else {
            ReportExact("MDToken", x.MetadataToken.ToUInt32().ToString(), y.MetadataToken.ToUInt32().ToString());
        }

        if (x.Name != y.Name) {
            ReportChange("Name", x.Name ?? "null", y.Name ?? "null");
        }
        else {
            ReportExact("Name", x.Name ?? "null", y.Name ?? "null");
        }

        if (x.PublicKey != y.PublicKey) {
            ReportChange("PublicKey", x.PublicKey.ToHexString(), y.PublicKey.ToHexString());
        }
        else {
            ReportExact("PublicKey", x.PublicKey.ToHexString(), y.PublicKey.ToHexString());
        }

        if (x.Version != y.Version) {
            ReportChange("Version", x.Version?.ToString() ?? "null", y.Version?.ToString() ?? "null");
        }
        else {
            ReportExact("Version", x.Version?.ToString() ?? "null", y.Version?.ToString() ?? "null");
        }

        return hierarchicalDirectory;

        void ReportChange(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.Module, left: x, right: y, reason: reason));
        }

        void ReportExact(string reason, string x, string y) {
            hierarchicalDirectory._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Exact, HierarchicalObjectValueKind.Module, left: x, right: y, reason: reason));
        }
    }

    private static HierarchicalDirectory CompareCharacteristics(ModuleDefinition left, ModuleDefinition right) {
        (bool left, bool right) appContainer = (left.Characteristics.HasFlag(ModuleCharacteristics.AppContainer), right.Characteristics.HasFlag(ModuleCharacteristics.AppContainer));
        (bool left, bool right) dynamicBase = (left.Characteristics.HasFlag(ModuleCharacteristics.DynamicBase), right.Characteristics.HasFlag(ModuleCharacteristics.DynamicBase));
        (bool left, bool right) highEntropyVA = (left.Characteristics.HasFlag(ModuleCharacteristics.HighEntropyVA), right.Characteristics.HasFlag(ModuleCharacteristics.HighEntropyVA));
        (bool left, bool right) noSeh = (left.Characteristics.HasFlag(ModuleCharacteristics.NoSEH), right.Characteristics.HasFlag(ModuleCharacteristics.NoSEH));
        (bool left, bool right) nxCompat = (left.Characteristics.HasFlag(ModuleCharacteristics.NXCompat), right.Characteristics.HasFlag(ModuleCharacteristics.NXCompat));
        (bool left, bool right) terminalServerAware = (left.Characteristics.HasFlag(ModuleCharacteristics.TerminalServerAware), right.Characteristics.HasFlag(ModuleCharacteristics.TerminalServerAware));
        var characteristics = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "Characteristics");

        if (appContainer.left != appContainer.right) {
            ReportChange("AppContainer", appContainer.left.ToString(), appContainer.right.ToString());
        }
        else {
            ReportExact("AppContainer", appContainer.left.ToString(), appContainer.right.ToString());
        }

        if (dynamicBase.left != dynamicBase.right) {
            ReportChange("DynamicBase", dynamicBase.left.ToString(), dynamicBase.right.ToString());
        }
        else {
            ReportExact("DynamicBase", dynamicBase.left.ToString(), dynamicBase.right.ToString());
        }

        if (highEntropyVA.left != highEntropyVA.right) {
            ReportChange("HighEntropyVA", highEntropyVA.left.ToString(), highEntropyVA.right.ToString());
        }
        else {
            ReportExact("HighEntropyVA", highEntropyVA.left.ToString(), highEntropyVA.right.ToString());
        }

        if (noSeh.left != noSeh.right) {
            ReportChange("NoSEH", noSeh.left.ToString(), noSeh.right.ToString());
        }
        else {
            ReportExact("NoSEH", noSeh.left.ToString(), noSeh.right.ToString());
        }

        if (nxCompat.left != nxCompat.right) {
            ReportChange("NXCompat", nxCompat.left.ToString(), nxCompat.right.ToString());
        }
        else {
            ReportExact("NXCompat", nxCompat.left.ToString(), nxCompat.right.ToString());
        }

        if (terminalServerAware.left != terminalServerAware.right) {
            ReportChange("AppContainer", terminalServerAware.left.ToString(), terminalServerAware.right.ToString());
        }
        else {
            ReportExact("TerminalServerAware", terminalServerAware.left.ToString(), terminalServerAware.right.ToString());
        }

        return characteristics;

        void ReportChange(string reason, string x, string y) {
            characteristics._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.Module, left: x, right: y, reason: reason));
        }

        void ReportExact(string reason, string x, string y) {
            characteristics._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Exact, HierarchicalObjectValueKind.Module, left: x, right: y, reason: reason));
        }
    }

    private static HierarchicalDirectory CompareTypeSystem(TypeSystem left, TypeSystem right) {
        var typeSys = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "AssemblyNameReference");

        if (left.Boolean.GetTypeNameLikeIL() != right.Boolean.GetTypeNameLikeIL()) {
            ReportChange("Boolean", left.Boolean.GetTypeNameLikeIL(), right.Boolean.GetTypeNameLikeIL());
        }
        else {
            ReportExact("Boolean", left.Boolean.GetTypeNameLikeIL(), right.Boolean.GetTypeNameLikeIL());
        }

        if (left.Byte.GetTypeNameLikeIL() != right.Byte.GetTypeNameLikeIL()) {
            ReportChange("Byte", left.Byte.GetTypeNameLikeIL(), right.Byte.GetTypeNameLikeIL());
        }
        else {
            ReportExact("Byte", left.Byte.GetTypeNameLikeIL(), right.Byte.GetTypeNameLikeIL());
        }

        if (left.Char.GetTypeNameLikeIL() != right.Char.GetTypeNameLikeIL()) {
            ReportChange("Char", left.Char.GetTypeNameLikeIL(), right.Char.GetTypeNameLikeIL());
        }
        else {
            ReportExact("Char", left.Char.GetTypeNameLikeIL(), right.Char.GetTypeNameLikeIL());
        }

        if (left.Double.GetTypeNameLikeIL() != right.Double.GetTypeNameLikeIL()) {
            ReportChange("Double", left.Double.GetTypeNameLikeIL(), right.Double.GetTypeNameLikeIL());
        }
        else {
            ReportExact("Double", left.Double.GetTypeNameLikeIL(), right.Double.GetTypeNameLikeIL());
        }

        if (left.Int16.GetTypeNameLikeIL() != right.Int16.GetTypeNameLikeIL()) {
            ReportChange("Int16", left.Int16.GetTypeNameLikeIL(), right.Int16.GetTypeNameLikeIL());
        }
        else {
            ReportExact("Int16", left.Int16.GetTypeNameLikeIL(), right.Int16.GetTypeNameLikeIL());
        }

        if (left.Int32.GetTypeNameLikeIL() != right.Int32.GetTypeNameLikeIL()) {
            ReportChange("Int32", left.Int32.GetTypeNameLikeIL(), right.Int32.GetTypeNameLikeIL());
        }
        else {
            ReportExact("Int32", left.Int32.GetTypeNameLikeIL(), right.Int32.GetTypeNameLikeIL());
        }

        if (left.Int64.GetTypeNameLikeIL() != right.Int64.GetTypeNameLikeIL()) {
            ReportChange("Int64", left.Int64.GetTypeNameLikeIL(), right.Int64.GetTypeNameLikeIL());
        }
        else {
            ReportExact("Int64", left.Int64.GetTypeNameLikeIL(), right.Int64.GetTypeNameLikeIL());
        }

        if (left.SByte.GetTypeNameLikeIL() != right.SByte.GetTypeNameLikeIL()) {
            ReportChange("SByte", left.SByte.GetTypeNameLikeIL(), right.SByte.GetTypeNameLikeIL());
        }
        else {
            ReportExact("SByte", left.SByte.GetTypeNameLikeIL(), right.SByte.GetTypeNameLikeIL());
        }

        if (left.UInt16.GetTypeNameLikeIL() != right.UInt16.GetTypeNameLikeIL()) {
            ReportChange("UInt16", left.UInt16.GetTypeNameLikeIL(), right.UInt16.GetTypeNameLikeIL());
        }
        else {
            ReportExact("UInt16", left.UInt16.GetTypeNameLikeIL(), right.UInt16.GetTypeNameLikeIL());
        }

        if (left.UInt32.GetTypeNameLikeIL() != right.UInt32.GetTypeNameLikeIL()) {
            ReportChange("UInt32", left.UInt32.GetTypeNameLikeIL(), right.UInt32.GetTypeNameLikeIL());
        }
        else {
            ReportExact("UInt32", left.UInt32.GetTypeNameLikeIL(), right.UInt32.GetTypeNameLikeIL());
        }

        if (left.UInt64.GetTypeNameLikeIL() != right.UInt64.GetTypeNameLikeIL()) {
            ReportChange("UInt64", left.UInt64.GetTypeNameLikeIL(), right.UInt64.GetTypeNameLikeIL());
        }
        else {
            ReportExact("UInt64", left.UInt64.GetTypeNameLikeIL(), right.UInt64.GetTypeNameLikeIL());
        }

        if (left.IntPtr.GetTypeNameLikeIL() != right.IntPtr.GetTypeNameLikeIL()) {
            ReportChange("IntPtr", left.IntPtr.GetTypeNameLikeIL(), right.IntPtr.GetTypeNameLikeIL());
        }
        else {
            ReportExact("IntPtr", left.IntPtr.GetTypeNameLikeIL(), right.IntPtr.GetTypeNameLikeIL());
        }

        if (left.Object.GetTypeNameLikeIL() != right.Object.GetTypeNameLikeIL()) {
            ReportChange("Object", left.Object.GetTypeNameLikeIL(), right.Object.GetTypeNameLikeIL());
        }
        else {
            ReportExact("Object", left.Object.GetTypeNameLikeIL(), right.Object.GetTypeNameLikeIL());
        }

        if (left.Single.GetTypeNameLikeIL() != right.Single.GetTypeNameLikeIL()) {
            ReportChange("Single", left.Single.GetTypeNameLikeIL(), right.Single.GetTypeNameLikeIL());
        }
        else {
            ReportExact("Single", left.Single.GetTypeNameLikeIL(), right.Single.GetTypeNameLikeIL());
        }

        if (left.String.GetTypeNameLikeIL() != right.String.GetTypeNameLikeIL()) {
            ReportChange("String", left.String.GetTypeNameLikeIL(), right.String.GetTypeNameLikeIL());
        }
        else {
            ReportExact("String", left.String.GetTypeNameLikeIL(), right.String.GetTypeNameLikeIL());
        }

        if (left.TypedReference.GetTypeNameLikeIL() != right.TypedReference.GetTypeNameLikeIL()) {
            ReportChange("TypedRef", left.TypedReference.GetTypeNameLikeIL(), right.TypedReference.GetTypeNameLikeIL());
        }
        else {
            ReportExact("TypedRef", left.TypedReference.GetTypeNameLikeIL(), right.TypedReference.GetTypeNameLikeIL());
        }

        if (left.UIntPtr.GetTypeNameLikeIL() != right.UIntPtr.GetTypeNameLikeIL()) {
            ReportChange("UIntPtr", left.UIntPtr.GetTypeNameLikeIL(), right.UIntPtr.GetTypeNameLikeIL());
        }
        else {
            ReportExact("UIntPtr", left.UIntPtr.GetTypeNameLikeIL(), right.UIntPtr.GetTypeNameLikeIL());
        }

        if (left.Void.GetTypeNameLikeIL() != right.Void.GetTypeNameLikeIL()) {
            ReportChange("Void", left.Void.GetTypeNameLikeIL(), right.Void.GetTypeNameLikeIL());
        }
        else {
            ReportExact("Void", left.Void.GetTypeNameLikeIL(), right.Void.GetTypeNameLikeIL());
        }

        return typeSys;

        void ReportChange(string reason, string x, string y) {
            typeSys._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.Module, left: x, right: y, reason: reason));
        }

        void ReportExact(string reason, string x, string y) {
            typeSys._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Exact, HierarchicalObjectValueKind.Module, left: x, right: y, reason: reason));
        }
    }

    private static HierarchicalDirectory CompareResource(Resource left, Resource right) {
        var resourceComparisonHierarchy = new HierarchicalDirectory(hierarchicalObjects: new(), hierarchicalDirectories: new(), type: "AssemblyNameReference");

        if (left.Attributes != right.Attributes) {
            ReportChange("ManifestResourceAttributes", left.Attributes.ToString(), right.Attributes.ToString());
        }
        else {
            ReportExact("ManifestResourceAttributes", left.Attributes.ToString(), right.Attributes.ToString());
        }

        if (left.IsPrivate != right.IsPrivate) {
            ReportChange("IsPrivate", left.IsPrivate.ToString(), right.IsPrivate.ToString());
        }
        else {
            ReportExact("IsPrivate", left.IsPrivate.ToString(), right.IsPrivate.ToString());
        }

        if (left.IsPublic != right.IsPublic) {
            ReportChange("IsPublic", left.IsPublic.ToString(), right.IsPublic.ToString());
        }
        else {
            ReportExact("IsPublic", left.IsPublic.ToString(), right.IsPublic.ToString());
        }

        if (left.Name != right.Name) {
            ReportChange("Name", left.Name, right.Name);
        }
        else {
            ReportExact("Name", left.Name, right.Name);
        }

        if (left.ResourceType != right.ResourceType) {
            ReportChange("ResourceType", left.ResourceType.ToString(), right.ResourceType.ToString());
        }
        else {
            ReportExact("ResourceType", left.ResourceType.ToString(), right.ResourceType.ToString());
        }

        EmbeddedResource? embeddedResourceX = left as EmbeddedResource;
        EmbeddedResource? embeddedResourceY = right as EmbeddedResource;
        switch ((embeddedResourceX != null, embeddedResourceY != null)) {
            case (false, false):
                ReportExact("EmbeddedResource", "[none]", "[none]");
                break;
            case (true, false):
                ReportChange("EmbeddedResource", embeddedResourceX!.GetResourceData().ToHexString().Truncate(256), "[none]");
                break;
            case (false, true):
                ReportChange("EmbeddedResource", "[none]", embeddedResourceY!.GetResourceData().ToHexString().Truncate(256));
                break;
            case (true, true):
                string resourceDataLeft = embeddedResourceX!.GetResourceData().ToHexString().Truncate(256);
                string resourceDataRight = embeddedResourceY!.GetResourceData().ToHexString().Truncate(256);
                if (resourceDataLeft != resourceDataRight) {
                    ReportChange("EmbeddedResource", resourceDataLeft, resourceDataRight);
                }
                else {
                    ReportExact("EmbeddedResource", resourceDataLeft, resourceDataRight);
                }
                break;
        }

        LinkedResource? linkedResourceX = left as LinkedResource;
        LinkedResource? linkedResourceY = right as LinkedResource;
        switch ((linkedResourceX != null, linkedResourceY != null)) {
            case (false, false):
                ReportExact("LinkedResource", "[none]", "[none]");
                break;
            case (true, false):
                ReportChange("LinkedResource", $"[{linkedResourceX!.File}] {linkedResourceX.Hash.ToHexString()}", "[none]");
                break;
            case (false, true):
                ReportChange("LinkedResource", "[none]", $"[{linkedResourceY!.File}] {linkedResourceY.Hash.ToHexString()}");
                break;
            case (true, true):
                string leftAsString = $"[{linkedResourceX!.File}] {linkedResourceX!.Hash.ToHexString()}";
                string rightAsString = $"[{linkedResourceY!.File}] {linkedResourceY!.Hash.ToHexString()}";
                if (leftAsString != rightAsString) {
                    ReportChange("LinkedResource", leftAsString, rightAsString);
                }
                else {
                    ReportExact("LinkedResource", leftAsString, rightAsString);
                }
                break;
        }

        AssemblyLinkedResource? assemblyLinkedResourceX = left as AssemblyLinkedResource;
        AssemblyLinkedResource? assemblyLinkedResourceY = right as AssemblyLinkedResource;
        switch ((assemblyLinkedResourceX != null, assemblyLinkedResourceY != null)) {
            case (false, false):
                ReportExact("AssemblyLinkedResource", "[none]", "[none]");
                break;
            case (true, false):
                ReportChange("AssemblyLinkedResource", $"[{assemblyLinkedResourceX!.Name}] {assemblyLinkedResourceX.Assembly.Name}", "[none]");
                break;
            case (false, true):
                ReportChange("AssemblyLinkedResource", "[none]", $"[{assemblyLinkedResourceY!.Name}] {assemblyLinkedResourceY!.Assembly.Name}");
                break;
            case (true, true):
                string resX = $"[{assemblyLinkedResourceX!.Name}] {assemblyLinkedResourceX!.Name}";
                string resY = $"[{assemblyLinkedResourceY!.Name}] {assemblyLinkedResourceY!.Name}";

                if (resX != resY) {
                    ReportChange("AssemblyLinkedResource", resX, resY);
                }
                else {
                    ReportExact("AssemblyLinkedResource", resX, resY);
                }
                break;
        }

        return resourceComparisonHierarchy;

        void ReportChange(string reason, string x, string y) {
            resourceComparisonHierarchy._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Change, HierarchicalObjectValueKind.Module, left: x, right: y, reason: reason));
        }

        void ReportExact(string reason, string x, string y) {
            resourceComparisonHierarchy._hierarchicalObjects.Add(
                new HierarchicalObject(
                    HierarchicalObjectKind.Exact, HierarchicalObjectValueKind.Module, left: x, right: y, reason: reason));
        }
    }
}
