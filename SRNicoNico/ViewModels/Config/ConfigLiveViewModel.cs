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

namespace SRNicoNico.ViewModels {
    public class ConfigLiveViewModel : TabItemViewModel {




        #region AlwaysShowSeekBar変更通知プロパティ

        public bool AlwaysShowSeekBar {
            get { return Properties.Settings.Default.AlwaysShowSeekBar; }
            set {
                if(Properties.Settings.Default.AlwaysShowSeekBar == value)
                    return;
                Properties.Settings.Default.AlwaysShowSeekBar = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }
        #endregion


        #region UseWindowFullScreen変更通知プロパティ
        public bool UseWindowFullScreen {
            get { return Properties.Settings.Default.UseWindowModeFullScreen; }
            set { 
                if(Properties.Settings.Default.UseWindowModeFullScreen == value)
                    return;
                Properties.Settings.Default.UseWindowModeFullScreen = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }
        #endregion

        #region RefreshInterval変更通知プロパティ

        public int RefreshInterval {
            get { return Properties.Settings.Default.RefreshInterval; }
            set { 
                if(Properties.Settings.Default.RefreshInterval == value)
                    return;
                Properties.Settings.Default.RefreshInterval = value;
                App.ViewModelRoot.StatusBar.RefreshTimer.Change(value, value);
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }
        #endregion


        public ConfigLiveViewModel() : base("生放送") {

        }

        public void Reset() {

            RaisePropertyChanged(nameof(AlwaysShowSeekBar));
            RaisePropertyChanged(nameof(UseWindowFullScreen));
            RaisePropertyChanged(nameof(RefreshInterval));

        }


    }
}
