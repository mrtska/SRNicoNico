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
using System.Threading.Tasks;

namespace SRNicoNico.ViewModels {
    public class FavoriteUserViewModel : TabItemViewModel {



        #region UserList変更通知プロパティ
        private DispatcherCollection<NicoNicoFavoriteUser> _UserList;

        public DispatcherCollection<NicoNicoFavoriteUser> UserList {
            get { return _UserList; }
            set { 
                if(_UserList == value)
                    return;
                _UserList = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region SelectedUser変更通知プロパティ
        private NicoNicoFavoriteUser _SelectedUser;

        public NicoNicoFavoriteUser SelectedUser {
            get { return _SelectedUser; }
            set { 
                if(_SelectedUser == value)
                    return;
                _SelectedUser = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private NicoNicoFavorite FavoriteInstance;

        public FavoriteUserViewModel(NicoNicoFavorite fav) : base("ユーザー") {

            FavoriteInstance = fav;
            UserList = new DispatcherCollection<NicoNicoFavoriteUser>(DispatcherHelper.UIDispatcher);
            Task.Run(() => {
                foreach(var entry in fav.GetFavoriteUser()) {

                    UserList.Add(entry);
                }
            });
        }


        //情報再取得
        public void Reflesh() {


        }


    }
}
