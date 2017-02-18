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
    public class SettingsLiveViewModel : TabItemViewModel {

        #region RefreshInterval変更通知プロパティ

        public int RefreshInterval {
            get { return Settings.Instance.RefreshInterval / 60000; }
            set {
                if(Settings.Instance.RefreshInterval == value)
                    return;
                Settings.Instance.RefreshInterval = value * 60000;
                App.ViewModelRoot.LiveNotify.UpdateTimer();
                RaisePropertyChanged();
            }
        }
        #endregion

        public SettingsLiveViewModel() : base("生放送") {

        }

    }
}
