using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class CommunityViewModel : TabItemViewModel {

        private readonly NicoNicoCommunity Community;

        public CommunityViewModel(string url) {

            Community = new NicoNicoCommunity(url);

            Community.GetCommunity();

            App.ViewModelRoot.AddTabAndSetCurrent(this);
        }





    }
}
