using System.Windows.Input;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ユーザーページのシリーズのViewModel
    /// </summary>
    public class UserSeriesViewModel : PageSpinnerViewModel {

        private SeriesList? _SeriesList;
        /// <summary>
        /// シリーズ情報
        /// </summary>
        public SeriesList? SeriesList {
            get { return _SeriesList; }
            set {
                if (_SeriesList == value)
                    return;
                _SeriesList = value;
                RaisePropertyChanged();
            }
        }

        private readonly ISeriesService SeriesService;
        private readonly string UserId;

        public UserSeriesViewModel(ISeriesService seriesService, string userId) : base("シリーズ", 100) {

            SeriesService = seriesService;
            UserId = userId;
        }

        public void Loaded() {

            SpinPage(1);
        }

        public override async void SpinPage(int page) {
            base.SpinPage(page);

            IsActive = true;
            Status = "シリーズを取得中";
            try {
                SeriesList = await SeriesService.GetUserSeriesAsync(UserId, page);
                Total = SeriesList.TotalCount;

                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"シリーズを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
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
