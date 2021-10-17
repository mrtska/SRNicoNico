using System.Collections.Generic;
using System.Windows.Input;
using FastEnumUtility;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// 検索結果ページのViewModel
    /// </summary>
    public class SearchResultViewModel : PageSpinnerViewModel {

        /// <summary>
        /// ソートキーのリスト
        /// </summary>
        public static IEnumerable<SearchSortKey> SortKeyAll { get; } = FastEnum.GetValues<SearchSortKey>();

        private ObservableSynchronizedCollection<VideoListItem> _SearchResult = new ObservableSynchronizedCollection<VideoListItem>();
        /// <summary>
        /// 検索結果
        /// </summary>
        public ObservableSynchronizedCollection<VideoListItem> SearchResult {
            get { return _SearchResult; }
            set {
                if (_SearchResult == value)
                    return;
                _SearchResult = value;
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
                // ソート順が変わったのでジャンル情報は変えずに再検索する
                Research(true);
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
                // ソート順が変わったのでジャンル情報ごと再検索する
                Research(false);
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

        private ObservableSynchronizedCollection<SearchGenreFacet> _GenreFacetItems = new ObservableSynchronizedCollection<SearchGenreFacet>();
        /// <summary>
        /// ジャンル情報
        /// </summary>
        public ObservableSynchronizedCollection<SearchGenreFacet> GenreFacetItems {
            get { return _GenreFacetItems; }
            set {
                if (_GenreFacetItems == value)
                    return;
                _GenreFacetItems = value;
                RaisePropertyChanged();
            }
        }

        private SearchGenreFacet? _SelectedGenreFacet;
        /// <summary>
        /// 選択されたジャンル
        /// </summary>
        public SearchGenreFacet? SelectedGenreFacet {
            get { return _SelectedGenreFacet; }
            set { 
                if (_SelectedGenreFacet == value)
                    return;
                _SelectedGenreFacet = value;
                RaisePropertyChanged();
            }
        }

        private long _SearchTime = 0;
        /// <summary>
        /// 検索した時間
        /// </summary>
        public long SearchTime {
            get { return _SearchTime; }
            set { 
                if (_SearchTime == value)
                    return;
                _SearchTime = value;
                RaisePropertyChanged();
            }
        }

        private readonly ISearchService SearchService;

        public SearchResultViewModel(ISearchService searchService) : base(string.Empty, 30) {

            SearchService = searchService;
        }

        /// <summary>
        /// 検索
        /// </summary>
        public override void SpinPage(int page) {

            Search(page, true);
        }

        private async void Search(int page, bool bypassGenreFacetUpdate) {

            // 検索クエリが空だったら何もしない
            if (string.IsNullOrWhiteSpace(SearchQuery) || IsActive) {

                return;
            }

            try {
                base.SpinPage(page);

                Name = $"{SearchQuery} の{(SelectedSearchType == SearchType.Keyword ? "キーワード" : "タグ")}検索結果";

                SearchResult.Clear();

                IsActive = true;

                Status = "検索中";
                var result = await SearchService.SearchAsync(SelectedSortKey, SelectedSearchType, SearchQuery, page, SelectedGenreFacet?.Key);
                Total = result.TotalCount;
                SearchTime = result.Time;

                foreach (var item in result.Items) {

                    SearchResult.Add(item);
                }
                if (Total != 0 && !bypassGenreFacetUpdate) {

                    GenreFacetItems.Clear();
                    var items = await SearchService.GetGenreFacetAsync(SelectedSearchType, SearchQuery);
                    // WrapPanel上にTextBlockを無理やり入れるためにダミーデータを入れる
                    GenreFacetItems.Add(null!);
                    // すべてボタンを追加する
                    GenreFacetItems.Add(new SearchGenreFacet { Key = "all", Count = Total.Value, Label = "すべて", Time = result.Time });
                    foreach (var item in items) {
                        // 取得時間を上書きする
                        item.Time = result.Time;
                        GenreFacetItems.Add(item);
                    }
                }

                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"検索出来ませんでした。 ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
            }

        }

        /// <summary>
        /// 再検索
        /// </summary>
        public void Research(bool bypassGenreFacetUpdate) {

            Search(1, bypassGenreFacetUpdate);
        }

        public void Reload() {

            Research(false);
        }

        /// <summary>
        /// ジャンルを絞るボタンが押された時
        /// </summary>
        /// <param name="searchGenreFacet">ジャンル</param>
        public void FilterGenre(SearchGenreFacet searchGenreFacet) {

            // すべての時はnullにする
            if (searchGenreFacet.Key == "all") {
                SelectedGenreFacet = null;
            } else {
                SelectedGenreFacet = searchGenreFacet;
            }
            Research(true);
        }

        public void Close() {

            Dispose();
        }

        public override void KeyDown(KeyEventArgs e) {
        }
    }
}
