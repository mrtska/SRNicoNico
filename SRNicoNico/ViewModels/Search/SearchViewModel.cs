using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using FastEnumUtility;
using Livet;
using SRNicoNico.Services;
using Unity;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// 検索ページのViewModel
    /// </summary>
    public class SearchViewModel : TabItemViewModel {

        /// <summary>
        /// ソートキーのリスト
        /// </summary>
        public IEnumerable<SearchSortKey> SortKeyAll { get; } = FastEnum.GetValues<SearchSortKey>();

        private ObservableSynchronizedCollection<SearchResultViewModel> _SearchItems = new ObservableSynchronizedCollection<SearchResultViewModel>();
        /// <summary>
        /// 検索のタブのリスト
        /// </summary>
        public ObservableSynchronizedCollection<SearchResultViewModel> SearchItems {
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

        private SearchSortKey _SelectedSortKey;
        /// <summary>
        /// 選択しているソート順
        /// </summary>
        public SearchSortKey SelectedSortKey {
            get { return _SelectedSortKey; }
            set { 
                if (_SelectedSortKey == value)
                    return;
                _SelectedSortKey = value;
                RaisePropertyChanged();
            }
        }

        private SearchType _SelectedSearchType;
        /// <summary>
        /// 検索タイプ
        /// </summary>
        public SearchType SelectedSearchType {
            get { return _SelectedSearchType; }
            set { 
                if (_SelectedSearchType == value)
                    return;
                _SelectedSearchType = value;
                RaisePropertyChanged();
            }
        }

        private string _SearchQuery = string.Empty;
        /// <summary>
        /// 検索ワード
        /// </summary>
        public string SearchQuery {
            get { return _SearchQuery; }
            set { 
                if (_SearchQuery == value)
                    return;
                _SearchQuery = value;
                RaisePropertyChanged();
            }
        }

        private readonly IUnityContainer UnityContainer;

        public SearchViewModel(IUnityContainer unityContainer) : base("検索") {

            UnityContainer = unityContainer;
        }

        /// <summary>
        /// 検索する
        /// </summary>
        public void Search() {

            if (string.IsNullOrWhiteSpace(SearchQuery)) {
                return;
            }

            var vm = UnityContainer.Resolve<SearchResultViewModel>();
            // 値をコピーする
            vm.SearchQuery = SearchQuery;
            vm.SelectedSearchType = SelectedSearchType;
            vm.SelectedSortKey = SelectedSortKey;

            // ステータスプロパティが更新されたら動画タブのステータスに反映させる
            vm.PropertyChanged += OnPropertyChanged;
            // 検索タブがDisposeされたら自動的にリストから削除する
            vm.CompositeDisposable.Add(() => {

                vm.PropertyChanged -= OnPropertyChanged;
                SearchItems.Remove(vm);

                // タブの一番後ろを選択状態にする
                SelectedItem = SearchItems.LastOrDefault();
            });

            SearchItems.Add(vm);
            SelectedItem = vm;

            vm.Reload();
        }

        public override void KeyDown(KeyEventArgs e) {
            SelectedItem?.KeyDown(e);
        }

        private void OnPropertyChanged(object o, PropertyChangedEventArgs e) {

            var tabItem = (TabItemViewModel)o;
            if (e.PropertyName == nameof(Status)) {

                Status = tabItem.Status;
            }
        }
    }
}
