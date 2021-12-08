using System.Windows.Input;
using Livet;
using Livet.Messaging;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;
using Unity;
using Unity.Resolution;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// カスタムランキングレーンページのViewModel
    /// </summary>
    public class CustomRankingItemViewModel : TabItemViewModel {

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

        public RankingSettingsEntry Settings { get; private set; }

        private readonly IUnityContainer UnityContainer;
        private readonly IRankingService RankingService;
        private readonly InteractionMessenger MainWindowMessanger;

        public CustomRankingItemViewModel(IUnityContainer container, IRankingService rankingService, RankingSettingsEntry entry, InteractionMessenger messenger) : base(entry.Title) {

            UnityContainer = container;
            RankingService = rankingService;
            Settings = entry;
            MainWindowMessanger = messenger;
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
                Name = details.Title;

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
        /// 編集UIを表示する
        /// </summary>
        public void OpenEditor() {

            using var vm = UnityContainer.Resolve<CustomRankingEditorViewModel>(new ParameterOverride("laneId", Settings.LaneId));
            vm.PropertyChanged += (o, e) => {

                if (e.PropertyName == nameof(Status)) {
                    Status = ((TabItemViewModel)o).Status;
                }
            };
            MainWindowMessanger.Raise(new TransitionMessage(typeof(Views.CustomRankingEditor), vm, TransitionMode.Modal));
            // 設定が保存されている場合は画面をリロードする
            if (vm.Saved) {

                Reload();
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
