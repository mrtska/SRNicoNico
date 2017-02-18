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

namespace SRNicoNico.ViewModels {
    public class UserVideoViewModel : PageSpinnerViewModel {

        #region UserVideoList変更通知プロパティ
        private DispatcherCollection<SearchResultEntryViewModel> _UserVideoList = new DispatcherCollection<SearchResultEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<SearchResultEntryViewModel> UserVideoList {
            get { return _UserVideoList; }
            set {
                if(_UserVideoList == value)
                    return;
                _UserVideoList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Closed変更通知プロパティ
        private bool _Closed;

        public bool Closed {
            get { return _Closed; }
            set {
                if(_Closed == value)
                    return;
                _Closed = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        //OwnerViewModel
        private UserViewModel User;

        public UserVideoViewModel(UserViewModel vm) : base("投稿動画") {

            User = vm;
        }

        public override void SpinPage() {

            GetPage();
        }

        public async void GetPage() {

            IsActive = true;

            UserVideoList.Clear();

            var videos = await User.UserInstance.GetUserVideoAsync(CurrentPage);

            if(videos == null) {

                if(UserVideoList.Count == 0) {

                    //非公開、又は表示期限切れ
                    Closed = true;
                }
                IsActive = false;
                return;
            }


            foreach(var video in videos) {

                UserVideoList.Add(new SearchResultEntryViewModel(video));
            }


            IsActive = false;
        }

        public async void Initialize() {

            MaxPages = await User.UserInstance.GetUserVideoCountAsync() / 30 + 1;
            CurrentPage = 1;
            GetPage();
        }
    }
}
