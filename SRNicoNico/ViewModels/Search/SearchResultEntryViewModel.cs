using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class SearchResultEntryViewModel : ViewModel {

        //検索結果
        public NicoNicoSearchResultEntry Item { get; private set; }

        public SearchResultEntryViewModel(NicoNicoSearchResultEntry node) {

            Item = node;
        }

        public void Open() {

            NicoNicoOpener.Open("http://www.nicovideo.jp/watch/" + Item.Cmsid);
        }
    }
}
