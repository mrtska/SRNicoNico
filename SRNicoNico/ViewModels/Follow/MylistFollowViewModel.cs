using System.Windows.Input;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// マイリストフォローページのViewModel
    /// </summary>
    public class MylistFollowViewModel : TabItemViewModel {

        /// <summary>
        /// フォローしているマイリストのリスト
        /// </summary>
        public ObservableSynchronizedCollection<MylistEntry> MylistItems { get; private set; }

        private readonly IUserService UserService;

        public MylistFollowViewModel(IUserService userService) : base("マイリスト") {

            UserService = userService;
            MylistItems = new ObservableSynchronizedCollection<MylistEntry>();
        }

        /// <summary>
        /// フォローしているマイリストを非同期で取得する
        /// </summary>
        public async void Loaded() {

            IsActive = true;
            Status = "フォローしているマイリストを取得中";
            MylistItems.Clear();
            try {

                await foreach (var entry in UserService.GetFollowedMylistsAsync()) {

                    MylistItems.Add(entry);
                }

                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"フォローしているマイリストを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
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
