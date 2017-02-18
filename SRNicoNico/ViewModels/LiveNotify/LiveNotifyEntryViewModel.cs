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
using System.Windows.Input;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class LiveNotifyEntryViewModel : TabItemViewModel {

        public NicoNicoLiveNotifyEntry Item { get; set; }

        public LiveNotifyEntryViewModel(NicoNicoLiveNotifyEntry item) : base("生放送通知") {

            Item = item;
        }

    }
}
