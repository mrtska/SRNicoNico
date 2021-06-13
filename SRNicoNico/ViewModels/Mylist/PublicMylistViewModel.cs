using System.Collections.Generic;
using System.Windows.Input;
using FastEnumUtility;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// マイリストのリストページのViewModel
    /// </summary>
    public class PublicMylistViewModel : PageSpinnerViewModel {

        private MylistSortKey _SelectedMylistSortKey;
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

        private Mylist? _Mylist;

        public Mylist? Mylist {
            get { return _Mylist; }
            set { 
                if (_Mylist == value)
                    return;
                _Mylist = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// マイリストのソート順のリスト
        /// </summary>
        public IEnumerable<MylistSortKey> SortKeyItems { get; } = FastEnum.GetValues<MylistSortKey>();

        private readonly IMylistService MylistService;
        private readonly string MylistId;

        public PublicMylistViewModel(IMylistService mylistService, string mylistId) : base("公開マイリスト", 100) {

            MylistService = mylistService;
            MylistId = mylistId;
        }

        /// <summary>
        /// マイリストを非同期で取得する
        /// </summary>
        public void Loaded() {

            SpinPage(1);
        }

        public override async void SpinPage(int page) {
            base.SpinPage(page);

            IsActive = true;
            Status = "マイリストを取得中";
            try {

                Mylist = await MylistService.GetPublicMylistAsync(MylistId, SelectedMylistSortKey, page);
                Name = "公開マイリスト\n" + Mylist.Name;
                Total = Mylist.TotalItemCount;

                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"マイリストを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
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

        /// <summary>
        /// タブを閉じる
        /// </summary>
        public void Close() {

            Dispose();
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
