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
    public class FollowUserEntryViewModel : ViewModel {

        public NicoNicoFollowUser Item { get; set; }

        public FollowUserEntryViewModel(NicoNicoFollowUser item) {

            Item = item;
        }
        public void Open() {

            NicoNicoOpener.Open(Item.UserPageUrl);
        }
    }
}
