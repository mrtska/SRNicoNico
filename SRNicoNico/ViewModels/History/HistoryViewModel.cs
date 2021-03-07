using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Livet;
using SRNicoNico.Services;
using Unity;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// 視聴履歴ページのViewModel
    /// </summary>
    public class HistoryViewModel : TabItemViewModel {

        private DispatcherCollection<TabItemViewModel> _HistoryItems = new DispatcherCollection<TabItemViewModel>(App.UIDispatcher);
        /// <summary>
        /// 履歴のタブのリスト
        /// </summary>
        public DispatcherCollection<TabItemViewModel> HistoryItems {
            get { return _HistoryItems; }
            set { 
                if (_HistoryItems == value)
                    return;
                _HistoryItems = value;
                RaisePropertyChanged();
            }
        }

        private TabItemViewModel? _SelectedItem;
        /// <summary>
        /// 現在選択されているタブ デフォルトはアカウントの視聴履歴
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

        public HistoryViewModel(IUnityContainer unityContainer) : base("履歴") {

            UnityContainer = unityContainer;
        }

        public void Loaded() {

            // 別のスレッドで各要素を初期化する
            Task.Run(() => {

                HistoryItems.Add(UnityContainer.Resolve<AccountHistoryViewModel>());
                HistoryItems.Add(UnityContainer.Resolve<LocalHistoryViewModel>());

                // 子ViewModelのStatusを監視する
                HistoryItems.ToList().ForEach(vm => {

                    vm.PropertyChanged += (o, e) => {

                        var tabItem = (TabItemViewModel)o;
                        if (e.PropertyName == nameof(Status)) {

                            Status = tabItem.Status;
                        }
                    };
                });

                // アカウントの視聴履歴をデフォルト値とする
                SelectedItem = HistoryItems.First();
            });
        }

        public override void KeyDown(KeyEventArgs e) {
            SelectedItem?.KeyDown(e);
        }
    }
}
