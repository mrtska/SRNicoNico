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

using SRNicoNico.Models;

namespace SRNicoNico.ViewModels {
    public class StatusBarViewModel : ViewModel {

        #region Status変更通知プロパティ
        private string _Status;

        public string Status {
            get { return _Status; }
            set { 
                if(_Status == value)
                    return;
                _Status = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Time変更通知プロパティ
        private string _Time;

        public string Time {
            get { return _Time; }
            set { 
                if(_Time == value)
                    return;
                _Time = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public Timer RefreshTimer;

        public void StartRefreshTimer() {

            RefreshTimer = new Timer(new TimerCallback(o => {

                App.ViewModelRoot.NotifyLive.Refresh();
                Console.WriteLine("Refresh Live");
            }), null, App.ViewModelRoot.Config.Live.RefreshInterval, App.ViewModelRoot.Config.Live.RefreshInterval);
        }

        public void TimerStart() {

            Task.Run(() => {

                for(;;) {

                    Time = DateTime.Now.ToString();

                    Thread.Sleep(1000);

                }
            });
        }

    }
}
