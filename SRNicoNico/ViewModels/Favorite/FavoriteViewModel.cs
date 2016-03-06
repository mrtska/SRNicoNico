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

using SRNicoNico.Models.NicoNicoWrapper;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class FavoriteViewModel : TabItemViewModel {


        #region FavoriteList変更通知プロパティ
        private DispatcherCollection<TabItemViewModel> _FavoriteList;

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


        public FavoriteViewModel() : base("お気に入り") {

            FavoriteList = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);

            FavoriteList.Add(new FavoriteUserViewModel(this));
            FavoriteList.Add(new FavoriteCommunityViewModel(this));


        }

        public void Refresh() {
            
                
        }

        public override void KeyDown(KeyEventArgs e) {

            SelectedList?.KeyDown(e);
        }
    }
}
