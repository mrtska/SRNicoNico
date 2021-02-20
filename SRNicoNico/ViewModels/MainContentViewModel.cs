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
            }
        }

        private readonly IUnityContainer UnityContainer;

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
            
            SystemItems.Add(UnityContainer.Resolve<WebViewViewModel>());
            SystemItems.Add(UnityContainer.Resolve<HistoryViewModel>());

            // すべてのViewModelをCompositeDisposableに登録する
            SystemItems.ToList().ForEach(vm => CompositeDisposable.Add(vm));
        }

    }
}
