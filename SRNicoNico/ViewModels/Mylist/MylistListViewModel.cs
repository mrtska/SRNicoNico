using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using FastEnumUtility;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// マイリストのリストページのViewModel
    /// </summary>
    public class MylistListViewModel : TabItemViewModel {

        private MylistSortKey _SelectedMylistSortKey = MylistSortKey.AddedAtDesc;
        /// <summary>
        /// マイリストのソート順
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
        /// マイリストに登録されている動画の総数
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

        private string? _Description;
        /// <summary>
        /// マイリスト説明文
        /// </summary>
        public string? Description {
            get { return _Description; }
            set { 
                if (_Description == value)
                    return;
                _Description = value;
                RaisePropertyChanged();
            }
        }

        private int _FollowerCount;

        public int FollowerCount {
            get { return _FollowerCount; }
            set { 
                if (_FollowerCount == value)
                    return;
                _FollowerCount = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// マイリストのリスト
        /// </summary>
        public ObservableSynchronizedCollection<MylistEntry> MylistItems { get; private set; }

        /// <summary>
        /// マイリストのソート順のリスト
        /// </summary>
        public IEnumerable<MylistSortKey> SortKeyItems { get; } = FastEnum.GetValues<MylistSortKey>();

        /// <summary>
        /// 次のページがあるかどうか
        /// </summary>
        private bool HasNext;
        private int CurrentPage;

        private readonly IMylistService MylistService;
        private readonly string MylistId;

        public MylistListViewModel(IMylistService mylistService, string mylistId) {

            MylistService = mylistService;
            MylistItems = new ObservableSynchronizedCollection<MylistEntry>();
            MylistId = mylistId;
        }

        /// <summary>
        /// マイリストを非同期で取得する
        /// </summary>
        public async void Loaded() {

            IsActive = true;
            Status = "マイリストを取得中";
            MylistItems.Clear();
            try {

                CurrentPage = 1;
                var result = await MylistService.GetMylistAsync(MylistId, SelectedMylistSortKey, CurrentPage);
                HasNext = result.HasNext;
                TotalCount = result.TotalItemCount;
                Description = result.Description;
                FollowerCount = result.FollowerCount;

                foreach (var entry in result.Entries!) {

                    MylistItems.Add(entry);
                }

                Status = "";
            } catch (StatusErrorException e) {

                Status = $"マイリストを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
            }
        }

        /// <summary>
        /// マイリストを更に読み込む
        /// </summary>
        public async void LoadMore() {

            // 次のページが無いか、ロード中の場合は無視
            if (!HasNext || IsActive) {
                return;
            }
            IsActive = true;
            Status = "マイリストを取得中";
            try {

                var result = await MylistService.GetMylistAsync(MylistId, SelectedMylistSortKey, ++CurrentPage);
                HasNext = result.HasNext;
                TotalCount = result.TotalItemCount;

                foreach (var entry in result.Entries!) {

                    MylistItems.Add(entry);
                }

                Status = "";
            } catch (StatusErrorException e) {

                Status = $"マイリストを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
            }
        }

        /// <summary>
        /// マイリストを削除する
        /// </summary>
        /// <param name="entry">削除したいマイリスト</param>
        public async void DeleteMylist(MylistEntry entry) {

            IsActive = true;
            Status = "マイリストを削除中";
            try {

                var result = await MylistService.DeleteMylistItemAsync(MylistId, entry.ItemId!);
                if (result) {

                    MylistItems.Remove(entry);
                    Status = "動画を削除しました。";
                } else {

                    Status = "マイリストから動画を削除出来ませんでした。";
                }
            } catch (StatusErrorException e) {

                Status = $"マイリストの削除に失敗しました。 ステータスコード: {e.StatusCode}";
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
