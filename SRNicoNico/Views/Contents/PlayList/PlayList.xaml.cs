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
    /// PlayList.xaml の相互作用ロジック
    /// </summary>
    public partial class PlayList : UserControl {
        public PlayList() {
            InitializeComponent();
        }

        public new void MouseDown(object sender, MouseEventArgs e) {

            var v = Mouse.DirectlyOver;

            //マウスでクリックしたやつがBorderだったらそれはプレイリストのつまみなので
            if(v is Border) {

                e.Handled = true;
            }
        }
    }
}
