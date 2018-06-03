﻿using SRNicoNico.Models.NicoNicoViewer;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace SRNicoNico.Views {
    public partial class UserMylistEntry : UserControl {
        public UserMylistEntry() {
            InitializeComponent();
        }

        public void OpenHyperLink(object sender, RequestNavigateEventArgs e) {

            var text = e.Uri.OriginalString;
            if(text.StartsWith("mylist/")) {

                text = "http://www.nicovideo.jp/" + text;
            }
            NicoNicoOpener.Open(text);
        }
        public void InitializeToolTip(object sender, RoutedEventArgs e) {

            var link = sender as Hyperlink;
            if (link.Inlines.First() is Run inline) {

                var text = link.NavigateUri.OriginalString;
                link.ToolTip = text;
            }
        }
    }
}
