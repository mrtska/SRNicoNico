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
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.ViewModels {
    public class UserMylistEntryViewModel : ViewModel {

        public NicoNicoUserMylistEntry Item { get; set; }

        public UserMylistEntryViewModel(NicoNicoUserMylistEntry entry) {

            Item = entry;
        }

        public void Open() {

            NicoNicoOpener.Open(Item.Url);
        }
    }
}
