using System.Windows;
using System.Windows.Controls;

namespace SRNicoNico.Views {
    public partial class SettingsRanking : UserControl {
        public SettingsRanking() {
            InitializeComponent();
        }

        public void Refresh(object sender, RoutedEventArgs e) {

            App.ViewModelRoot.Ranking.Refresh();
        }
    }
}
