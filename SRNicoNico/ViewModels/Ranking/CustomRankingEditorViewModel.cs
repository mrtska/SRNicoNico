using System.Linq;
using Livet;
using Livet.Messaging.Windows;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// カスタムランキング設定編集画面のViewModel
    /// </summary>
    public class CustomRankingEditorViewModel : TabItemViewModel {

        private string _Title = string.Empty;
        /// <summary>
        /// カスタムランキング名
        /// </summary>
        public string Title {
            get { return _Title; }
            set { 
                if (_Title == value)
                    return;
                _Title = value;
                RaisePropertyChanged();
                Validate();
            }
        }

        private string _Tag = string.Empty;
        /// <summary>
        /// タグ
        /// </summary>
        public string Tag {
            get { return _Tag; }
            set { 
                if (_Tag == value)
                    return;
                _Tag = value;
                RaisePropertyChanged();
                Validate();

                // 空文字の場合はお気に入りタグをサジェストする
                if (string.IsNullOrEmpty(Tag)) {

                    SuggestFavoriteTags();
                    return;
                }
                // サジェストされた文字列と入力された値が一致している場合は再サジェストしない
                if (SuggestedTags.Any(s => s == Tag)) {

                    Setting!.Setting.GenreKeys = Enumerable.Empty<string>();
                    // タグが存在しているのでファセットを取得する
                    GetGenreFacet();
                    return;
                }

                SuggestTag();
            }
        }

        private string _ChannelVideoListingStatus = string.Empty;
        /// <summary>
        /// チャンネル動画を含むかどうか
        /// only(チャンネル動画), included(両方), excluded(ユーザー動画)のどれか
        /// </summary>
        public string ChannelVideoListingStatus {
            get { return _ChannelVideoListingStatus; }
            set { 
                if (_ChannelVideoListingStatus == value)
                    return;
                _ChannelVideoListingStatus = value;
                RaisePropertyChanged();
            }
        }

        private string? _RankingType = string.Empty;
        /// <summary>
        /// genreかtagかnull
        /// </summary>
        public string? RankingType {
            get { return _RankingType; }
            set { 
                if (_RankingType == value)
                    return;
                _RankingType = value;
                RaisePropertyChanged();
            }
        }

        private bool _IsAllGenre;
        /// <summary>
        /// 全ジャンル選択かどうか
        /// </summary>
        public bool IsAllGenre {
            get { return _IsAllGenre; }
            set { 
                if (_IsAllGenre == value)
                    return;
                _IsAllGenre = value;
                RaisePropertyChanged();
                Validate();
            }
        }

        private int _SelectedGenreCount;
        /// <summary>
        /// 選択されたジャンルの数
        /// </summary>
        public int SelectedGenreCount {
            get { return _SelectedGenreCount; }
            set { 
                if (_SelectedGenreCount == value)
                    return;
                _SelectedGenreCount = value;
                RaisePropertyChanged();
                Validate();
            }
        }

        private bool _CanSave;
        /// <summary>
        /// 保存可能かどうか
        /// バリデーションが引っ掛かっている場合はFalse
        /// </summary>
        public bool CanSave {
            get { return _CanSave; }
            set { 
                if (_CanSave == value)
                    return;
                _CanSave = value;
                RaisePropertyChanged();
            }
        }

        private ObservableSynchronizedCollection<CustomRankingEditorGenreViewModel> _Genres = new ObservableSynchronizedCollection<CustomRankingEditorGenreViewModel>();
        /// <summary>
        /// ジャンルのリスト
        /// </summary>
        public ObservableSynchronizedCollection<CustomRankingEditorGenreViewModel> Genres {
            get { return _Genres; }
            set { 
                if (_Genres == value)
                    return;
                _Genres = value;
                RaisePropertyChanged();
            }
        }

        private ObservableSynchronizedCollection<CustomRankingEditorGenreViewModel> _TagFacetGenres = new ObservableSynchronizedCollection<CustomRankingEditorGenreViewModel>();
        /// <summary>
        /// タグのジャンルのリスト
        /// </summary>
        public ObservableSynchronizedCollection<CustomRankingEditorGenreViewModel> TagFacetGenres {
            get { return _TagFacetGenres; }
            set {
                if (_TagFacetGenres == value)
                    return;
                _TagFacetGenres = value;
                RaisePropertyChanged();
            }
        }

        private ObservableSynchronizedCollection<string> _SuggestedTags = new ObservableSynchronizedCollection<string>();
        /// <summary>
        /// サジェストされたタグのリスト
        /// </summary>
        public ObservableSynchronizedCollection<string> SuggestedTags {
            get { return _SuggestedTags; }
            set {
                if (_SuggestedTags == value)
                    return;
                _SuggestedTags = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 設定が保存されたらTrueになる
        /// </summary>
        public bool Saved { get; private set; }

        private readonly IRankingService RankingService;
        private readonly ISearchService SearchService;
        private readonly int LaneId;

        private RankingSetting? Setting;

        public CustomRankingEditorViewModel(IRankingService rankingService, ISearchService searchService, int laneId) {

            RankingService = rankingService;
            SearchService = searchService;
            LaneId = laneId;
        }

        private void Validate() {

            if (string.IsNullOrEmpty(Title)) {
                CanSave = false;
                return;
            }
            if (RankingType == "genre") {
                if (!IsAllGenre) {
                    if (SelectedGenreCount == 0) {
                        CanSave = false;
                        return;
                    }
                }
            } else if (RankingType == "tag") {

                if (string.IsNullOrEmpty(Tag)) {
                    CanSave = false;
                    return;
                }
            }
            CanSave = true;
        }

        private async void SuggestTag() {

            try {
                var result = await SearchService.GetTagSuggestionAsync(Tag);
                SuggestedTags.Clear();
                foreach (var tag in result) {

                    SuggestedTags.Add(tag);
                }
            } catch (StatusErrorException e) {
                Status = $"タグの候補を取得出来ませんでした。 ステータスコード: {e.StatusCode}";
            }
        }

        private async void SuggestFavoriteTags() {

            try {
                var result = await SearchService.GetFavoriteTagsAsync();
                SuggestedTags.Clear();
                foreach (var tag in result) {

                    SuggestedTags.Add(tag);
                }
            } catch (StatusErrorException e) {
                Status = $"お気に入りタグを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
            }
        }

        private async void GetGenreFacet() {

            if (Setting == null) {
                return;
            }

            try {
                TagFacetGenres.Clear();
                IsActive = true;
                Status = "ジャンルファセットを取得中";
                var result = await SearchService.GetGenreFacetAsync(SearchType.Tag, Tag);

                foreach (var facet in result) {

                    TagFacetGenres.Add(new CustomRankingEditorGenreViewModel {
                        Key = facet.Key,
                        Value = facet.Label,
                        IsChecked = Setting.Setting.GenreKeys.Contains(facet.Key)
                    });
                }

                Status = string.Empty;

            } catch (StatusErrorException e) {
                Status = $"ジャンルファセットを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
            } finally {
                IsActive = false;
            }
        }

        /// <summary>
        /// ランキング設定を取得する
        /// </summary>
        public async void Loaded() {

            IsActive = true;
            Status = "ランキング設定を取得中";
            try {

                Initialize(await RankingService.GetCustomRankingSettingAsync(LaneId));

                Status = string.Empty;
            } catch (StatusErrorException e) {
                Status = $"ランキング設定を取得出来ませんでした。 ステータスコード: {e.StatusCode}";
                return;
            } finally {
                IsActive = false;
            }
        }

        /// <summary>
        /// 値を初期化する
        /// </summary>
        /// <param name="setting">取得した設定</param>
        private void Initialize(RankingSetting setting) {

            Genres.Clear();
            foreach (var genre in setting.GenreMap) {

                Genres.Add(new CustomRankingEditorGenreViewModel {
                    Key = genre.Key,
                    Value = genre.Value,
                    IsChecked = setting.Setting.GenreKeys.Contains(genre.Key)
                });
            }
            Setting = setting;
            Title = setting.Setting.IsDefault ? $"カスタムランキング{LaneId}" : setting.Setting.Title;
            ChannelVideoListingStatus = setting.Setting.ChannelVideoListingStatus;
            RankingType = setting.Setting.IsDefault ? null : setting.Setting.Type;
            IsAllGenre = setting.Setting.IsAllGenre;

            SelectedGenreCount = Genres.Count(c => c.IsChecked);
            
            // タグの場合はファセットを取得する
            if (setting.Setting.Type == "tag") {

                Tag = setting.Setting.Tags.FirstOrDefault();
                if (!string.IsNullOrEmpty(Tag)) {
                    GetGenreFacet();
                }
            } else {
                // リセット後にタグを選ばれた時にサジェストされるように取得しておく
                SuggestFavoriteTags();
            }
            // 一度バリデートしておく
            Validate();

            Genres.ToList().ForEach(x => {
                x.PropertyChanged += (o, e) => {
                    // チェックボックスが変更された時に選択されたジャンルを数える
                    if (e.PropertyName == nameof(CustomRankingEditorGenreViewModel.IsChecked)) {
                        SelectedGenreCount = Genres.Count(c => c.IsChecked);
                    }
                };
            });
        }

        /// <summary>
        /// 設定を保存する
        /// </summary>
        public async void Save() {

            if (RankingType == null || Setting == null) {
                return;
            }

            IsActive = true;
            Status = "ランキング設定を保存中";
            Setting.Setting.Title = Title;
            Setting.Setting.Type = RankingType;
            Setting.Setting.IsAllGenre = IsAllGenre;
            Setting.Setting.ChannelVideoListingStatus = ChannelVideoListingStatus;
            Setting.Setting.Tags = Enumerable.Repeat(Tag, 1);

            if (Setting.Setting.Type == "genre") {
                Setting.Setting.GenreKeys = Genres.Where(w => w.IsChecked).Select(s => s.Key);
            } else if (Setting.Setting.Type == "tag") {
                Setting.Setting.GenreKeys = TagFacetGenres.Where(w => w.IsChecked).Select(s => s.Key);
            }
            try {
                
                Initialize(await RankingService.SaveCustomRankingSettingAsync(Setting.Setting));
                Status = string.Empty;
                Saved = true;
                Messenger.Raise(new WindowActionMessage(WindowAction.Close));

            } catch (StatusErrorException e) {
                Status = $"ランキング設定を保存出来ませんでした。 ステータスコード: {e.StatusCode}";
                return;
            } finally {
                IsActive = false;
            }
        }

        /// <summary>
        /// 設定をリセットする
        /// </summary>
        public async void Reset() {

            IsActive = true;
            Status = "ランキング設定をリセット中";
            try {

                Initialize(await RankingService.ResetCustomRankingSettingAsync(LaneId));
                Saved = true;
                Status = string.Empty;
                Messenger.Raise(new WindowActionMessage(WindowAction.Close));

            } catch (StatusErrorException e) {
                Status = $"ランキング設定をリセット出来ませんでした。 ステータスコード: {e.StatusCode}";
                return;
            } finally {
                IsActive = false;
            }
        }

        public void Reload() {

            Loaded();
        }
    }

    public class CustomRankingEditorGenreViewModel : NotificationObject {

        private bool _IsChecked;
        /// <summary>
        /// 選択状態かどうか
        /// </summary>
        public bool IsChecked {
            get { return _IsChecked; }
            set { 
                if (_IsChecked == value)
                    return;
                _IsChecked = value;
                RaisePropertyChanged();
            }
        }

        private string _Key = string.Empty;
        /// <summary>
        /// ジャンルキー
        /// </summary>
        public string Key {
            get { return _Key; }
            set { 
                if (_Key == value)
                    return;
                _Key = value;
                RaisePropertyChanged();
            }
        }

        private string _Value = string.Empty;
        /// <summary>
        /// ジャンル名
        /// </summary>
        public string Value {
            get { return _Value; }
            set { 
                if (_Value == value)
                    return;
                _Value = value;
                RaisePropertyChanged();
            }
        }
    }
}
