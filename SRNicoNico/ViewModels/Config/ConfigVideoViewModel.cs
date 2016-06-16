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
    public class ConfigVideoViewModel : ConfigViewModelBase {



        #region VideoPlacement変更通知プロパティ

        public string VideoPlacement {
            get { return Settings.Instance.VideoInfoPlacement; }
            set { 
                if(Settings.Instance.VideoInfoPlacement == value)
                    return;
                Settings.Instance.VideoInfoPlacement = value;
                RaisePropertyChanged();

                if(value == "Right") {

                    Application.Current.Resources["VideoColumn"] = 0;
                    Application.Current.Resources["InfoColumn"] = 1;
                    Application.Current.Resources["GridWidth1"] = new GridLength(1.0, GridUnitType.Star);
                    Application.Current.Resources["GridWidth2"] = new GridLength(300);
                } else {

                    Application.Current.Resources["VideoColumn"] = 1;
                    Application.Current.Resources["InfoColumn"] = 0;
                    Application.Current.Resources["GridWidth1"] = new GridLength(300);
                    Application.Current.Resources["GridWidth2"] = new GridLength(1.0, GridUnitType.Star);

                }
            }
        }
        #endregion




        public ConfigVideoViewModel() : base("動画") {

        }

    }
}
