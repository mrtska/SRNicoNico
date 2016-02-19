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

using SRNicoNico.Models.NicoNicoViewer;
using System.Diagnostics;

namespace SRNicoNico.ViewModels {
    public class UpdateViewModel : TabItemViewModel {


        #region CurrentVersion変更通知プロパティ
        private double _CurrentVersion;

        public double CurrentVersion {
            get { return _CurrentVersion; }
            set { 
                if(_CurrentVersion == value)
                    return;
                _CurrentVersion = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Url変更通知プロパティ
        private string _Url;

        public string Url {
            get { return _Url; }
            set { 
                if(_Url == value)
                    return;
                _Url = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public UpdateViewModel(double ver) {

            CurrentVersion = ver;
        }
        
        public void CheckUpdate() {

            Task.Run(() => {
                string url = null;
                if(UpdateCheck.IsUpdateAvailable(CurrentVersion, ref url)) {

                    Url = url;
                    App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.Contents.Misc.UpdateDialog), this, TransitionMode.Modal));
                }
            });
        }

        public void Update() {

            Task.Run(() => {

                Process.Start("Updater.exe", Process.GetCurrentProcess().Id + " prepare " + Url);
                
                Environment.Exit(0);
            });

        }


    }
}
