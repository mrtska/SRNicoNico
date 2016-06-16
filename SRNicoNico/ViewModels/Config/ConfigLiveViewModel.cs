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

using SRNicoNico.Models;
using System.Windows;
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.ViewModels {
    public class ConfigLiveViewModel : ConfigViewModelBase {



        #region RefreshInterval変更通知プロパティ

        public int RefreshInterval {
            get { return Settings.Instance.RefreshInterval; }
            set { 
                if(Settings.Instance.RefreshInterval == value)
                    return;
                Settings.Instance.RefreshInterval = value;
                App.ViewModelRoot.StatusBar.RefreshTimer.Change(value, value);
                RaisePropertyChanged();
            }
        }
        #endregion


        public ConfigLiveViewModel() : base("生放送") {

        }

    }
}
