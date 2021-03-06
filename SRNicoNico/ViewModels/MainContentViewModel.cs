using System;
using System.Linq;
using Livet;
using Unity;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// MainContentクラスのDataContext
    /// </summary>
    public class MainContentViewModel : ViewModel {

        /// <summary>
        /// システムタブのリスト
        /// WebViewや視聴履歴など
        /// </summary>
        public ObservableSynchronizedCollection<TabItemViewModel> SystemItems { get; private set; }

        /// <summary>
        /// ユーザータブのリスト
        /// 動画やユーザーページなど
        /// </summary>
        public ObservableSynchronizedCollection<TabItemViewModel> UserItems { get; private set; }

        /// <summary>
        /// WebViewのViewModel
        /// </summary>
        public WebViewViewModel? WebView { get; private set; }

        private TabItemViewModel? _SelectedItem;
        /// <summary>
        /// 現在選択されているタブ
        /// </summary>
        public TabItemViewModel? SelectedItem {
            get { return _SelectedItem; }
            set {
                if (_SelectedItem == value)
                    return;
                _SelectedItem = value;
                RaisePropertyChanged();
                if (value != null) {
                    StatusChangedAction?.Invoke(value.Status);
                }
            }
        }

        private readonly IUnityContainer UnityContainer;
        private Action<string>? StatusChangedAction;

        public MainContentViewModel(IUnityContainer container) {

            UnityContainer = container;
            SystemItems = new ObservableSynchronizedCollection<TabItemViewModel>();
            UserItems = new ObservableSynchronizedCollection<TabItemViewModel>();

            SystemItems.Add(container.Resolve<StartViewModel>());

            // スタートページをデフォルトで開くようにする
            SelectedItem = SystemItems.First();
        }

        /// <summary>
        /// サインイン完了後に表示するタブをリストに追加する
        /// </summary>
        public void PostInitialize() {

            SystemItems.Add(WebView = UnityContainer.Resolve<WebViewViewModel>());
            SystemItems.Add(UnityContainer.Resolve<FollowViewModel>());
            SystemItems.Add(UnityContainer.Resolve<HistoryViewModel>());

            SystemItems.ToList().ForEach(vm => {

                // すべてのViewModelをCompositeDisposableに登録する
                CompositeDisposable.Add(vm);

                // Statusを監視する
                vm.PropertyChanged += (o, e) => {

                    var tabItem = (TabItemViewModel)o;
                    if (e.PropertyName == nameof(TabItemViewModel.Status)) {

                        StatusChangedAction?.Invoke(tabItem.Status);
                    }
                };
            });

        }

        public void RegisterStatusChangeAction(Action<string> action) {

            StatusChangedAction = action;
        }

    }
}
