using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Livet;
using Unity;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// フォローページのViewModel
    /// </summary>
    public class FollowViewModel : TabItemViewModel {

        private DispatcherCollection<TabItemViewModel> _FollowItems = new DispatcherCollection<TabItemViewModel>(App.UIDispatcher);
        /// <summary>
        /// フォローのタブのリスト
        /// </summary>
        public DispatcherCollection<TabItemViewModel> FollowItems {
            get { return _FollowItems; }
            set { 
                if (_FollowItems == value)
                    return;
                _FollowItems = value;
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
                FollowItems.Add(UnityContainer.Resolve<TagFollowViewModel>());
                FollowItems.Add(UnityContainer.Resolve<MylistFollowViewModel>());
                FollowItems.Add(UnityContainer.Resolve<ChannelFollowViewModel>());
                FollowItems.Add(UnityContainer.Resolve<CommunityFollowViewModel>());

                // 子ViewModelのStatusを監視する
                FollowItems.ToList().ForEach(vm => {

                    vm.PropertyChanged += (o, e) => {

                        var tabItem = (TabItemViewModel)o;
                        if (e.PropertyName == nameof(Status)) {

                            Status = tabItem.Status;
                        }
                    };
                });

                // ユーザーフォローをデフォルト値とする
                SelectedItem = FollowItems.First();
            });
        }

        public override void KeyDown(KeyEventArgs e) {
            SelectedItem?.KeyDown(e);
        }
    }
}
