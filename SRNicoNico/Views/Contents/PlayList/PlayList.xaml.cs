using System.Windows.Controls;
using System.Windows.Input;

namespace SRNicoNico.Views {
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
