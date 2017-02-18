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


namespace SRNicoNico.ViewModels {
    public class StartViewModel : TabItemViewModel {

        public double CurrentVersion {
            get { return App.ViewModelRoot.CurrentVersion; }
        }

        public StartViewModel() : base("スタート") {

        }

        public override bool CanShowHelp() {
            return true;
        }

        public override void ShowHelpView(InteractionMessenger Messenger) {

            Messenger.Raise(new TransitionMessage(typeof(Views.StartHelpView), this, TransitionMode.NewOrActive));
        }

    }
}
