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

        /// <summary>
        /// マイリストのリスト
        /// </summary>
        public ObservableSynchronizedCollection<WatchLaterEntry> MylistItems { get; private set; }

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

        public MylistListViewModel(IMylistService mylistService) {

            MylistService = mylistService;
            MylistItems = new ObservableSynchronizedCollection<WatchLaterEntry>();
        }

        /// <summary>
        /// ニコレポを非同期で取得する
        /// </summary>
        public async void Loaded() {

            IsActive = true;
            Status = "マイリストを取得中";
            MylistItems.Clear();
            try {

                //var result = await MylistService.GetMylistsAsync();


                Status = "";
            } catch (StatusErrorException e) {

                Status = $"ニコレポを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
            }
        }

        /// <summary>
        /// ニコレポを更に読み込む
        /// </summary>
        public async void LoadMore() {

            // 次のページが無いか、ロード中の場合は無視
            if (!HasNext || IsActive) {
                return;
            }
            IsActive = true;
            Status = "ニコレポを取得中";
            try {

                // 最後のニコレポのIDから後ろを取得する
                //var result = await NicoRepoService.GetNicoRepoAsync(NicoRepoType, SelectedFilter, NicoRepoItems.Last().Id);
                //HasNext = result.HasNext;


                Status = "";
            } catch (StatusErrorException e) {

                Status = $"ニコレポを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
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
