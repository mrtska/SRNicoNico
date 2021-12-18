using System.Windows.Input;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ユーザーフォローページのViewModel
    /// </summary>
    public class UserFollowViewModel : PageSpinnerViewModel {

        /// <summary>
        /// フォローしているユーザーのリスト
        /// </summary>
        public ObservableSynchronizedCollection<UserEntry> UserItems { get; private set; }

        private readonly IUserService UserService;

        public UserFollowViewModel(IUserService userService) : base("ユーザー", 100) {

            UserService = userService;
            UserItems = new ObservableSynchronizedCollection<UserEntry>();
        }

        /// <summary>
        /// フォローしているユーザーを非同期で取得する
        /// </summary>
        public void Loaded() {

            SpinPage(1);
        }

        public async void Unfollow(UserEntry user) {

            IsActive = true;
            Status = "フォロー解除中";
            try {

                var result = await UserService.UnfollowUserAsync(user.Id);
                if (result) {

                    Reload();
                } else {

                    Status = "フォロー解除に失敗しました";
                }

                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"フォローを解除出来ませんでした。 ステータスコード: {e.StatusCode}";
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

        public override async void SpinPage(int page) {
            base.SpinPage(page);

            IsActive = true;
            Status = "フォローしているユーザーを取得中";
            UserItems.Clear();
            try {

                var result = await UserService.GetFollowedUsersAsync(page);
                Total = result.Total;

                foreach (var entry in result.Entries) {

                    UserItems.Add(entry);
                }

                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"フォローしているユーザーを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
            }
        }
    }
}
