namespace AsmComp.Core;

internal record FileMetadata(string? Name, byte[] Data) {
    public static FileMetadata Open(string file) {
        return new FileMetadata(file, File.ReadAllBytes(file));
    }
}
