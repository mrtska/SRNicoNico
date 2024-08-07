using System.Linq;
using System.Windows.Input;
using Livet;
using Unity;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// その他ページのViewModel
    /// </summary>
    public class OtherViewModel : TabItemViewModel {

        private ObservableSynchronizedCollection<TabItemViewModel> _OtherItems = new ObservableSynchronizedCollection<TabItemViewModel>();
        /// <summary>
        /// その他のタブのリスト
        /// </summary>
        public ObservableSynchronizedCollection<TabItemViewModel> OtherItems {
            get { return _OtherItems; }
            set {
                if (_OtherItems == value)
                    return;
                _OtherItems = value;
                RaisePropertyChanged();
            }
        }

        private TabItemViewModel? _SelectedItem;
        /// <summary>
        /// 現在選択されているタブ デフォルトは概要ページ
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

        public OtherViewModel(IUnityContainer unityContainer) : base("その他") {

            UnityContainer = unityContainer;
        }

        /// <summary>
        /// その他タブの一覧をインスタンス化する
        /// </summary>
        public void Loaded() {

            OtherItems.Add(UnityContainer.Resolve<OverviewViewModel>());
            OtherItems.Add(UnityContainer.Resolve<PrivacyPolicyViewModel>());
            OtherItems.Add(UnityContainer.Resolve<OpenSourceViewModel>());

            // 子ViewModelのStatusを監視する
            OtherItems.ToList().ForEach(vm => {

                vm.PropertyChanged += (o, e) => {

                    var tabItem = (TabItemViewModel)o;
                    if (e.PropertyName == nameof(Status)) {

                        Status = tabItem.Status;
                    }
                };
            });

            // アプリ概要をデフォルト値とする
            SelectedItem = OtherItems.First();
        }

        public override void KeyDown(KeyEventArgs e) {
            SelectedItem?.KeyDown(e);
        }
    }
}
