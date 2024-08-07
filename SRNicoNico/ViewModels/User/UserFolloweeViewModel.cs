using System.Windows.Input;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ユーザーページのフォローのViewModel
    /// </summary>
    public class UserFolloweeViewModel : TabItemViewModel {

        private ObservableSynchronizedCollection<UserFollowItem> _FollowItems = new ObservableSynchronizedCollection<UserFollowItem>();
        /// <summary>
        /// フォロー中のユーザーのリスト
        /// </summary>
        public ObservableSynchronizedCollection<UserFollowItem> FollowItems {
            get { return _FollowItems; }
            set { 
                if (_FollowItems == value)
                    return;
                _FollowItems = value;
                RaisePropertyChanged();
            }
        }

        private int _Followee;
        /// <summary>
        /// フォロー中のユーザー数
        /// </summary>
        public int Followee {
            get { return _Followee; }
            set { 
                if (_Followee == value)
                    return;
                _Followee = value;
                RaisePropertyChanged();
            }
        }

        private readonly IUserService UserService;
        private readonly string UserId;

        /// <summary>
        /// 次のページがあるかどうか
        /// </summary>
        private bool HasNext;
        /// <summary>
        /// 次のページがある場合に指定するパラメータ
        /// </summary>
        private string? NextCursor;

        public UserFolloweeViewModel(IUserService userService, string userId) : base("フォロー中") {

            UserService = userService;
            UserId = userId;
        }

        public async void Loaded() {

            IsActive = true;
            Status = "フォロー中のユーザーを取得中";
            FollowItems.Clear();
            try {
                var result = await UserService.GetUserFollowingAsync(UserId);
                Followee = result.Followees;
                HasNext = result.HasNext;
                NextCursor = result.Cursor;

                foreach (var item in result.Items!) {

                    FollowItems.Add(item);
                }

                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"フォロー中のユーザーを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
            }
        }

        /// <summary>
        /// 更に読み込む
        /// </summary>
        public async void LoadMore() {

            // 次のページが無いか、ロード中の場合は無視
            if (!HasNext || IsActive) {
                return;
            }
            IsActive = true;
            Status = "フォロー中のユーザーを取得中";
            try {
                var result = await UserService.GetUserFollowingAsync(UserId, NextCursor);
                HasNext = result.HasNext;
                NextCursor = result.Cursor;

                foreach (var item in result.Items!) {

                    FollowItems.Add(item);
                }

                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"フォロー中のユーザーを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
            }
        }

        /// <summary>
        /// 再読み込み
        /// </summary>
        public void Reload() {

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
