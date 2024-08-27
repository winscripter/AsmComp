using AsmComp.Desktop.Windows.Views;
using System.Windows;

namespace AsmComp.Desktop.Windows;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
        var mainLayout = new MainLayout();
        contentPresenter.Content = mainLayout;
    }
}