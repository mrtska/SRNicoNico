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
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class UserViewModel : TabItemViewModel {

        private static readonly Regex UserUrlPattern = new Regex(@"http://www.nicovideo.jp/user/\d+");

        protected internal readonly NicoNicoUser UserInstance;



        #region UserPageUrl変更通知プロパティ
        private string _UserPageUrl;

        public string UserPageUrl {
            get { return _UserPageUrl; }
            set { 
                if(_UserPageUrl == value)
                    return;
                _UserPageUrl = value;
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

        #region UserInfo変更通知プロパティ
        private NicoNicoUserEntry _UserInfo;

        public NicoNicoUserEntry UserInfo {
            get { return _UserInfo; }
            set { 
                if(_UserInfo == value)
                    return;
                _UserInfo = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region LoadFailed変更通知プロパティ
        private  bool _LoadFailed = false;

        public  bool LoadFailed {
            get { return _LoadFailed; }
            set { 
                if(_LoadFailed == value)
                    return;
                _LoadFailed = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public UserViewModel(string url) : base("ユーザー") {


            UserPageUrl = UserUrlPattern.Match(url).Value;


            UserInstance = new NicoNicoUser(this);
        }

        public async void Initialize() {

            IsActive = true;
            var info = await UserInstance.GetUserInfoAsync();

            if(info == null) {

                LoadFailed = true;
            } else {

                UserInfo = info;
                UserContentList.Clear();

                UserContentList.Add(new UserNicoRepoViewModel(this));
                UserContentList.Add(new UserMylistViewModel(this));
                UserContentList.Add(new UserVideoViewModel(this));
            }
            Name += "\n" + UserInfo.UserName;


            IsActive = false;
        }

        public async void ToggleFollow() {

            if(UserInfo != null) {

                if(await UserInstance.ToggleFollowOwnerAsync(UserInfo)) {

                    UserInfo.IsFollow ^= true;
                }
               
            }
        }

        public void Refresh() {

            Initialize();
        }


        public void Close() {

            App.ViewModelRoot.MainContent.RemoveUserTab(this);
        }

        public override void KeyDown(KeyEventArgs e) {

            if(e.KeyboardDevice.Modifiers == ModifierKeys.Control) {

                if(e.Key == Key.W) {

                    Close();
                    return;
                }

                if(e.Key == Key.F5) {

                    Refresh();
                    return;
                }
            }
            SelectedList?.KeyDown(e);

        }

        public override bool CanShowHelp() {
            return false;
        }

        public override void ShowHelpView(InteractionMessenger Messenger) {

            Messenger.Raise(new TransitionMessage(typeof(Views.StartHelpView), this, TransitionMode.NewOrActive));
        }

    }
}
