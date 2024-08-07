using System.Linq;
using System.Windows.Input;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// チャンネルフォローページのViewModel
    /// </summary>
    public class ChannelFollowViewModel : TabItemViewModel {

        /// <summary>
        /// フォローしているチャンネルのリスト
        /// </summary>
        public ObservableSynchronizedCollection<ChannelEntry> ChannelItems { get; private set; }

        private readonly IUserService UserService;

        public ChannelFollowViewModel(IUserService userService) : base("チャンネル") {

            UserService = userService;
            ChannelItems = new ObservableSynchronizedCollection<ChannelEntry>();
        }

        /// <summary>
        /// フォローしているチャンネルを非同期で取得する
        /// </summary>
        public async void Loaded() {

            IsActive = true;
            Status = "フォローしているチャンネルを取得中";
            ChannelItems.Clear();
            try {

                await foreach (var entry in UserService.GetFollowedChannelsAsync()) {

                    ChannelItems.Add(entry);
                }

                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"フォローしているチャンネルを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
            }
        }

        /// <summary>
        /// 月額課金額を計算してステータスバーに表示する
        /// </summary>
        public void CalcMonthlyPayment() {

            Status = $"月額{ChannelItems.Where(w => w.IsJoining).Sum(s => s.Price):N0}円 (税抜:{ChannelItems.Where(w => w.IsJoining).Sum(s => s.BodyPrice):N0}円)";
        }

        /// <summary>
        /// 更新ボタンが押された時に実行される
        /// </summary>
        public void Reload() {

            // 単に再取得するだけ
            Loaded();
        }

        public override void KeyDown(KeyEventArgs e) {

            // F5で更新
            if (e.Key == Key.F5) {

                Reload();
                e.Handled = true;
            }
        }
    }
}
