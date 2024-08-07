using System.Linq;
using System.Windows.Input;
using Livet;
using Unity;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ニコレポページのViewModel
    /// </summary>
    public class NicoRepoViewModel : TabItemViewModel {

        private ObservableSynchronizedCollection<TabItemViewModel> _NicoRepoListItems = new ObservableSynchronizedCollection<TabItemViewModel>();
        /// <summary>
        /// ニコレポのタブのリスト
        /// </summary>
        public ObservableSynchronizedCollection<TabItemViewModel> NicoRepoItems {
            get { return _NicoRepoListItems; }
            set {
                if (_NicoRepoListItems == value)
                    return;
                _NicoRepoListItems = value;
                RaisePropertyChanged();
            }
        }

        private TabItemViewModel? _SelectedItem;
        /// <summary>
        /// 現在選択されているタブ デフォルトはすべて
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

        public NicoRepoViewModel(IUnityContainer unityContainer) : base("ニコレポ") {

            UnityContainer = unityContainer;
        }

        /// <summary>
        /// ニコレポの一覧をインスタンス化する
        /// </summary>
        public void Loaded() {

            NicoRepoItems.Add(UnityContainer.Resolve<NicoRepoListAllViewModel>());
            NicoRepoItems.Add(UnityContainer.Resolve<NicoRepoListSelfViewModel>());
            NicoRepoItems.Add(UnityContainer.Resolve<NicoRepoListUserViewModel>());
            NicoRepoItems.Add(UnityContainer.Resolve<NicoRepoListChannelViewModel>());
            NicoRepoItems.Add(UnityContainer.Resolve<NicoRepoListCommunityViewModel>());
            NicoRepoItems.Add(UnityContainer.Resolve<NicoRepoListMylistViewModel>());

            // 子ViewModelのStatusを監視する
            NicoRepoItems.ToList().ForEach(vm => {

                vm.PropertyChanged += (o, e) => {

                    var tabItem = (TabItemViewModel)o;
                    if (e.PropertyName == nameof(Status)) {

                        Status = tabItem.Status;
                    }
                };
            });

            // 「すべて」をデフォルト値とする
            SelectedItem = NicoRepoItems.First();
        }

        public override void KeyDown(KeyEventArgs e) {
            SelectedItem?.KeyDown(e);
        }
    }
}
