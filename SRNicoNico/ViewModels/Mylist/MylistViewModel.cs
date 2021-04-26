using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Livet;
using Unity;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// マイリストページのViewModel
    /// </summary>
    public class MylistViewModel : TabItemViewModel {

        private DispatcherCollection<TabItemViewModel> _NicoRepoListItems = new DispatcherCollection<TabItemViewModel>(App.UIDispatcher);
        /// <summary>
        /// ニコレポのタブのリスト
        /// </summary>
        public DispatcherCollection<TabItemViewModel> NicoRepoItems {
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

        public MylistViewModel(IUnityContainer unityContainer) : base("マイリスト") {

            UnityContainer = unityContainer;
        }

        public void Loaded() {

            // 別のスレッドで各要素を初期化する
            Task.Run(() => {


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
            });
        }

        public override void KeyDown(KeyEventArgs e) {
            SelectedItem?.KeyDown(e);
        }
    }
}
