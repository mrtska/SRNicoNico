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

        public UserFollowViewModel(IUserService userService) : base("ユーザー") {

            UserService = userService;
            UserItems = new ObservableSynchronizedCollection<UserEntry>();
        }

        /// <summary>
        /// 視聴履歴を非同期で取得する
        /// </summary>
        public async void Loaded() {

            IsActive = true;
            Status = "フォローしているユーザーを取得中";
            UserItems.Clear();
            try {

                var result = await UserService.GetFollowedUsersAsync();

                foreach (var entry in result.Entries!) {

                    UserItems.Add(entry);
                }

                Status = "";
            } catch (StatusErrorException e) {

                Status = $"フォローしているユーザーを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
            }


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

        public override void SpinPage(int page) {
            throw new System.NotImplementedException();
        }
    }
}
