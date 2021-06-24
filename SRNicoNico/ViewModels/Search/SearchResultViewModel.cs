using System.Windows.Input;
using Livet;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// 検索結果ページのViewModel
    /// </summary>
    public class SearchResultViewModel : TabItemViewModel {

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

        private readonly IRankingService RankingService;


        public SearchResultViewModel(IRankingService rankingService) : base("") {

            RankingService = rankingService;
        }

        /// <summary>
        /// 検索結果
        /// </summary>
        public async void Loaded() {

        }

        /// <summary>
        /// 再読み込み
        /// </summary>
        public void Reload() {

            Loaded();
        }

        public override void KeyDown(KeyEventArgs e) {
        }
    }
}
