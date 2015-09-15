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
        private string _VideoPlacement;

        public string VideoPlacement {
            get { return _VideoPlacement; }
            set { 
                if(_VideoPlacement == value)
                    return;
                _VideoPlacement = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        public ConfigVideoViewModel() : base("動画関連") {

            VideoPlacement = Properties.Settings.Default.VideoInfoPlacement;
        }

        public void ToggleVideoPlacement() {

            VideoPlacement = VideoPlacement == "Left" ? "Right" : "Left";
            Properties.Settings.Default.VideoInfoPlacement = VideoPlacement;
            Properties.Settings.Default.Save();
            Console.WriteLine("!!!" + VideoPlacement);
        }



    }
}
