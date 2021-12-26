using System.Collections.Generic;
using System.Windows.Input;
using FastEnumUtility;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// あとで見るページのViewModel
    /// </summary>
    public class WatchLaterViewModel : TabItemViewModel {

        private readonly IMylistService MylistService;

        /// <summary>
        /// 次のページがあるかどうか
        /// </summary>
        private bool HasNext;
        private int CurrentPage;

        private MylistSortKey _SelectedMylistSortKey = MylistSortKey.AddedAtDesc;
        /// <summary>
        /// あとで見るのソート順
        /// </summary>
        public MylistSortKey SelectedMylistSortKey {
            get { return _SelectedMylistSortKey; }
            set { 
                if (_SelectedMylistSortKey == value)
                    return;
                _SelectedMylistSortKey = value;
                RaisePropertyChanged();
                Reload();
            }
        }

        private int? _TotalCount;
        /// <summary>
        /// あとで見るに登録されている動画の総数
        /// </summary>
        public int? TotalCount {
            get { return _TotalCount; }
            set { 
                if (_TotalCount == value)
                    return;
                _TotalCount = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// あとで見るのリスト
        /// </summary>
        public ObservableSynchronizedCollection<MylistVideoItem> WatchLaterItems { get; private set; }

        /// <summary>
        /// マイリストのソート順のリスト
        /// </summary>
        public IEnumerable<MylistSortKey> SortKeyItems { get; } = FastEnum.GetValues<MylistSortKey>();

        public WatchLaterViewModel(IMylistService mylistService) : base("あとで見る") {

            MylistService = mylistService;
            WatchLaterItems = new ObservableSynchronizedCollection<MylistVideoItem>();
        }

        /// <summary>
        /// あとで見るを非同期で取得する
        /// </summary>
        public async void Loaded() {

            IsActive = true;
            Status = "あとで見るを取得中";
            WatchLaterItems.Clear();
            try {

                CurrentPage = 1;
                var result = await MylistService.GetWatchLaterAsync(SelectedMylistSortKey, CurrentPage);
                HasNext = result.HasNext;
                TotalCount = result.TotalCount;

                foreach (var entry in result.Entries) {

                    WatchLaterItems.Add(entry);
                }
                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"あとで見るを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
            }
        }

        /// <summary>
        /// あとで見るを更に読み込む
        /// </summary>
        public async void LoadMore() {

            // 次のページが無いか、ロード中の場合は無視
            if (!HasNext || IsActive) {
                return;
            }
            IsActive = true;
            Status = "あとで見るを取得中";
            try {

                // 次のページを取得する
                var result = await MylistService.GetWatchLaterAsync(SelectedMylistSortKey, ++CurrentPage);
                HasNext = result.HasNext;

                foreach (var entry in result.Entries) {

                    WatchLaterItems.Add(entry);
                }
                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"あとで見るを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
            }
        }

        /// <summary>
        /// あとで見るから削除する
        /// </summary>
        /// <param name="entry">削除したいあとで見るの動画情報</param>
        public async void DeleteWatchLater(MylistVideoItem entry) {

            IsActive = true;
            Status = "あとで見るから動画を削除中";
            try {

                var result = await MylistService.DeleteWatchLaterAsync(entry.ItemId);
                if (result) {

                    WatchLaterItems.Remove(entry);
                    TotalCount--;
                    Status = "動画を削除しました。";
                } else {

                    Status = "あとで見るから動画を削除出来ませんでした。";
                }
            } catch (StatusErrorException e) {

                Status = $"あとで見るから動画を削除出来ませんでした。 ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
            }
        }

        public void Reload() {

            Loaded();
        }

        public override void KeyDown(KeyEventArgs e) {

            if (e.Key == Key.F5) {
                Reload();
            }
        }
    }
}
