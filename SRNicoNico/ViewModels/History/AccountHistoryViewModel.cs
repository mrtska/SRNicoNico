using System.Windows.Input;
using Livet;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// アカウント視聴履歴ページのViewModel
    /// </summary>
    public class AccountHistoryViewModel : TabItemViewModel {

        /// <summary>
        /// アカウントの視聴履歴のリスト
        /// </summary>
        public ObservableSynchronizedCollection<HistoryEntry> HistoryItems { get; private set; }

        private readonly IHistoryService HistoryService;
        //private readonly IMylistService MylistService;

        public AccountHistoryViewModel(IHistoryService historyService) : base("アカウント") {

            HistoryService = historyService;
            HistoryItems = new ObservableSynchronizedCollection<HistoryEntry>();
        }

        /// <summary>
        /// 視聴履歴を非同期で取得する
        /// </summary>
        public async void Loaded() {

            IsActive = true;
            Status = "アカウントの視聴履歴を取得中";
            HistoryItems.Clear();

            // 視聴履歴を取得する
            await foreach (var entry in HistoryService.GetAccountHistoryAsync()) {

                HistoryItems.Add(entry);
            }

            Status = "";
            IsActive = false;
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
