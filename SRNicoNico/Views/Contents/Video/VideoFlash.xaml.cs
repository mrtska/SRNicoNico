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
using CefSharp;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Threading;

namespace SRNicoNico.Views.Contents.Video {
    /// <summary>
    /// VideoFlash.xaml の相互作用ロジック
    /// </summary>
    public partial class VideoFlash : UserControl {
        public VideoFlash() {
            InitializeComponent();

            
            
        }



        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {

            //決め打ち
            var vm = (VideoViewModel)DataContext;
            Cef.RegisterJsObject("external", vm.Handler);
        }


        private void Cef_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e) {

            if((bool)e.NewValue) {


                var vm = (VideoViewModel)DataContext;

                vm.Handler.PreInitialize(Cef.WebBrowser);


                var history = NicoNicoWrapperMain.Session.HttpHandler.CookieContainer.GetCookies(new Uri("http://nicovideo.jp/"))["nicohistory"];
                while(history == null) {

                    history = NicoNicoWrapperMain.Session.HttpHandler.CookieContainer.GetCookies(new Uri("http://nicovideo.jp/"))["nicohistory"];
                    Thread.Sleep(10);
                }


                var cefcookie = new Cookie();
                cefcookie.Domain = ".nicovideo.jp";
                cefcookie.Name = "nicohistory";
                cefcookie.Value = history.Value;
                cefcookie.Expires = history.Expires;

                //Chromium側にセッションを使わせる
                var c = CefSharp.Cef.GetGlobalCookieManager().SetCookieAsync("http://.nicovideo.jp/", cefcookie).Result;
            }
        }
    }
}
