using System.Text;
using System.Text.Json;

namespace AsmComp.Core.Hierarchy;

/// <summary>
/// Represents a base for all hierarchical objects used for making assembly diffs.
/// </summary>
public class Hierarchy {
    private static readonly JsonSerializerOptions s_writeIndentedJsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

    private readonly HierarchicalDirectory _rootDirectory;

    /// <summary>
    /// Initializes a new instance of the <see cref="Hierarchy"/> class.
    /// </summary>
    /// <param name="rootDirectory">A root hierarchical directory.</param>
    public Hierarchy(HierarchicalDirectory rootDirectory) {
        _rootDirectory = rootDirectory;
    }

    /// <summary>
    /// Represents the root directory.
    /// </summary>
    public HierarchicalDirectory Root => _rootDirectory;

    /// <summary>
    /// Checks whether the hierarchy has any objects, recursively.
    /// </summary>
    public bool HasHierarchicalObjects {
        get {
            return HasObjects(this.Root);

            static bool HasObjects(HierarchicalDirectory directory) {
                if (directory.HasDescendantObjects) {
                    return true;
                }
                else {
                    foreach (HierarchicalDirectory nestedDirectory in directory.Directories) {
                        if (HasObjects(nestedDirectory)) {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
    }

    /// <summary>
    /// Converts this hierarchy to a JSON string asynchronously, and then formats the result and returns
    /// it.
    /// </summary>
    /// <returns>An awaitable task that returns the JSON representation of this hierarchy.</returns>
    public async Task<string> ToUtf8JsonAndFormatAsync() {
        string result = await ToUtf8JsonAsync();
        using var jDoc = JsonDocument.Parse(result);
        return JsonSerializer.Serialize(jDoc, s_writeIndentedJsonSerializerOptions);
    }

    /// <summary>
    /// Converts this hierarchy to a JSON string asynchronously.
    /// </summary>
    /// <returns>An awaitable task that returns the JSON representation of this hierarchy.</returns>
    public async Task<string> ToUtf8JsonAsync() {
        await using var ms = new MemoryStream();
        await using var utf8Writer = new Utf8JsonWriter(ms);

        WriteDirectory(Root);

        await utf8Writer.FlushAsync();

        return Encoding.UTF8.GetString(ms.ToArray());

        void WriteObject(HierarchicalObject obj) {
            utf8Writer.WriteStartObject();

            utf8Writer.WriteString("type", "object");
            utf8Writer.WriteString("leftValue", obj.Left);
            utf8Writer.WriteString("rightValue", obj.Right);
            utf8Writer.WriteString("kind", obj.Kind.ToString());
            utf8Writer.WriteString("valueKind", obj.ValueKind.ToString());

            utf8Writer.WriteEndObject();
        }

        void WriteDirectory(HierarchicalDirectory dir) {
            if (dir == null) { // I wish I knew why the parameter 'dir' is sometimes null...
                return;
            }

            utf8Writer.WriteStartObject();

            utf8Writer.WriteString("type", "dir");
            utf8Writer.WriteString("dirType", dir.Type ?? "[null]");
            utf8Writer.WritePropertyName("descendants");
            utf8Writer.WriteStartArray();

            foreach (HierarchicalObject obj in dir.Objects) {
                WriteObject(obj);
            }

            foreach (HierarchicalDirectory nestedDir in dir.Directories) {
                WriteDirectory(nestedDir);
            }

            utf8Writer.WriteEndArray();
            utf8Writer.WriteEndObject();
        }
    }
}
