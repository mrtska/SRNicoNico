using System.Windows;
using Microsoft.Xaml.Behaviors;
using SRNicoNico.Models;
using Unity;

namespace SRNicoNico.Views.Actions {
    /// <summary>
    /// Viewから各ニコニコのリンクを開くトリガーアクション
    /// </summary>
    public class NicoNicoOpenAction : TriggerAction<DependencyObject> {

        /// <summary>
        /// URL本体
        /// </summary>
        public string Url {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }
        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register(nameof(Url), typeof(string), typeof(NicoNicoOpenAction), new PropertyMetadata(""));

        protected override void Invoke(object parameter) {

            var nnv = App.UnityContainer!.Resolve<INicoNicoViewer>();

            // URLを最適なUIで開く
            nnv.OpenUrl(Url);
        }
    }
}
