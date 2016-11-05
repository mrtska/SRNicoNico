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
using System.Runtime.InteropServices;
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.Views.Contents.Video {
    /// <summary>
    /// VideoHtml5.xaml の相互作用ロジック
    /// </summary>
    public partial class VideoHtml5 : UserControl, IVideoPlayerView {

        public VideoHtml5() {
            InitializeComponent();

        }
        private InternetSecurityManagerHelper SecurityHelper;

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {

            if(DataContext is VideoViewModel) {
                var vm = (VideoViewModel)DataContext;
                SecurityHelper = new InternetSecurityManagerHelper();
                var ax = browser.ActiveXInstance;
                SecurityHelper.Attach(ax);
                SecurityHelper.ProcessUrlAction += internetSecurityManagerHelper_ProcessUrlAction;
                SecurityHelper.MapUrlToZone += SecurityHelper_MapUrlToZone;
                vm.Initialize(browser);
            }
        }

        private int SecurityHelper_MapUrlToZone(string pwszUrl, out int pdwZone, int dwFlags) {

            pdwZone = (int) WebBrowserAPI.URLZONE.URLZONE_TRUSTED;
            return HRESULT.S_OK;

        }

        private int internetSecurityManagerHelper_ProcessUrlAction(String pwszUrl, int dwAction, out byte pPolicy, int cbPolicy, byte pContext, int cbContext, int dwFlags, int dwReserved) {
            pPolicy = 0;
            if(WebBrowserAPI.URLACTION_SCRIPT_MIN <= dwAction && dwAction <= WebBrowserAPI.URLACTION_SCRIPT_MAX) {

                pPolicy = WebBrowserAPI.URLPOLICY_ALLOW;
                return HRESULT.S_OK;
            } else if((WebBrowserAPI.URLACTION_ACTIVEX_MIN <= dwAction && dwAction <= WebBrowserAPI.URLACTION_ACTIVEX_MAX)) {

                pPolicy = WebBrowserAPI.URLPOLICY_ALLOW;
                return HRESULT.S_OK;
            }
            return WebBrowserAPI.INET_E_DEFAULT_ACTION;
        }

        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e) {

            Console.WriteLine(e.Delta);
        }
    }
}