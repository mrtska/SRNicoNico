using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// 話題ランキングページのViewModel
    /// </summary>
    public class HotTopicRankingItemViewModel : TabItemViewModel {

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

        /// <summary>
        /// 集計期間のリスト
        /// </summary>
        public IEnumerable<RankingTerm> Terms { get; } = new List<RankingTerm> { RankingTerm.Hour, RankingTerm.Day }.AsReadOnly();

        private RankingTerm _SelectedTerm = RankingTerm.Day;
        /// <summary>
        /// 選択された集計期間
        /// デフォルト：24時間
        /// </summary>
        public RankingTerm SelectedTerm {
            get { return _SelectedTerm; }
            set {
                if (_SelectedTerm == value)
                    return;
                _SelectedTerm = value;
                RaisePropertyChanged();
                // 集計期間が変更されたのでリロードする
                Reload();
            }
        }

        private string _Key = "all";
        /// <summary>
        /// 話題のジャンルのキー
        /// </summary>
        public string Key {
            get { return _Key; }
            set { 
                if (_Key == value)
                    return;
                _Key = value;
                RaisePropertyChanged();

                Reload();
            }
        }

        private HotTopics? _HotTopics;
        /// <summary>
        /// 話題のジャンル情報
        /// </summary>
        public HotTopics? HotTopics {
            get { return _HotTopics; }
            set { 
                if (_HotTopics == value)
                    return;
                _HotTopics = value;
                RaisePropertyChanged();
            }
        }

        private string _Label = "話題 ランキング";
        /// <summary>
        /// ラベル
        /// </summary>
        public string Label {
            get { return _Label; }
            private set {
                if (_Label == value)
                    return;
                _Label = value;
                RaisePropertyChanged();
            }
        }

        private readonly IRankingService RankingService;
        private int CurrentPage = 1;
        private bool HasNext = false;

        public HotTopicRankingItemViewModel(IRankingService rankingService) : base("話題") {

            RankingService = rankingService;
        }

        /// <summary>
        /// ランキングタブの一覧をインスタンス化する
        /// </summary>
        public async void Loaded() {

            IsActive = true;
            Status = "話題ランキングを取得中";

            Ranking.Clear();
            try {
                CurrentPage = 1;
                var details = await RankingService.GetHotTopicRankingAsync(SelectedTerm, Key);
                if (details == null) {

                    Status = "話題ランキングの取得に失敗しました";
                    return;
                }
                HasNext = details.HasNext;

                foreach (var video in details.VideoList) {

                    Ranking.Add(video);
                }

            } catch (StatusErrorException e) {
                Status = $"話題ランキングを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
                return;
            } finally {
                IsActive = false;
            }

            IsActive = true;
            Status = "話題のジャンルを取得中";
            try {
                HotTopics = await RankingService.GetHotTopicsAsync();
                if (Key == "all") {
                    Label = "話題 ランキング";
                } else {
                    Label = $"話題 ランキング：{HotTopics.Items.Single(s => s.Key == Key).Label}";
                }

                Status = string.Empty;

            } catch (StatusErrorException e) {
                Status = $"話題のジャンルを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
                return;
            } finally {
                IsActive = false;
            }
        }

        /// <summary>
        /// 次のページがあればロードする
        /// </summary>
        public async void LoadMore() {

            if (!HasNext) {
                return;
            }

            IsActive = true;
            Status = "話題ランキングを取得中";
            try {
                var details = await RankingService.GetHotTopicRankingAsync(SelectedTerm, Key, ++CurrentPage);
                if (details == null) {

                    Status = "話題ランキングの取得に失敗しました";
                    return;
                }
                HasNext = details.HasNext;

                foreach (var video in details.VideoList) {

                    Ranking.Add(video);
                }
                Status = string.Empty;

            } catch (StatusErrorException e) {
                Status = $"話題ランキングを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
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

            if (e.Key == System.Windows.Input.Key.F5) {

                Reload();
            }
        }
    }
}
