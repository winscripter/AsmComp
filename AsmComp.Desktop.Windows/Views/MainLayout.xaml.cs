using AsmComp.Desktop.Windows.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace AsmComp.Desktop.Windows.Views;

public partial class MainLayout : UserControl {
    public MainLayout() {
        InitializeComponent();
        Tabs.Children.Add(TabFactory.GetPickFileView("Compare Assemblies", (l, r) => {
            var tab = TabFactory.CompareSideBySideAndGetTab("Comparison", l, r);
            Tabs.Children.Add(tab);
        }));
    }

    public void FileExit(object sender, RoutedEventArgs e) {
        Environment.Exit(0);
    }

    private void BringAboutWindow(object sender, RoutedEventArgs e) {
        var window = new AboutDialog();
        window.ShowDialog();
    }
}
