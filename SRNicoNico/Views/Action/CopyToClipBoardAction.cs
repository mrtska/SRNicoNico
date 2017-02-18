using SRNicoNico.Models.NicoNicoWrapper;
using System;
using System.Windows;
using System.Windows.Interactivity;

namespace SRNicoNico.Views.Action {

    //Textをクリップボードにコピーするアクション
    public class CopyToClipBoardAction : TriggerAction<DependencyObject> {




        public string Text {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CopyToClipBoardAction), new PropertyMetadata(""));

        protected override void Invoke(object parameter) {

            NicoNicoUtil.CopyToClipboard(Convert.ToString(Text));
        }
    }
}
