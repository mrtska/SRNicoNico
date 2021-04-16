using System.Collections.Generic;
using System.Windows.Input;
using FastEnumUtility;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ニコレポのリストページのViewModel
    /// </summary>
    public abstract class NicoRepoListViewModel : TabItemViewModel {

        /// <summary>
        /// ニコレポタイプ
        /// </summary>
        public abstract NicoRepoType NicoRepoType { get; }

        /// <summary>
        /// フィルター
        /// </summary>
        public abstract IEnumerable<NicoRepoFilter> FilterItems { get; }

        /// <summary>
        /// 全て有効
        /// </summary>
        public static IEnumerable<NicoRepoFilter> FilterAll { get; } = FastEnum.GetValues<NicoRepoFilter>();
        /// <summary>
        /// すべてのみ
        /// </summary>
        public static IEnumerable<NicoRepoFilter> FilterOnlyAll { get; } = new List<NicoRepoFilter> { NicoRepoFilter.All }.AsReadOnly();

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

        private readonly INicoRepoService NicoRepoService;

        public NicoRepoListViewModel(INicoRepoService nicorepoService, string tabName) : base(tabName) {

            NicoRepoService = nicorepoService;
            NicoRepoItems = new ObservableSynchronizedCollection<NicoRepoEntry>();
        }

        /// <summary>
        /// ニコレポを非同期で取得する
        /// </summary>
        public async void Loaded() {

            IsActive = true;
            Status = "ニコレポを取得中";
            NicoRepoItems.Clear();
            try {

                var result = await NicoRepoService.GetNicoRepoAsync(NicoRepoType, SelectedFilter);

                foreach (var entry in result.Entries!) {

                    NicoRepoItems.Add(entry);
                }

                Status = "";
            } catch (StatusErrorException e) {

                Status = $"フォローしているユーザーを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
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
