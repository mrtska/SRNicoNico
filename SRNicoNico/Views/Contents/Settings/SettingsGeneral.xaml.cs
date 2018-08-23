using System.Windows;
using System.Windows.Controls;

namespace SRNicoNico.Views {
    public partial class SettingsGeneral : UserControl {
        public SettingsGeneral() {
            InitializeComponent();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e) {

            if (sender is RadioButton radio) {
                var str = (string)radio.Content;
                switch (str) {
                    case "Blue":
                        App.ViewModelRoot.ThemeSelector.ChangeAccent(Service.EnumAccents.Blue);
                        break;
                    case "Orange":
                        App.ViewModelRoot.ThemeSelector.ChangeAccent(Service.EnumAccents.Orange);
                        break;
                    case "Purple":
                        App.ViewModelRoot.ThemeSelector.ChangeAccent(Service.EnumAccents.Purple);
                        break;
                }
                Models.NicoNicoViewer.Settings.Instance.ThemeColor = str;
            }
        }

        private void RadioButton_Click_Theme(object sender, RoutedEventArgs e) {

            if (sender is RadioButton radio) {
                var str = (string)radio.Content;
                switch (str) {
                    case "Dark":
                        App.ViewModelRoot.ThemeSelector.ChangeTheme(Service.EnumThemes.Dark);
                        break;
                    case "Light":
                        App.ViewModelRoot.ThemeSelector.ChangeTheme(Service.EnumThemes.Light);
                        break;
                }
                Models.NicoNicoViewer.Settings.Instance.Theme = str;
            }
        }
    }
}
