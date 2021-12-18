using System.Windows.Input;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// タグフォローページのViewModel
    /// </summary>
    public class TagFollowViewModel : TabItemViewModel {

        /// <summary>
        /// フォローしているタグのリスト
        /// </summary>
        public ObservableSynchronizedCollection<TagEntry> TagItems { get; private set; }

        private readonly IUserService UserService;

        public TagFollowViewModel(IUserService userService) : base("タグ") {

            UserService = userService;
            TagItems = new ObservableSynchronizedCollection<TagEntry>();
        }

        /// <summary>
        /// フォローしているタグを非同期で取得する
        /// </summary>
        public async void Loaded() {

            IsActive = true;
            Status = "フォローしているタグを取得中";
            TagItems.Clear();
            try {

                await foreach (var entry in UserService.GetFollowedTagsAsync()) {

                    TagItems.Add(entry);
                }

                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"フォローしているタグを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
            }
        }

        /// <summary>
        /// フォロー解除
        /// </summary>
        public async void Unfollow(TagEntry tag) {

            IsActive = true;
            Status = "フォロー解除中";
            try {

                var result = await UserService.UnfollowTagAsync(tag.Name);
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
    }
}
