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
    public class FollowChannelEntryViewModel : ViewModel {


        public NicoNicoFollowChannel Item { get; set; }

        public FollowChannelEntryViewModel(NicoNicoFollowChannel item) {

            Item = item;
        }

        public void Open() {

            App.ViewModelRoot.AddWebViewTab(Item.ChannelUrl, false);
        }
    }
}
