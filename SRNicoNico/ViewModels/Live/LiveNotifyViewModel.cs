using System.Threading;
using System.Windows.Input;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// 生放送通知ページのViewModel
    /// </summary>
    public class LiveNotifyViewModel : TabItemViewModel {

        /// <summary>
        /// 生放送のリスト
        /// </summary>
        public DispatcherCollection<OngoingLive> LiveItems { get; private set; }
        
        private readonly ILiveService LiveService;
        private readonly ISettings Settings;
        private readonly Timer Timer;

        public LiveNotifyViewModel(ILiveService liveService, ISettings settings) : base("生放送通知") {

            LiveService = liveService;
            Settings = settings;
            LiveItems = new DispatcherCollection<OngoingLive>(App.UIDispatcher);
            Loaded();

            Timer = new Timer(TimerCallback, null, settings.LiveNotifyRefreshInterval * 60 * 1000, settings.LiveNotifyRefreshInterval * 60 * 1000);
        }

        private void TimerCallback(object? _) {

            Reload();
        }

        /// <summary>
        /// タイマー設定を更新する
        /// </summary>
        public void UpdateInterval() {

            Timer.Change(Settings.LiveNotifyRefreshInterval * 60 * 1000, Settings.LiveNotifyRefreshInterval * 60 * 1000);
        }

        /// <summary>
        /// 生放送を非同期で取得する
        /// </summary>
        public async void Loaded() {

            IsActive = true;
            Status = "生放送を取得中";
            LiveItems.Clear();
            try {

                var result = await LiveService.GetOngoingLivesAsync();
                foreach (var entry in result) {

                    LiveItems.Add(entry);
                }
                Badge = LiveItems.Count == 0 ? null : (int?)LiveItems.Count;
                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"放送中の生放送を取得出来ませんでした。 ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
            }
        }

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
