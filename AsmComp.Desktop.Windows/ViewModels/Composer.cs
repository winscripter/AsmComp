using AsmComp.Core.Hierarchy;
using Humanizer;
using System.Windows.Controls;
using System.Windows.Media;

namespace AsmComp.Desktop.Windows.ViewModels;

internal static class Composer {
    private const int MaxDepth = 128;

    public static void Compose(HierarchicalDirectory hierarchicalDirectory, bool isLeft, TreeView treeView) {
        var root = new TreeViewItem {
            Header = "Root"
        };
        AddToPart(hierarchicalDirectory, isLeft, root);
        treeView.Items.Add(root);
    }

    private static void AddToPart(HierarchicalDirectory dir, bool isLeft, TreeViewItem item) {
        int depth = 0;
        foreach (HierarchicalDirectory directory in dir.Directories) {
            if (depth++ >= MaxDepth) {
                break;
            }
            if (directory == null) {
                continue;
            }
            int changeCount = directory.CountAll(HierarchicalObjectKind.Change);
            int exactCount = directory.CountAll(HierarchicalObjectKind.Exact);
            int substituteCount = directory.CountAll(HierarchicalObjectKind.Substitute);
            var newItem = new TreeViewItem() {
                Header = GetHeader(directory, changeCount, exactCount, substituteCount)
            };
            if ((changeCount + substituteCount) != 0) {
                newItem.Background = Brushes.Yellow;
            }
            AddToPart(directory, isLeft, newItem);
            item.Items.Add(newItem);
        }

        foreach (HierarchicalObject obj in dir.Objects) {
            if (obj == null) {
                continue;
            }
            bool change = obj.Kind == HierarchicalObjectKind.Change;
            bool substitute = obj.Kind == HierarchicalObjectKind.Substitute;
            bool remove = obj.Kind == HierarchicalObjectKind.Remove;
            SolidColorBrush brush =
                isLeft
                ? (
                    change ? Brushes.Yellow : substitute ? Brushes.IndianRed : remove ? Brushes.LimeGreen : Brushes.White
                )
                : (
                    change ? Brushes.Yellow : substitute ? Brushes.LimeGreen : remove ? Brushes.IndianRed : Brushes.White
                );
            var treeObj = new TreeViewItem {
                Header = $"{obj.Reason}: L = {obj.Left.Truncate(16)} R = {obj.Right.Truncate(16)}"
            };
            treeObj.Background = brush;
            item.Items.Add(treeObj);
        }
    }

    private static string GetHeader(HierarchicalDirectory hierarchicalDirectory, int changeCount, int exactCount, int substituteCount) {
        if (hierarchicalDirectory == null) {
            return string.Empty;
        }
        if (hierarchicalDirectory?.Type == null) {
            return string.Empty;
        }

        return $"{hierarchicalDirectory.Type} [{changeCount}|{exactCount}|{substituteCount}]";
    }
}
