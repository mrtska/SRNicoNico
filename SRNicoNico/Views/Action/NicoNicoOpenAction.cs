using SRNicoNico.Models.NicoNicoViewer;
using System.Windows;
using System.Windows.Interactivity;

namespace SRNicoNico.Views.Action {

    //ニコニコを開くアクション
    public class NicoNicoOpenAction : TriggerAction<DependencyObject> {

        public string Url {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        public static readonly DependencyProperty UrlProperty = DependencyProperty.Register("Url", typeof(string), typeof(NicoNicoOpenAction), new PropertyMetadata(""));

        protected override void Invoke(object parameter) {

            NicoNicoOpener.Open(Url);
        }
    }
}
