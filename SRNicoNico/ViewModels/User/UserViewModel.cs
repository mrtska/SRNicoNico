using Livet;
using Livet.Messaging;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class UserViewModel : TabItemViewModel {

        private static readonly Regex UserUrlPattern = new Regex(@"http://www.nicovideo.jp/user/\d+");

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
        private ObservableSynchronizedCollection<TabItemViewModel> _UserContentList;

        public ObservableSynchronizedCollection<TabItemViewModel> UserContentList {
            get { return _UserContentList; }
            set {
                if(_UserContentList == value)
                    return;
                _UserContentList = value;
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

        public NicoNicoUser Model { get; set; }

        public UserViewModel(string url) : base("ユーザー") {

            UserPageUrl = UserUrlPattern.Match(url).Value;

            UserContentList = new ObservableSynchronizedCollection<TabItemViewModel>();
            Model = new NicoNicoUser(UserPageUrl);
        }

        public async void Initialize() {

            IsActive = true;
            Status = "ユーザー情報取得中";
            Status = await Model.GetUserInfoAsync();

            if(Model.UserInfo == null) {

                LoadFailed = true;
            } else {

                UserContentList.Clear();
                UserContentList.Add(new UserNicoRepoViewModel(this));
                UserContentList.Add(new UserMylistViewModel(this));
                UserContentList.Add(new UserVideoViewModel(this));
                Name += "\n" + Model.UserInfo.UserName;
            }
            IsActive = false;
        }

        public async void ToggleFollow() {

            Status = await Model.ToggleFollowOwnerAsync();
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
