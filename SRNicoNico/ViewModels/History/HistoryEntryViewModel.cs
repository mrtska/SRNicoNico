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
    public class HistoryEntryViewModel : ViewModel {

        public NicoNicoHistoryEntry Item { get; set; }

        public HistoryEntryViewModel(NicoNicoHistoryEntry item) {

            Item = item;
        }

        public void Open() {

            NicoNicoOpener.Open("http://www.nicovideo.jp/watch/" + Item.VideoId);
        }
    }
}
