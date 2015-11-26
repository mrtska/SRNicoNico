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

namespace SRNicoNico.Views.Contents.Misc {
    /// <summary>
    /// VolumeBar.xaml の相互作用ロジック
    /// </summary>
    public partial class VolumeBar : UserControl {
        public VolumeBar() {
            InitializeComponent();
        }

        private bool IsDrag;

        private void Volume_MouseEnter(object sender, MouseEventArgs e) {

            Volume.IsPopupOpen = true;
        }

        private void Volume_MouseLeave(object sender, MouseEventArgs e) {

            Volume.IsPopupOpen = false;
        }

        private void Volume_MouseMove(object sender, MouseEventArgs e) {


            double x = e.GetPosition(this).X;

            int ans = (int)(x / Volume.ActualWidth * Volume.VideoTime);
            if(ans < 0) {

                ans = 0;
            } else if(ans > Volume.VideoTime) {

                ans = (int)Volume.VideoTime;
            }

            Volume.PopupText = ans + "%";

            if(IsDrag) {

                VideoViewModel vm = (VideoViewModel)DataContext;
                vm.Volume = ans;
            }

        }

        private void Volume_MouseUp(object sender, MouseButtonEventArgs e) {

            IsDrag = false;
        }

        private void Volume_MouseDown(object sender, MouseButtonEventArgs e) {

            IsDrag = true;
        }
    }
}
