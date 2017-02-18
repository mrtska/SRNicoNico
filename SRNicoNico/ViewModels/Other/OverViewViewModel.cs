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

namespace SRNicoNico.ViewModels {
    public class OverViewViewModel : TabItemViewModel {

        private TabItemViewModel Owner;

        public OverViewViewModel(TabItemViewModel vm) : base("このソフトウェアについて") {

            Owner = vm;
        }

        public async void CheckUpdate() {

            if(await UpdateChecker.IsUpdateAvailable()) {

                App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.UpdateFoundView), new UpdaterViewModel(), TransitionMode.Modal));
            } else {

                Owner.Status = "アップデートはありません。";
            }
        }
    }
}
