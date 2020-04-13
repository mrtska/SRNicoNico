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

        private TabItemViewModel _SelectedItem;
        /// <summary>
        /// 現在選択されているタブ
        /// </summary>
        public TabItemViewModel SelectedItem {
            get { return _SelectedItem; }
            set { 
                if (_SelectedItem == value)
                    return;
                _SelectedItem = value;
                RaisePropertyChanged();
            }
        }

        public MainContentViewModel(IUnityContainer container) {

            SystemItems = new ObservableSynchronizedCollection<TabItemViewModel>();
            UserItems = new ObservableSynchronizedCollection<TabItemViewModel>();
            Initialize(container);
        }

        private void Initialize(IUnityContainer container) {

            SystemItems.Add(container.Resolve<StartViewModel>());
            SystemItems.Add(container.Resolve<WebViewViewModel>());


            // スタートページをデフォルトで開くようにする
            SelectedItem = SystemItems.First();
        }

    }
}
