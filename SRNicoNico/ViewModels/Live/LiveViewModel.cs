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
using System.Windows.Input;
using System.Threading.Tasks;

namespace SRNicoNico.ViewModels {
    public class LiveViewModel : TabItemViewModel {

        private NicoNicoLive LiveInstance;


        private string LiveUrl;

        public LiveViewModel(string url) : base(url) {

            LiveUrl = url;

            Task.Run(() => Initialize());
        }

        public void Initialize() {

            LiveInstance = new NicoNicoLive(LiveUrl);


            LiveInstance.GetPage();
            
        }


        public void DisposeViewModel() {

            Dispose();
            App.ViewModelRoot.RemoveTabAndLastSet(this);
        }


        public override void KeyDown(KeyEventArgs e) {

            if(e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.W) {

                DisposeViewModel();
            }
        }


    }
}
