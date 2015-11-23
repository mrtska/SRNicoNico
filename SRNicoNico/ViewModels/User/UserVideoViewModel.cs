using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRNicoNico.ViewModels {
    public class UserVideoViewModel : TabItemViewModel {


        #region UserVideoList変更通知プロパティ
        private DispatcherCollection<SearchResultEntryViewModel> _UserVideoList;

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


        #region SelectedItem変更通知プロパティ
        private SearchResultEntryViewModel _SelectedItem;

        public SearchResultEntryViewModel SelectedItem {
            get { return _SelectedItem; }
            set { 
                if(_SelectedItem == value)
                    return;
                _SelectedItem = value;
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

        private bool IsEnd;

        public UserVideoViewModel(UserViewModel vm) : base("投稿動画") {

            User = vm;
            UserVideoList = new DispatcherCollection<SearchResultEntryViewModel>(DispatcherHelper.UIDispatcher);
        }

        public void Initialize() {

            IsActive = true;

            Task.Run(() => {

                 var videos = User.UserInstance.GetUserVideo();

                if(videos == null) {


                    if(UserVideoList.Count == 0) {

                        //非公開、又は表示期限切れ
                        Closed = true;
                    }
                    IsEnd = true;
                    IsActive = false;
                    return;
                }

                foreach(var video in videos) {

                    UserVideoList.Add(new SearchResultEntryViewModel(video));
                }

                IsActive = false;
            });
        }

        public void Next() {

            if(IsEnd) {

                return;
            }
            IsActive = true;

            Task.Run(() => {

                var videos = User.UserInstance.GetUserVideo();

                if(videos == null) {

                    IsEnd = true;
                    IsActive = false;
                    return;
                }

                foreach(var video in videos) {

                    UserVideoList.Add(new SearchResultEntryViewModel(video));
                }

                IsActive = false;
            });
        }

        public void Open() {

            //not existsの時など
            if(SelectedItem == null) {

                return;
            }
            NicoNicoOpener.Open("http://www.nicovideo.jp/watch/" + SelectedItem.Node.Cmsid);
            SelectedItem = null;
        }

    }
}
