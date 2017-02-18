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
using SRNicoNico.Models.NicoNicoViewer;
using System.Windows.Input;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows;

namespace SRNicoNico.ViewModels {
    public class RankingResultEntryViewModel : ViewModel {

        public RankingItem Item { get; private set; }

        public RankingResultEntryViewModel(RankingItem item) {

            Item = item;
        }

        public void Open() {

            NicoNicoOpener.Open(Item.ContentUrl);
        }
    }
}
