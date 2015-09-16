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

namespace SRNicoNico.ViewModels {
    public class ConfigVideoViewModel : TabItemViewModel {



        #region VideoPlacement変更通知プロパティ

        public string VideoPlacement {
            get { return Properties.Settings.Default.VideoInfoPlacement; }
            set { 
                if(Properties.Settings.Default.VideoInfoPlacement == value)
                    return;
                Properties.Settings.Default.VideoInfoPlacement = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }
        #endregion
        


        public ConfigVideoViewModel() : base("動画関連") {

            VideoPlacement = Properties.Settings.Default.VideoInfoPlacement;
        }
    }
}
