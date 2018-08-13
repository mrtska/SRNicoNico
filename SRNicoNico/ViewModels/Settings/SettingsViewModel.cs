using Livet;
using Livet.Messaging;
using Livet.Messaging.Windows;
using SRNicoNico.Models.NicoNicoViewer;
using System.Diagnostics;

namespace SRNicoNico.ViewModels {
    public class SettingsViewModel : TabItemViewModel {

        #region SettingsList変更通知プロパティ
        private DispatcherCollection<TabItemViewModel> _SettingsList = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<TabItemViewModel> SettingsList {
            get { return _SettingsList; }
            set { 
                if(_SettingsList == value)
                    return;
                _SettingsList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public readonly SettingsCommentViewModel Comment;
        public readonly SettingsNGFilterViewModel NGFilter;

        public SettingsViewModel() : base("設定") {

            SettingsList.Add(new SettingsGeneralViewModel());
            SettingsList.Add(new SettingsVideoViewModel());
            SettingsList.Add(new SettingsRankingViewModel());
            SettingsList.Add(Comment = new SettingsCommentViewModel());
            SettingsList.Add(NGFilter = new SettingsNGFilterViewModel());
            SettingsList.Add(new SettingsLiveViewModel());
        }


        public async void Logout() {

            try {

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync("https://secure.nicovideo.jp/secure/logout");

                App.ViewModelRoot.CurrentUser = await App.ViewModelRoot.SignIn.SignInAsync();
            } catch(RequestFailed) {

                return;
            }

        }

        public void OpenSettingsFolder() {

            Process.Start(Settings.Instance.Dir);
        }

        public void OpenResetConfigView() {

            App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.ResetSettingsView), this, TransitionMode.Modal));
        }

        public void CloseView() {

            Messenger.Raise(new WindowActionMessage(WindowAction.Close));
        }

        public void Reset() {

            Settings.Instance.Reset();
        }
    }
}
