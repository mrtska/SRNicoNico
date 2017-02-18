using SRNicoNico.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SRNicoNico.Views {
    /// <summary>
    /// SettingsGeneral.xaml の相互作用ロジック
    /// </summary>
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
