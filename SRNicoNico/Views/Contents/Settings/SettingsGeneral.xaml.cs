using System.Windows;
using System.Windows.Controls;

namespace SRNicoNico.Views {
    public partial class SettingsGeneral : UserControl {
        public SettingsGeneral() {
            InitializeComponent();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e) {

            if(sender is RadioButton) {
                var radio = (RadioButton)sender;
                var str = (string) radio.Content;
                switch(str) {
                    case "Blue":
                        App.ViewModelRoot.ThemeSelector.ChangeTheme(Service.ThemeColors.Blue);
                        break;
                    case "Orange":
                        App.ViewModelRoot.ThemeSelector.ChangeTheme(Service.ThemeColors.Orange);
                        break;
                    case "Purple":
                        App.ViewModelRoot.ThemeSelector.ChangeTheme(Service.ThemeColors.Purple);
                        break;
                }
                Models.NicoNicoViewer.Settings.Instance.ThemeColor = str;
            }
        }
    }
}
