using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using System;
using System.Windows;
using System.Windows.Interactivity;

namespace SRNicoNico.Views.Action {

    //UrlをWebViewで開くアクション
    public class OpenWebViewAction : TriggerAction<DependencyObject> {




        public string Url {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register("Url", typeof(string), typeof(OpenWebViewAction), new PropertyMetadata(""));

        protected override void Invoke(object parameter) {

            //Viewerで開けるURLをWebViewで開こうとした時は
            //Viewerで開くフラグが経っているとアレな動作になるので
            if(NicoNicoOpener.GetType(Url) == NicoNicoUrlType.Other) {

                App.ViewModelRoot.AddWebViewTab(Url, false);
            } else {

                App.ViewModelRoot.AddWebViewTab(Url, true);
            }

        }
    }
}
