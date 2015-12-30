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
    public class FavoriteCommunityViewModel : TabItemViewModel {

        #region CommunityList変更通知プロパティ
        private DispatcherCollection<NicoNicoFavoriteCommunityContent> _CommunityList;

        public DispatcherCollection<NicoNicoFavoriteCommunityContent> CommunityList {
            get { return _CommunityList; }
            set { 
                if(_CommunityList == value)
                    return;
                _CommunityList = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region SelectedCommunity変更通知プロパティ
        private NicoNicoFavoriteCommunityContent _SelectedCommunity;

        public NicoNicoFavoriteCommunityContent SelectedCommunity {
            get { return _SelectedCommunity; }
            set { 
                if(_SelectedCommunity == value)
                    return;
                _SelectedCommunity = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private NicoNicoFavoriteCommunity FavoriteInstance;

        private FavoriteViewModel Owner;

        private bool IsEnd = false;

        public FavoriteCommunityViewModel(FavoriteViewModel vm) : base("コミュニティ") {

            Owner = vm;
            FavoriteInstance = new NicoNicoFavoriteCommunity();
        }

        public void Initialize() {

            IsActive = true;
            Owner.Status = "お気に入りコミュニティを取得中";
            CommunityList = new DispatcherCollection<NicoNicoFavoriteCommunityContent>(DispatcherHelper.UIDispatcher);
            Task.Run(() => {
                foreach(var entry in FavoriteInstance.GetFavoriteCommunity()) {

                    CommunityList.Add(entry);
                }
                IsActive = false;
                Owner.Status = "";
            });
        }


        //情報再取得
        public void Reflesh() {

            FavoriteInstance.ResetFavoriteCommunity();
            IsEnd = false;
            Initialize();
        }

        //インフィニットスクロール
        public void NextPage() {

            if(IsEnd) {

                return;
            }
            IsActive = true;
            Owner.Status = "お気に入りコミュニティを取得中";
            Task.Run(() => {

                var users = FavoriteInstance.GetFavoriteCommunity();
                if(users == null) {

                    IsEnd = true;
                    IsActive = false;
                    Owner.Status = "";
                    return;
                }

                foreach(var entry in users) {

                    CommunityList.Add(entry);
                }
                IsActive = false;
                Owner.Status = "";
            });
        }

        //ユーザーページを開く
        public void Open() {

            if(SelectedCommunity != null) {

                NicoNicoOpener.Open(SelectedCommunity.CommunityPage);
            }
            SelectedCommunity = null;
        }
        public override void KeyDown(KeyEventArgs e) {

            if(e.Key == Key.F5) {

                Reflesh();
            }

        }
    }
}
