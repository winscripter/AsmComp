using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace AsmComp.Desktop.Windows.Views;
/// <summary>
/// Interaction logic for PickFileView.xaml
/// </summary>
public partial class PickFileView : UserControl {
    public PickFileView() {
        InitializeComponent();
    }

    public string? LeftSelectedAssembly { get; set; } = null;
    public string? RightSelectedAssembly { get; set; } = null;

    public Action<string, string> OnCompareAssemblies { get; set; } = (l, r) => { };
    
    private void OnClickChooseLeft(object sender, RoutedEventArgs e) {
        var dialog = new OpenFileDialog {
            Filter = "DLL|*.dll|EXE|*.exe",
            Title = "Select left .NET assembly",
            Multiselect = false
        };

        bool? success = dialog.ShowDialog();

        if (success == true) {
            LeftSelectedAssembly = dialog.FileName;
            selectedFile1.Text = dialog.SafeFileName;
        }
    }

    private void OnClickChooseRight(object sender, RoutedEventArgs e) {
        var dialog = new OpenFileDialog {
            Filter = "DLL|*.dll|EXE|*.exe",
            Title = "Select right .NET assembly",
            Multiselect = false
        };

        bool? success = dialog.ShowDialog();

        if (success == true) {
            RightSelectedAssembly = dialog.FileName;
            selectedFile2.Text = dialog.SafeFileName;
        }
    }

    private void OnClickCompareAssemblies(object sender, RoutedEventArgs e) {
        if (LeftSelectedAssembly == null) {
            MessageBox.Show("Please select the first file to continue.", "AsmComp", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (RightSelectedAssembly == null) {
            MessageBox.Show("Please select the second file to continue.", "AsmComp", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (!File.Exists(LeftSelectedAssembly)) {
            MessageBox.Show("First input assembly no longer exists.", "AsmComp", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (!File.Exists(RightSelectedAssembly)) {
            MessageBox.Show("Second input assembly no longer exists.", "AsmComp", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        OnCompareAssemblies(LeftSelectedAssembly, RightSelectedAssembly);
    }
}
