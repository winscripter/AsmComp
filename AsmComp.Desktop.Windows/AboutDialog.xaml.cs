using System.Windows;

namespace AsmComp.Desktop.Windows;

public partial class AboutDialog : Window {
    public AboutDialog() {
        InitializeComponent();
    }

    public void CloseAppWhenOKIsClicked(object sender, RoutedEventArgs e) {
        // Misleading name: it won't close "App", it will close window
        Close();
    }
}
