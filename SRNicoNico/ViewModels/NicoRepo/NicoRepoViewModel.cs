using Livet;
using Livet.Messaging;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class NicoRepoViewModel : TabItemViewModel {

        #region NicoRepoList変更通知プロパティ
        private ObservableSynchronizedCollection<TabItemViewModel> _NicoRepoList;

        public ObservableSynchronizedCollection<TabItemViewModel> NicoRepoList {
            get { return _NicoRepoList; }
            set {
                if(_NicoRepoList == value)
                    return;
                _NicoRepoList = value;
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


        public NicoRepoViewModel() : base("ニコレポ") {

            NicoRepoList = new ObservableSynchronizedCollection<TabItemViewModel>();
        }

        public void Initialize() {

            IsActive = true;
            NicoRepoList.Clear();
            NicoRepoList.Add(new NicoRepoResultViewModel(this, "すべて", "all"));
            NicoRepoList.Add(new NicoRepoResultViewModel(this, "自分", "self"));
            NicoRepoList.Add(new NicoRepoResultViewModel(this, "ユーザー", "followingUser"));
            NicoRepoList.Add(new NicoRepoResultViewModel(this, "チャンネル", "followingChannel"));
            NicoRepoList.Add(new NicoRepoResultViewModel(this, "コミュニティ", "followingCommunity"));
            NicoRepoList.Add(new NicoRepoResultViewModel(this, "マイリスト", "followingMylist"));

            IsActive = false;
        }

        public void Refresh() {

            Initialize();
        }

        public override void KeyDown(KeyEventArgs e) {

            if(e.KeyboardDevice.Modifiers == ModifierKeys.Control) {

                if(e.Key == Key.F5) {

                    Refresh();
                    return;
                }
            }
            SelectedList?.KeyDown(e);
        }

        public override bool CanShowHelp() {
            return true;
        }
        public override void ShowHelpView(InteractionMessenger Messenger) {

            Messenger.Raise(new TransitionMessage(typeof(Views.NicoRepoHelpView), this, TransitionMode.NewOrActive));
        }
    }
}
