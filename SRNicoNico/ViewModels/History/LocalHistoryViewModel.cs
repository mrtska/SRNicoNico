using System.Windows.Input;
using Livet;
using SRNicoNico.Entities;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ローカル視聴履歴ページのViewModel
    /// </summary>
    public class LocalHistoryViewModel : TabItemViewModel {

        /// <summary>
        /// ローカルの視聴履歴のリスト
        /// </summary>
        public ObservableSynchronizedCollection<LocalHistory> HistoryItems { get; private set; }

        private readonly IHistoryService HistoryService;

        public LocalHistoryViewModel(IHistoryService historyService) : base("ローカル") {

            HistoryService = historyService;
            HistoryItems = new ObservableSynchronizedCollection<LocalHistory>();
        }

        /// <summary>
        /// 視聴履歴を非同期で取得する
        /// </summary>
        public async void Loaded() {

            IsActive = true;
            Status = "ローカルの視聴履歴を取得中";
            HistoryItems.Clear();

            // 視聴履歴を取得する
            await foreach (var entry in HistoryService.GetLocalHistoryAsync()) {

                HistoryItems.Add(entry);
            }

            Status = string.Empty;
            IsActive = false;
        }

        /// <summary>
        /// ローカルの視聴履歴を削除する
        /// </summary>
        /// <param name="videoId">削除したい動画</param>
        public async void DeleteLocalHistory(LocalHistory entry) {

            Status = $"{entry.VideoId}の視聴履歴を削除中";

            if (await HistoryService.DeleteLocalHistoryAsync(entry.VideoId!)) {

                HistoryItems.Remove(entry);
                Status = $"{entry.VideoId}の視聴履歴を削除しました";
            } else {

                Status = $"{entry.VideoId}の視聴履歴の削除に失敗しました";
            }
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
