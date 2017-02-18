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
using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class FollowViewModel : TabItemViewModel {

        private readonly NicoNicoFollow FollowInstance;


        #region FavoriteList変更通知プロパティ
        private DispatcherCollection<TabItemViewModel> _FavoriteList = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<TabItemViewModel> FavoriteList {
            get { return _FavoriteList; }
            set {
                if(_FavoriteList == value)
                    return;
                _FavoriteList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SelectedList変更通知プロパティ
        private TabItemViewModel _SelectedList;

        public TabItemViewModel SelectedList {
            get { return _SelectedList; }
            set {
                if(_SelectedList == value)
                    return;
                _SelectedList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public FollowViewModel() : base("フォロー") {

            FollowInstance = new NicoNicoFollow(this);

            FavoriteList.Add(new FollowUserViewModel(this, FollowInstance));
            FavoriteList.Add(new FollowMylistViewModel(this, FollowInstance));
            FavoriteList.Add(new FollowChannelViewModel(this, FollowInstance));
            FavoriteList.Add(new FollowCommunityViewModel(this, FollowInstance));
        }

        public override void KeyDown(KeyEventArgs e) {

            SelectedList?.KeyDown(e);
        }
    }
}
