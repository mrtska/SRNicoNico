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
using SRNicoNico.Models.NicoNicoViewer;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class FavoriteUserViewModel : TabItemViewModel {



        #region UserList変更通知プロパティ
        private DispatcherCollection<NicoNicoFavoriteUserContent> _UserList;

        public DispatcherCollection<NicoNicoFavoriteUserContent> UserList {
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
        private NicoNicoFavoriteUserContent _SelectedUser;

        public NicoNicoFavoriteUserContent SelectedUser {
            get { return _SelectedUser; }
            set { 
                if(_SelectedUser == value)
                    return;
                _SelectedUser = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private NicoNicoFavoriteUser FavoriteInstance;

        private FavoriteViewModel Owner;

        private bool IsEnd = false;

        public FavoriteUserViewModel(FavoriteViewModel vm) : base("ユーザー") {

            Owner = vm;
            FavoriteInstance = new NicoNicoFavoriteUser();
        }
        
        public void Initialize() {

            IsActive = true;
            Owner.Status = "フォローユーザーを取得中";
            UserList = new DispatcherCollection<NicoNicoFavoriteUserContent>(DispatcherHelper.UIDispatcher);
            Task.Run(() => {
                foreach(var entry in FavoriteInstance.GetFavoriteUser()) {

                    UserList.Add(entry);
                }
                IsActive = false;
                Owner.Status = "";
            });
        }


        //情報再取得
        public void Refresh() {

            FavoriteInstance.ResetFavoriteUser();
            IsEnd = false;
            Initialize();
        }

        //インフィニットスクロール
        public void NextPage() {

            if(IsEnd) {

                return;
            }
            IsActive = true;
            Owner.Status = "フォローユーザーを取得中";
            Task.Run(() => {

                var users = FavoriteInstance.GetFavoriteUser();
                if(users == null) {

                    IsEnd = true;
                    IsActive = false;
                    Owner.Status = "";
                    return;
                }

                foreach(var entry in users) {

                    UserList.Add(entry);
                }
                IsActive = false;
                Owner.Status = "";
            });
        }

        //ユーザーページを開く
        public void Open() {

            if(SelectedUser != null) {

                NicoNicoOpener.Open(SelectedUser.UserPage);
            }
            SelectedUser = null;
        }
        public override void KeyDown(KeyEventArgs e) {

            if(e.Key == Key.F5) {

                Refresh();
            }

        }
    }
}
