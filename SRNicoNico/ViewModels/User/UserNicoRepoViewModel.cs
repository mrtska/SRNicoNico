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
    /// ユーザーページのニコレポのViewModel
    /// </summary>
    public class UserNicoRepoViewModel : TabItemViewModel {

        private NicoRepoFilter _SelectedFilter = NicoRepoFilter.All;
        /// <summary>
        /// 現在選択されているフィルター
        /// </summary>
        public NicoRepoFilter SelectedFilter {
            get { return _SelectedFilter; }
            set {
                if (_SelectedFilter == value)
                    return;
                _SelectedFilter = value;
                Reload();
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// ニコレポのリスト
        /// </summary>
        public ObservableSynchronizedCollection<NicoRepoEntry> NicoRepoItems { get; private set; }

        /// <summary>
        /// フィルターのリスト
        /// </summary>
        public IEnumerable<NicoRepoFilter> FilterItems { get; private set; }

        /// <summary>
        /// 次のページがあるかどうか
        /// </summary>
        private bool HasNext;

        private readonly INicoRepoService NicoRepoService;
        private readonly string UserId;

        public UserNicoRepoViewModel(INicoRepoService nicoRepoService, string userId) : base("ニコレポ") {

            NicoRepoService = nicoRepoService;
            UserId = userId;

            NicoRepoItems = new ObservableSynchronizedCollection<NicoRepoEntry>();
            FilterItems = FastEnum.GetValues<NicoRepoFilter>();
        }

        public async void Loaded() {

            IsActive = true;
            Status = "ニコレポを取得中";
            NicoRepoItems.Clear();
            try {

                var result = await NicoRepoService.GetUserNicoRepoAsync(UserId, SelectedFilter);
                HasNext = result.HasNext;

                foreach (var entry in result.Entries!) {

                    NicoRepoItems.Add(entry);
                }

                Status = string.Empty;
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
                var result = await NicoRepoService.GetUserNicoRepoAsync(UserId, SelectedFilter, NicoRepoItems.Last().Id);
                HasNext = result.HasNext;

                foreach (var entry in result.Entries!) {

                    NicoRepoItems.Add(entry);
                }

                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"ニコレポを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
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
