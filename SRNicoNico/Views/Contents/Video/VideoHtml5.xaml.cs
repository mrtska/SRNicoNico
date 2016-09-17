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
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Markup;
using System.IO;
using SRNicoNico.ViewModels;
using System.Windows.Interop;
using AxShockwaveFlashObjects;
using Flash.External;
using System.Runtime.InteropServices;

namespace SRNicoNico.Views.Contents.Video {
    /// <summary>
    /// VideoHtml5.xaml の相互作用ロジック
    /// </summary>
    public partial class VideoHtml5 : UserControl {

        private const int SET_FEATURE_ON_PROCESS = 0x00000002;

        public VideoHtml5() {
            InitializeComponent();

        }



        [DllImport("urlmon.dll")]
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Error)]
        static extern int CoInternetSetFeatureEnabled(int FeatureEntry, [MarshalAs(UnmanagedType.U4)] int dwFlags, bool fEnable);


        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {

            if(DataContext is VideoViewModel) {

                var vm = (VideoViewModel) DataContext;
                
                vm.Initialize(browser);
            }   
        }


        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e) {

            Console.WriteLine(e.Delta);
        }
    }
}
