using MetroRadiance.UI.Controls;
using SRNicoNico.ViewModels;

namespace SRNicoNico.Views;

public partial class MainWindow : MetroWindow {
    public MainWindow(MainWindowViewModel vm) {
        InitializeComponent();
        DataContext = vm;
    }
}
