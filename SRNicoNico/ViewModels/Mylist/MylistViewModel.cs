using System.Linq;
using System.Windows.Input;
using FastEnumUtility;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Services;
using Unity;
using Unity.Resolution;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// マイリストページのViewModel
    /// </summary>
    public class MylistViewModel : TabItemViewModel {

        private ObservableSynchronizedCollection<TabItemViewModel> _MylistListItems = new ObservableSynchronizedCollection<TabItemViewModel>();
        /// <summary>
        /// マイリストのタブのリスト
        /// </summary>
        public ObservableSynchronizedCollection<TabItemViewModel> MylistListItems {
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
        /// 現在選択されているタブ デフォルトは一番上のマイリスト
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
        public async void Loaded() {

            IsActive = true;
            Status = "マイリストの一覧を取得中";
            MylistListItems.Clear();
            try {

                await foreach (var result in MylistService.GetMylistsAsync()) {

                    var vm = UnityContainer.Resolve<MylistListViewModel>(new ParameterOverride("mylistId", result.Id));

                    vm.Name = result.Name;
                    vm.SelectedMylistSortKey = FastEnum.Parse<MylistSortKey>(result.DefaultSortKey + result.DefaultSortOrder, true);

                    MylistListItems.Add(vm);
                }

            } catch (StatusErrorException e) {

                Status = $"マイリストの一覧を取得出来ませんでした。 ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
            }

            // 子ViewModelのStatusを監視する
            MylistListItems.ToList().ForEach(vm => {

                vm.PropertyChanged += (o, e) => {

                    var tabItem = (TabItemViewModel)o;
                    if (e.PropertyName == nameof(Status)) {

                        Status = tabItem.Status;
                    }
                };
            });

            // 一番上をデフォルト値とする
            SelectedItem = MylistListItems.FirstOrDefault();
        }

        /// <summary>
        /// 再読み込み
        /// </summary>
        public void Reload() {

            Loaded();
        }

        public void CreateMylist() {

            ;
        }
        

        public override void KeyDown(KeyEventArgs e) {
        
            // Ctrl + F5でマイリストの一覧をリロードする
            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control) && e.Key == Key.F5) {

                Reload();
                return;
            }
            // Ctrl + F5以外は下位のViewModelに渡す            
            SelectedItem?.KeyDown(e);
        }
    }
}
