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


        public UserViewModel(string url) {


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


        public UserNicoRepoViewModel(UserViewModel user) : base("ニコレポ") {

            IsActive = true;
            UserNicoRepoList = new DispatcherCollection<NicoRepoResultEntryViewModel>(DispatcherHelper.UIDispatcher);

            Task.Run(() => {

                foreach(var entry in user.UserInstance.GetUserNicoRepo()) {

                    UserNicoRepoList.Add(new NicoRepoResultEntryViewModel(entry));
                }

                IsActive = false;
            });
        }

        public void Next() {


        }



    }
    class UserMylistViewModel : TabItemViewModel {

        public UserMylistViewModel() : base("マイリスト") { }
    }
    class UserVideoViewModel : TabItemViewModel {

        public UserVideoViewModel() : base("投稿") { }
    }

}
