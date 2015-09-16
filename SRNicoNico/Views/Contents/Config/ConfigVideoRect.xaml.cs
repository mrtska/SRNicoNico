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

namespace SRNicoNico.Views.Contents.Config {
    /// <summary>
    /// ConfigVideoRect.xaml の相互作用ロジック
    /// </summary>
    public partial class ConfigVideoRect : UserControl {




        public string Placement {
            get { return (string)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Placement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(string), typeof(ConfigVideoRect), new PropertyMetadata(""));







        public ConfigVideoRect() {
            InitializeComponent();
        }
    }
}
