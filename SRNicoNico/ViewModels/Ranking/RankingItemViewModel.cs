using System.Windows.Input;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ランキングレーンページのViewModel
    /// </summary>
    public class RankingItemViewModel : TabItemViewModel {

        private ObservableSynchronizedCollection<VideoItem> _Ranking = new ObservableSynchronizedCollection<VideoItem>();
        /// <summary>
        /// ランキング
        /// </summary>
        public ObservableSynchronizedCollection<VideoItem> Ranking {
            get { return _Ranking; }
            set {
                if (_Ranking == value)
                    return;
                _Ranking = value;
                RaisePropertyChanged();
            }
        }

        private readonly IRankingService RankingService;

        public RankingSettingsEntry Settings { get; private set; }

        public RankingItemViewModel(IRankingService rankingService, RankingSettingsEntry entry) : base(entry.Title!) {

            RankingService = rankingService;
            Settings = entry;
        }

        /// <summary>
        /// ランキングタブの一覧をインスタンス化する
        /// </summary>
        public async void Loaded() {

            IsActive = true;
            Status = "ランキングを取得中";
            Ranking.Clear();
            try {
                var details = await RankingService.GetCustomRankingAsync(Settings.LaneId);

                foreach (var video in details.VideoList!) {

                    Ranking.Add(video);
                }
                Status = string.Empty;

            } catch (StatusErrorException e) {
                Status = $"ランキングを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
                return;
            } finally {
                IsActive = false;
            }
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
