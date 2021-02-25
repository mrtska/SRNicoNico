using System;
using System.Windows;
using Microsoft.Xaml.Behaviors;

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
            DependencyProperty.Register(nameof(Url), typeof(string), typeof(NicoNicoOpenAction), new PropertyMetadata(0));

        protected override void Invoke(object parameter) {
            throw new NotImplementedException();
        }
    }
}
