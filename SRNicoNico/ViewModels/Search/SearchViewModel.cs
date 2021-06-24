using System.Windows.Input;
using Livet;
using SRNicoNico.Services;
using Unity;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// 検索ページのViewModel
    /// </summary>
    public class SearchViewModel : TabItemViewModel {

        private ObservableSynchronizedCollection<TabItemViewModel> _SearchItems = new ObservableSynchronizedCollection<TabItemViewModel>();
        /// <summary>
        /// 検索のタブのリスト
        /// </summary>
        public ObservableSynchronizedCollection<TabItemViewModel> SearchItems {
            get { return _SearchItems; }
            set {
                if (_SearchItems == value)
                    return;
                _SearchItems = value;
                RaisePropertyChanged();
            }
        }

        private TabItemViewModel? _SelectedItem;
        /// <summary>
        /// 現在選択されている検索タブ
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
        private readonly ISearchService SearchService;


        public SearchViewModel(IUnityContainer unityContainer, ISearchService searchService) : base("検索") {

            UnityContainer = unityContainer;
            SearchService = searchService;
        }

        /// <summary>
        /// ランキングタブの一覧をインスタンス化する
        /// </summary>
        public void Loaded() {

        }

        public override void KeyDown(KeyEventArgs e) {
            SelectedItem?.KeyDown(e);
        }
    }
}
