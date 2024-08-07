using System.Linq;
using System.Windows.Input;
using Livet;
using Unity;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// 設定ページのViewModel
    /// </summary>
    public class SettingsViewModel : TabItemViewModel {

        private ObservableSynchronizedCollection<TabItemViewModel> _SettingsItems = new ObservableSynchronizedCollection<TabItemViewModel>();
        /// <summary>
        /// 設定のタブのリスト
        /// </summary>
        public ObservableSynchronizedCollection<TabItemViewModel> SettingsItems {
            get { return _SettingsItems; }
            set {
                if (_SettingsItems == value)
                    return;
                _SettingsItems = value;
                RaisePropertyChanged();
            }
        }

        private TabItemViewModel? _SelectedItem;
        /// <summary>
        /// 現在選択されているタブ デフォルトは一般ページ
        /// </summary>
        public TabItemViewModel? SelectedItem {
            get { return _SelectedItem; }
            set {
                if (_SelectedItem == value)
                    return;
                _SelectedItem = value;
                RaisePropertyChanged();
            }
        }

        private readonly IUnityContainer UnityContainer;

        public SettingsViewModel(IUnityContainer unityContainer) : base("設定") {

            UnityContainer = unityContainer;
        }

        /// <summary>
        /// 設定タブの一覧をインスタンス化する
        /// </summary>
        public void Loaded() {

            SettingsItems.Add(UnityContainer.Resolve<SettingsGeneralViewModel>());
            SettingsItems.Add(UnityContainer.Resolve<SettingsRankingViewModel>());
            SettingsItems.Add(UnityContainer.Resolve<SettingsMutedAccountViewModel>());
            SettingsItems.Add(UnityContainer.Resolve<SettingsVideoViewModel>());
            SettingsItems.Add(UnityContainer.Resolve<SettingsCommentViewModel>());
            SettingsItems.Add(UnityContainer.Resolve<SettingsLiveViewModel>());

            // 子ViewModelのStatusを監視する
            SettingsItems.ToList().ForEach(vm => {

                vm.PropertyChanged += (o, e) => {

                    var tabItem = (TabItemViewModel)o;
                    if (e.PropertyName == nameof(Status)) {

                        Status = tabItem.Status;
                    }
                };
            });

            // 一般設定をデフォルト値とする
            SelectedItem = SettingsItems.First();
        }

        public override void KeyDown(KeyEventArgs e) {
            SelectedItem?.KeyDown(e);
        }
    }
}
