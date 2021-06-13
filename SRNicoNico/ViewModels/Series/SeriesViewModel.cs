using System.Windows.Input;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// シリーズページのViewModel
    /// </summary>
    public class SeriesViewModel : TabItemViewModel {

        private Series? _Series;
        /// <summary>
        /// シリーズ情報
        /// </summary>
        public Series? Series {
            get { return _Series; }
            set { 
                if (_Series == value)
                    return;
                _Series = value;
                RaisePropertyChanged();
            }
        }

        private int _TotalCount;
        /// <summary>
        /// シリーズの中の動画数
        /// </summary>
        public int TotalCount {
            get { return _TotalCount; }
            set { 
                if (_TotalCount == value)
                    return;
                _TotalCount = value;
                RaisePropertyChanged();
            }
        }

        private readonly ISeriesService SeriesService;
        private readonly string SeriesId;

        public SeriesViewModel(ISeriesService seriesService, string seriesId) : base("シリーズ") {

            SeriesService = seriesService;
            SeriesId = seriesId;
        }

        /// <summary>
        /// シリーズを非同期で取得する
        /// </summary>
        public async void Loaded() {

            IsActive = true;
            Status = "シリーズを取得中";
            try {

                Series = await SeriesService.GetSeriesAsync(SeriesId);
                Name = "シリーズ\n" + Series.Title;
                TotalCount = Series.TotalCount;

                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"シリーズを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
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
