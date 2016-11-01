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
using System.Windows.Input;

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
        private DispatcherCollection<TabItemViewModel> _UserContentList = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);

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


        #region SelectedIndex変更通知プロパティ
        private int _SelectedIndex;

        public int SelectedIndex {
            get { return _SelectedIndex; }
            set { 
                if(_SelectedIndex == value)
                    return;
                _SelectedIndex = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        private readonly string UserPageUrl;

        public UserViewModel(string url) : base("読込中") {

            UserPageUrl = url;

            Task.Run(() => Initialize());
        }

        public async void Initialize() {

            UserInstance = new NicoNicoUser(this, UserPageUrl);
            UserEntry = await UserInstance.GetUserInfoAsync();
            Name = UserEntry.UserName;

            UserContentList.Clear();

            UserContentList.Add(new UserNicoRepoViewModel(this));
            UserContentList.Add(new UserMylistViewModel(this));
            UserContentList.Add(new UserVideoViewModel(this));

        }

        public void OpenBrowser() {

            System.Diagnostics.Process.Start(UserPageUrl);
        }

        public void Close() {

            App.ViewModelRoot.RemoveTabAndLastSet(this);
        }

        public void Refresh() {

            Task.Run(() => {

                Initialize();
                SelectedIndex = 0;
            });

        }

        public override void KeyDown(KeyEventArgs e) {

            if(e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.W) {

                Close();
            } else if(e.Key == Key.F5) {

                Refresh();
            }
        }

    }

    

}
