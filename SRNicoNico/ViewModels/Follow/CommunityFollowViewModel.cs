using System.Windows.Input;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// コミュニティフォローページのViewModel
    /// </summary>
    public class CommunityFollowViewModel : PageSpinnerViewModel {

        /// <summary>
        /// フォローしているコミュニティのリスト
        /// </summary>
        public ObservableSynchronizedCollection<CommunityEntry> CommunityItems { get; private set; }

        private readonly IUserService UserService;

        public CommunityFollowViewModel(IUserService userService) : base("コミュニティ", 10) {

            UserService = userService;
            CommunityItems = new ObservableSynchronizedCollection<CommunityEntry>();
        }

        /// <summary>
        /// フォローしているコミュニティを非同期で取得する
        /// </summary>
        public void Loaded() {

            SpinPage(1);
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
            Status = "フォローしているコミュニティを取得中";
            CommunityItems.Clear();
            try {

                var result = await UserService.GetFollowedCommunitiesAsync(page, 10);
                Total = result.Total;

                foreach (var entry in result.Entries!) {

                    CommunityItems.Add(entry);
                }

                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"フォローしているコミュニティを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
            }
        }
    }
}
