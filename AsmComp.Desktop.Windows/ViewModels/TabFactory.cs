using AsmComp.Core;
using AsmComp.Core.Hierarchy;
using AsmComp.Desktop.Windows.Views;
using AvalonDock.Layout;
using System.Windows.Controls;

namespace AsmComp.Desktop.Windows.ViewModels;

internal static class TabFactory {
    public static LayoutDocument GetPickFileView(string name, Action<string, string> compareAssemblies) {
        var pickFileView = new PickFileView();
        pickFileView.OnCompareAssemblies = compareAssemblies;

        var frame = new ContentPresenter() {
            Content = pickFileView
        };

        var document = new LayoutDocument {
            CanClose = true,
            CanFloat = true,
            Title = name,
            Content = frame
        };

        return document;
    }

    public static LayoutDocument CompareSideBySideAndGetTab(string name, string left, string right) {
        var leftView = new SideBySideComparison();

        HierarchicalDirectory resultDir = AssemblyComposer.OpenAndCompose(left, right);
        Composer.Compose(resultDir, true, leftView.treeView1);
        Composer.Compose(resultDir, false, leftView.treeView2);

        var frame = new ContentPresenter() {
            Content = leftView
        };

        var document = new LayoutDocument {
            CanClose = true,
            CanFloat = true,
            Title = name,
            Content = frame
        };

        return document;
    }
}
