using System.Collections.Generic;
using System.Linq;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// 設定ページの一般タブのViewModel
    /// </summary>
    public class SettingsRankingViewModel : TabItemViewModel {

        private ObservableSynchronizedCollection<SettingsRankingGenreViewModel> _Genres = new ObservableSynchronizedCollection<SettingsRankingGenreViewModel>();
        /// <summary>
        /// ジャンルのリスト
        /// </summary>
        public ObservableSynchronizedCollection<SettingsRankingGenreViewModel> Genres {
            get { return _Genres; }
            set { 
                if (_Genres == value)
                    return;
                _Genres = value;
                RaisePropertyChanged();
            }
        }

        private readonly IRankingService RankingService;

        public SettingsRankingViewModel(IRankingService rankingService) : base("ランキング設定") {

            RankingService = rankingService;
        }

        public async void Loaded() {

            var visibilities = await RankingService.GetRankingVisibilityAsync();
            var genreMap = await RankingService.GetGenresAsync();
            // R18はジャンル一覧に載っていないので手動で追加する
            genreMap["r18"] = "R-18";

            foreach (var genre in genreMap) {
                // DBに設定が無い時はデータを作成する
                if (!visibilities.ContainsKey(genre.Key)) {
                    // その他とR18はデフォルトでは非表示とする
                    if (genre.Key == "other" || genre.Key == "r18") {
                        await RankingService.SaveRankingVisibilityAsync(genre.Key, false);
                        visibilities[genre.Key] = false;
                    } else {
                        await RankingService.SaveRankingVisibilityAsync(genre.Key, true);
                        visibilities[genre.Key] = true;
                    }
                }

                var vm = new SettingsRankingGenreViewModel {
                    Key = genre.Key,
                    Label = genre.Value,
                    IsChecked = visibilities[genre.Key]
                };
                vm.PropertyChanged += async (s, e) => {

                    if (e.PropertyName == nameof(SettingsRankingGenreViewModel.IsChecked)) {

                        await RankingService.SaveRankingVisibilityAsync(vm.Key, vm.IsChecked);
                    }
                };

                Genres.Add(vm);
            }
        }
    }
    public class SettingsRankingGenreViewModel : NotificationObject {

        private string _Key = default!;
        /// <summary>
        /// ジャンルのキー名
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

        private string _Label = default!;
        /// <summary>
        /// ジャンル名
        /// </summary>
        public string Label {
            get { return _Label; }
            set {
                if (_Label == value)
                    return;
                _Label = value;
                RaisePropertyChanged();
            }
        }

        private bool _IsChecked;
        /// <summary>
        /// チェックされたかどうか
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

        internal SettingsRankingGenreViewModel() { }
    }
}
