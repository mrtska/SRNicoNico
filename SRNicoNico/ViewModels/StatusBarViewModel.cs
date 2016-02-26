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

        public void TimerStart() {

            Task.Run(() => {

                Timer timer = new Timer(new TimerCallback(o => {

                    App.ViewModelRoot.NotifyLive.Reflesh();
                    Console.WriteLine("Reflesh Live");
                }), null, 50000, 50000);

                for(;;) {

                    Time = DateTime.Now.ToString();

                    Thread.Sleep(1000);

                }
            });
        }
    }
}
