using System.Linq;
using System.Windows.Input;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;
using Unity;
using Unity.Resolution;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ランキングページのViewModel
    /// </summary>
    public class RankingViewModel : TabItemViewModel {

        private ObservableSynchronizedCollection<TabItemViewModel> _CustomRankingItems = new ObservableSynchronizedCollection<TabItemViewModel>();
        /// <summary>
        /// カスタムランキングのタブのリスト
        /// </summary>
        public ObservableSynchronizedCollection<TabItemViewModel> CustomRankingItems {
            get { return _CustomRankingItems; }
            set {
                if (_CustomRankingItems == value)
                    return;
                _CustomRankingItems = value;
                RaisePropertyChanged();
            }
        }

        private ObservableSynchronizedCollection<TabItemViewModel> _RankingItems = new ObservableSynchronizedCollection<TabItemViewModel>();
        /// <summary>
        /// ランキングのタブのリスト
        /// </summary>
        public ObservableSynchronizedCollection<TabItemViewModel> RankingItems {
            get { return _RankingItems; }
            set {
                if (_RankingItems == value)
                    return;
                _RankingItems = value;
                RaisePropertyChanged();
            }
        }

        private TabItemViewModel? _SelectedItem;
        /// <summary>
        /// 現在選択されているタブ デフォルトは1レーン目のランキング
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
        private readonly IRankingService RankingService;

        private RankingSettings? Settings;

        public RankingViewModel(IUnityContainer unityContainer, IRankingService rankingService) : base("ランキング") {

            UnityContainer = unityContainer;
            RankingService = rankingService;
        }

        /// <summary>
        /// ランキングタブの一覧をインスタンス化する
        /// </summary>
        public async void Loaded() {

            IsActive = true;
            Status = "ランキング設定を取得中";
            CustomRankingItems.Clear();
            RankingItems.Clear();
            try {
                Settings = await RankingService.GetCustomRankingSettingsAsync();

                foreach (var lane in Settings.Settings) {

                    if (lane == null) {
                        continue;
                    }
                    CustomRankingItems.Add(UnityContainer.Resolve<CustomRankingItemViewModel>(new ParameterOverride("entry", lane)));
                }

                RankingItems.Add(UnityContainer.Resolve<HotTopicRankingItemViewModel>());

                var visibility = await RankingService.GetRankingVisibilityAsync();

                foreach (var genre in Settings.GenreMap) {

                    if (visibility.ContainsKey(genre.Key)) {

                        if (visibility[genre.Key]) {

                            RankingItems.Add(UnityContainer.Resolve<RankingItemViewModel>(new ParameterOverride("genreKey", genre.Key), new ParameterOverride("label", genre.Value)));
                        }
                    } else {
                        // DBに設定が無い場合はその他とR18を除いたランキングを表示する
                        if (genre.Key != "other" && genre.Key != "r18") {

                            RankingItems.Add(UnityContainer.Resolve<RankingItemViewModel>(new ParameterOverride("genreKey", genre.Key), new ParameterOverride("label", genre.Value)));
                        }
                    }
                }
                Status = string.Empty;
            } catch (StatusErrorException e) {
                Status = $"ランキング設定を取得出来ませんでした。 ステータスコード: {e.StatusCode}";
                return;
            } finally {
                IsActive = false;
            }

            // 子ViewModelのStatusを監視する
            CustomRankingItems.ToList().ForEach(vm => {
                vm.PropertyChanged += (o, e) => {
                    var tabItem = (TabItemViewModel)o;
                    if (e.PropertyName == nameof(Status)) {
                        Status = tabItem.Status;
                    }
                };
            });
            RankingItems.ToList().ForEach(vm => {
                vm.PropertyChanged += (o, e) => {
                    var tabItem = (TabItemViewModel)o;
                    if (e.PropertyName == nameof(Status)) {
                        Status = tabItem.Status;
                    }
                };
            });

            // 1レーン目のランキングをデフォルト値とする
            SelectedItem = CustomRankingItems.First();
        }

        public void Reload() {

            Loaded();
        }

        public override void KeyDown(KeyEventArgs e) {
            SelectedItem?.KeyDown(e);
        }
    }
}
