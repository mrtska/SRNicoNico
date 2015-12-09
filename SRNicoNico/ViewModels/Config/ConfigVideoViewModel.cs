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

                //タブから動画タブを探して情報コントロールの位置を変える
                foreach(VideoViewModel tab in App.ViewModelRoot.TabItems.Where(tab => tab is VideoViewModel)) {

                    //プロパティにダミー値を与えて変更通知を飛ばす この方法が最良かは不明
                    tab.VideoInfoPlacement = "dummy";
                }

            }
        }
        #endregion


        #region EnableTwitterLink変更通知プロパティ

        public bool EnableTwitterLink {
            get { return Properties.Settings.Default.EnableTwitterLink; }
            set { 
                if(Properties.Settings.Default.EnableTwitterLink == value)
                    return;
                Properties.Settings.Default.EnableTwitterLink = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }
        #endregion


        #region EnableUrlLink変更通知プロパティ

        public bool EnableUrlLink {
            get { return Properties.Settings.Default.EnableUrlLink; }
            set { 
                if(Properties.Settings.Default.EnableUrlLink == value)
                    return;
                Properties.Settings.Default.EnableUrlLink = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }
        #endregion



        public ConfigVideoViewModel() : base("動画関連") {

            VideoPlacement = Properties.Settings.Default.VideoInfoPlacement;
            EnableTwitterLink = Properties.Settings.Default.EnableTwitterLink;
            EnableUrlLink = Properties.Settings.Default.EnableUrlLink;
        }
    }
}
