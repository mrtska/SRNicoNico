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
using System.Collections.ObjectModel;

namespace SRNicoNico.ViewModels {
    public class ConfigViewModel : TabItemViewModel {


        #region ConfigCollection変更通知プロパティ
        private ObservableCollection<TabItemViewModel> _ConfigCollection = new ObservableCollection<TabItemViewModel>();

        public ObservableCollection<TabItemViewModel> ConfigCollection {
            get { return _ConfigCollection; }
            set {
                if(_ConfigCollection == value)
                    return;
                _ConfigCollection = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region SelectedItem変更通知プロパティ
        private TabItemViewModel _SelectedItem;

        public TabItemViewModel SelectedItem {
            get { return _SelectedItem; }
            set {
                if(_SelectedItem == value)
                    return;
                _SelectedItem = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public ConfigGeneralViewModel General { get; private set; }
        public ConfigVideoViewModel Video { get; private set; }
        public ConfigCommentViewModel Comment { get; private set; }
        public ConfigLiveViewModel Live { get; private set; }


        public ConfigViewModel() : base("設定") {

            ConfigCollection.Add(General = new ConfigGeneralViewModel(this));
            ConfigCollection.Add(Video = new ConfigVideoViewModel());
            ConfigCollection.Add(Comment = new ConfigCommentViewModel());
            ConfigCollection.Add(Live = new ConfigLiveViewModel());

        }

        public void Reset() {

            Video.Reset();
            Comment.Reset();
            Live.Reset();

        }
    }
}
