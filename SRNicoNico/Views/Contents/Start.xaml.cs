using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Start.xaml の相互作用ロジック
    /// </summary>
    public partial class Start : UserControl {
        public Start() {
            InitializeComponent();
        }

        private void browser_Navigating(object sender, NavigatingCancelEventArgs e) {

            if(e.Uri.OriginalString != "https://mrtska.net/niconicowrapper/releasenote.html") {

                Process.Start(e.Uri.OriginalString);
                e.Cancel = true;
            }
        }
    }
}
