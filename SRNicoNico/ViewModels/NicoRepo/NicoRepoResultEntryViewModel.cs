using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows.Input;
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.ViewModels {
    public class NicoRepoResultEntryViewModel : ViewModel {

        
        public NicoNicoNicoRepoResultEntry Item { get; set; }

        public NicoRepoResultEntryViewModel(NicoNicoNicoRepoResultEntry item) {

            Item = item;

        }

        public void OpenWebView() {

            App.ViewModelRoot.AddWebViewTab(Item.ContentUrl, true);
        }

        public void CopyUrl() {

            NicoNicoUtil.CopyToClipboard(Item.ContentUrl);
        }

        public void Open() {

            NicoNicoOpener.Open(Item.ContentUrl);
        }


    }
}
