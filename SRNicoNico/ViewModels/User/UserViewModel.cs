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
using System.Threading.Tasks;
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.ViewModels {
    public class UserViewModel : TabItemViewModel {

        internal NicoNicoUser UserInstance;

        #region UserEntry変更通知プロパティ
        private NicoNicoUserEntry _UserEntry;

        public NicoNicoUserEntry UserEntry {
            get { return _UserEntry; }
            set { 
                if(_UserEntry == value)
                    return;
                _UserEntry = value;
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


        #region UserContentList変更通知プロパティ
        private DispatcherCollection<TabItemViewModel> _UserContentList;

        public DispatcherCollection<TabItemViewModel> UserContentList {
            get { return _UserContentList; }
            set { 
                if(_UserContentList == value)
                    return;
                _UserContentList = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public UserViewModel(string url) : base("読込中") {


            App.ViewModelRoot.TabItems.Add(this);
            App.ViewModelRoot.SelectedTab = this;
            UserInstance = new NicoNicoUser(this, url);

            UserEntry = UserInstance.GetUserInfo();
            Name = UserEntry.UserName;

            UserContentList = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher) {

                new UserNicoRepoViewModel(this),
                new UserMylistViewModel(),
                new UserVideoViewModel()
            };

        }

        public void OpenBrowser() {

            System.Diagnostics.Process.Start(UserEntry.UserPage);
        }

        public void Close() {

            App.ViewModelRoot.TabItems.Remove(this);
        }
    }

    class UserNicoRepoViewModel : TabItemViewModel {


        #region UserNicoRepoList変更通知プロパティ
        private DispatcherCollection<NicoRepoResultEntryViewModel> _UserNicoRepoList;

        public DispatcherCollection<NicoRepoResultEntryViewModel> UserNicoRepoList {
            get { return _UserNicoRepoList; }
            set { 
                if(_UserNicoRepoList == value)
                    return;
                _UserNicoRepoList = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region SelectedItem変更通知プロパティ
        private NicoRepoResultEntryViewModel _SelectedItem;

        public NicoRepoResultEntryViewModel SelectedItem {
            get { return _SelectedItem; }
            set { 
                if(_SelectedItem == value)
                    return;
                _SelectedItem = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        //OwnerViewModel
        private UserViewModel User;

        //ニコレポを全て取得し終わったら
        private bool IsEnd = false;

        public UserNicoRepoViewModel(UserViewModel user) : base("ニコレポ") {

            User = user;
            IsActive = true;
            UserNicoRepoList = new DispatcherCollection<NicoRepoResultEntryViewModel>(DispatcherHelper.UIDispatcher);

            Task.Run(() => {

                var timeline = User.UserInstance.GetUserNicoRepo();

                if(timeline == null) {

                    IsActive = false;
                    return;
                }
                foreach(var entry in timeline) {

                    UserNicoRepoList.Add(new NicoRepoResultEntryViewModel(entry));
                }

                IsActive = false;
            });
        }

        //インフィニットスクロール発動で呼ばれる
        public void Next() {

            if(IsEnd) {

                return;
            }
            IsActive = true;

            Task.Run(() => {

                var timeline = User.UserInstance.GetUserNicoRepo();

                if(timeline == null) {

                    IsEnd = true;
                    IsActive = false;
                    return;
                }
                foreach(var entry in timeline) {

                    UserNicoRepoList.Add(new NicoRepoResultEntryViewModel(entry));
                }

                IsActive = false;
            });
        }

        public void Open() {


            //not existsの時など
            if(SelectedItem == null || SelectedItem.Entry.VideoUrl == null) {

                SelectedItem = null;
                return;
            }

            NicoNicoOpener.Open(SelectedItem.Entry.VideoUrl);

            SelectedItem = null;
        }



    }
    class UserMylistViewModel : TabItemViewModel {

        public UserMylistViewModel() : base("マイリスト") { }
    }
    class UserVideoViewModel : TabItemViewModel {

        public UserVideoViewModel() : base("投稿") { }
    }

}
