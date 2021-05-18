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
        public DispatcherCollection<TabItemViewModel> UserItems { get; private set; }

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

        private readonly VideoTabViewModel VideoTab;

        public MainContentViewModel(IUnityContainer container) {

            UnityContainer = container;
            SystemItems = new ObservableSynchronizedCollection<TabItemViewModel>();
            UserItems = new DispatcherCollection<TabItemViewModel>(App.UIDispatcher);

            SystemItems.Add(container.Resolve<StartViewModel>());

            // スタートページをデフォルトで開くようにする
            SelectedItem = SystemItems.First();

            VideoTab = new VideoTabViewModel(() => {
                UserItems.Remove(VideoTab!);
            });
        }

        /// <summary>
        /// サインイン完了後に表示するタブをリストに追加する
        /// </summary>
        public void PostInitialize() {

            SystemItems.Add(WebView = UnityContainer.Resolve<WebViewViewModel>());
            SystemItems.Add(UnityContainer.Resolve<FollowViewModel>());
            SystemItems.Add(UnityContainer.Resolve<NicoRepoViewModel>());
            SystemItems.Add(UnityContainer.Resolve<WatchLaterViewModel>());
            SystemItems.Add(UnityContainer.Resolve<MylistViewModel>());
            SystemItems.Add(UnityContainer.Resolve<HistoryViewModel>());
            SystemItems.Add(UnityContainer.Resolve<OtherViewModel>());

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

        /// <summary>
        /// 動画タブ管理用ViewModelに動画のViewModelを追加する
        /// </summary>
        /// <param name="vm"></param>
        public void AddVideoTab(VideoViewModel vm) {

            VideoTab.Add(vm);
            // 動画タブが表に表示されていなければ表示する
            if (!UserItems.Contains(VideoTab)) {

                UserItems.Add(VideoTab);
            }
            // 表示を切り替える
            SelectedItem = VideoTab;
        }

        public void RegisterStatusChangeAction(Action<string> action) => StatusChangedAction = action;
    }
}
