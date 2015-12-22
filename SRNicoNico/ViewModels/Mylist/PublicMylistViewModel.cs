using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class PublicMylistViewModel : TabItemViewModel {

        private NicoNicoPublicMylist PublicMylist;

        public PublicMylistViewModel(string url) : base(url.Substring(31)) {

            PublicMylist = new NicoNicoPublicMylist(url);

            App.ViewModelRoot.TabItems.Add(this);
            App.ViewModelRoot.SelectedTab = this;
        }

        public void Initialize() {

            Task.Run(() => {

                PublicMylist.GetMylist();
            });
        }
    }
}
