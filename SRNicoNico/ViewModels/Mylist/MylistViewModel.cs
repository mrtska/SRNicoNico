using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Livet;
using SRNicoNico.Services;
using Unity;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// マイリストページのViewModel
    /// </summary>
    public class MylistViewModel : TabItemViewModel {

        private DispatcherCollection<TabItemViewModel> _MylistListItems = new DispatcherCollection<TabItemViewModel>(App.UIDispatcher);
        /// <summary>
        /// マイリストのタブのリスト
        /// </summary>
        public DispatcherCollection<TabItemViewModel> MylistListItems {
            get { return _MylistListItems; }
            set { 
                if (_MylistListItems == value)
                    return;
                _MylistListItems = value;
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
        private readonly IMylistService MylistService;

        public MylistViewModel(IUnityContainer unityContainer, IMylistService mylistService) : base("マイリスト") {

            UnityContainer = unityContainer;
            MylistService = mylistService;
        }

        /// <summary>
        /// マイリストの一覧を非同期で取得する
        /// </summary>
        public void Loaded() {

            // 別のスレッドで各要素を初期化する
            Task.Run(() => {

                // 子ViewModelのStatusを監視する
                MylistListItems.ToList().ForEach(vm => {

                    vm.PropertyChanged += (o, e) => {

                        var tabItem = (TabItemViewModel)o;
                        if (e.PropertyName == nameof(Status)) {

                            Status = tabItem.Status;
                        }
                    };
                });

                // 「すべて」をデフォルト値とする
                SelectedItem = MylistListItems.FirstOrDefault();
            });
        }

        public override void KeyDown(KeyEventArgs e) {
            SelectedItem?.KeyDown(e);
        }
    }
}
