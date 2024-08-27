// This is a barebone sample application.
// Doesn't even have a command-line argument check.

using AsmComp.Core;
using AsmComp.Core.Hierarchy;

HierarchicalDirectory directory = AssemblyComposer.OpenAndCompose(args[0], args[1]);
Hierarchy hierarchy = new(directory);

Console.WriteLine(hierarchy.Root.CountAll(HierarchicalObjectKind.Substitute));
Console.WriteLine(hierarchy.Root.CountAll(HierarchicalObjectKind.Remove));
Console.WriteLine(hierarchy.Root.CountAll(HierarchicalObjectKind.Change));
Console.WriteLine(hierarchy.Root.CountAll(HierarchicalObjectKind.Exact));

string json = await hierarchy.ToUtf8JsonAndFormatAsync();
Console.WriteLine(json);
