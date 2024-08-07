using SRNicoNico.Services;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Models;
using System.Windows.Input;
using System.Collections.Generic;
using FastEnumUtility;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ユーザーページの投稿動画のViewModel
    /// </summary>
    public class UserVideoViewModel : PageSpinnerViewModel {

        private VideoList? _VideoList;
        /// <summary>
        /// 投稿動画情報
        /// </summary>
        public VideoList? VideoList {
            get { return _VideoList; }
            set { 
                if (_VideoList == value)
                    return;
                _VideoList = value;
                RaisePropertyChanged();
            }
        }

        private VideoSortKey _SelectedSortKey;
        /// <summary>
        /// 並び順
        /// </summary>
        public VideoSortKey SelectedSortKey {
            get { return _SelectedSortKey; }
            set { 
                if (_SelectedSortKey == value)
                    return;
                _SelectedSortKey = value;
                RaisePropertyChanged();
                Reload();
            }
        }
        /// <summary>
        /// ソートキーのリスト
        /// </summary>
        public IEnumerable<VideoSortKey> SortKeyItems { get; private set; }

        private readonly IUserService UserService;
        private readonly string UserId;

        public UserVideoViewModel(IUserService userService, string userId) : base("投稿動画", 100) {

            UserService = userService;
            UserId = userId;

            SortKeyItems = FastEnum.GetValues<VideoSortKey>();
        }

        public void Loaded() {

            SpinPage(1);
        }

        public override async void SpinPage(int page) {
            base.SpinPage(page);

            IsActive = true;
            Status = "投稿動画を取得中";
            try {
                VideoList = await UserService.GetUserVideosAsync(UserId, SelectedSortKey, page);
                Total = VideoList.TotalCount;
                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"投稿動画を取得出来ませんでした。 ステータスコード: {e.StatusCode}";
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
