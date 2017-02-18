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

namespace SRNicoNico.ViewModels {
    public class FollowCommunityEntryViewModel : ViewModel {

        public NicoNicoFollowCommunity Item { get; set; }

        public FollowCommunityEntryViewModel(NicoNicoFollowCommunity item) {

            Item = item;
        }
        public void Open() {

            App.ViewModelRoot.AddWebViewTab(Item.CommunityUrl, false);
        }
    }
}
