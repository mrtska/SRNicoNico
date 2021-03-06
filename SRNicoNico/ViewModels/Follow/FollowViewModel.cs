using System.Linq;
using System.Threading.Tasks;
using Livet;
using Unity;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// フォローページのViewModel
    /// </summary>
    public class FollowViewModel : TabItemViewModel {

        private DispatcherCollection<TabItemViewModel> _HistoryItems = new DispatcherCollection<TabItemViewModel>(App.UIDispatcher);
        /// <summary>
        /// フォローのタブのリスト
        /// </summary>
        public DispatcherCollection<TabItemViewModel> FollowItems {
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
        /// 現在選択されているタブ デフォルトはフォローしているユーザー
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

        public FollowViewModel(IUnityContainer unityContainer) : base("フォロー") {

            UnityContainer = unityContainer;
        }

        public void Loaded() {

            // 別のスレッドで各要素を初期化する
            Task.Run(() => {

                FollowItems.Add(UnityContainer.Resolve<UserFollowViewModel>());

                // 子ViewModelのStatusを監視する
                FollowItems.ToList().ForEach(vm => {

                    vm.PropertyChanged += (o, e) => {

                        var tabItem = (TabItemViewModel)o;
                        if (e.PropertyName == nameof(Status)) {

                            Status = tabItem.Status;
                        }
                    };
                });

                // アカウントの視聴履歴をデフォルト値とする
                SelectedItem = FollowItems.First();
            });
        }


    }
}
