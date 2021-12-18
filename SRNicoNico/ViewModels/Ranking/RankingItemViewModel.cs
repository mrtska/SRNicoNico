using System.Collections.Generic;
using System.Windows.Input;
using FastEnumUtility;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ジャンルランキングページのViewModel
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

        private IEnumerable<RankingTerm> _Terms = AllTerms;
        /// <summary>
        /// 集計期間のリスト
        /// </summary>
        public IEnumerable<RankingTerm> Terms {
            get { return _Terms; }
            set { 
                if (_Terms == value)
                    return;
                _Terms = value;
                RaisePropertyChanged();
            }
        }

        private static readonly IEnumerable<RankingTerm> AllTerms = FastEnum.GetValues<RankingTerm>();
        private static readonly IEnumerable<RankingTerm> TagUsedTerms = new List<RankingTerm> { RankingTerm.Hour, RankingTerm.Day }.AsReadOnly();

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

        private PopularTags? _PopularTags;
        /// <summary>
        /// 人気のタグ情報
        /// </summary>
        public PopularTags? PopularTags {
            get { return _PopularTags; }
            set { 
                if (_PopularTags == value)
                    return;
                _PopularTags = value;
                RaisePropertyChanged();
            }
        }


        private string? _Tag;
        /// <summary>
        /// 人気のタグで絞る場合
        /// </summary>
        public string? Tag {
            get { return _Tag; }
            set { 
                if (_Tag == value)
                    return;
                // すべての場合はnullにする
                if (value == "すべて") {
                    value = null;
                    Terms = AllTerms;
                } else {
                    // タグで絞った場合は毎時と24時間ランキングしか使えないのでコンボボックスを絞る
                    Terms = TagUsedTerms;
                }
                // 集計期間をリセットしてリロードする
                _SelectedTerm = RankingTerm.Day;
                RaisePropertyChanged(nameof(SelectedTerm));
                _Tag = value;
                RaisePropertyChanged();
                Reload();
            }
        }

        /// <summary>
        /// ジャンルキー
        /// </summary>
        public string GenreKey { get; private set; }

        private string _Label = string.Empty;
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

        public RankingItemViewModel(IRankingService rankingService, string genreKey, string label) : base(label) {

            RankingService = rankingService;
            GenreKey = genreKey;
            Label = $"{Name} ランキング";
        }

        /// <summary>
        /// ランキングタブの一覧をインスタンス化する
        /// </summary>
        public async void Loaded() {

            IsActive = true;
            Status = "ランキングを取得中";
            Ranking.Clear();
            try {
                var details = await RankingService.GetRankingAsync(SelectedTerm, GenreKey, Tag);
                if (details == null) {

                    Status = "ランキングの取得に失敗しました";
                    return;
                }
                HasNext = details.HasNext;

                foreach (var video in details.VideoList) {

                    Ranking.Add(video);
                }

            } catch (StatusErrorException e) {
                Status = $"ランキングを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
                return;
            } finally {
                IsActive = false;
            }

            if (Tag == null) {
                Label = $"{Name} ランキング";
            } else {
                Label = $"{Name} ランキング：{Tag}";
            }

            // 全ジャンルに人気のタグは無いのでスキップする
            if (GenreKey == "all") {
                return;
            }

            try {
                PopularTags = await RankingService.GetPopularTagsAsync(GenreKey);
                Status = string.Empty;

            } catch (StatusErrorException e) {
                Status = $"人気のタグを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
                return;
            } finally {
                IsActive = false;
            }
        }

        /// <summary>
        /// 次のページがあればロードする
        /// </summary>
        public async void LoadMore() {

            if (!HasNext || IsActive) {
                return;
            }

            IsActive = true;
            Status = "ランキングを取得中";
            try {
                var details = await RankingService.GetRankingAsync(SelectedTerm, GenreKey, Tag, ++CurrentPage);
                if (details == null) {

                    Status = "ランキングの取得に失敗しました";
                    return;
                }
                HasNext = details.HasNext;

                foreach (var video in details.VideoList) {

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

            if (e.Key == Key.F5) {

                Reload();
            }
        }
    }
}
