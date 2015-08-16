using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Threading;

using Livet;



using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {

	public class VideoViewModel : ViewModel {


		private static GeometryGroup Pause;
		private static GeometryGroup Playing;

		//パスで書いた一時停止ボタンと再生ボタン もっといい方法があるはず・・・
		static VideoViewModel() {

			RectangleGeometry rect1 = new RectangleGeometry();
			rect1.Rect = new Rect(-5, 0, 5, 20);

			RectangleGeometry rect2 = new RectangleGeometry();
			rect2.Rect = new Rect(5, 0, 5, 20);

			Pause = new GeometryGroup();
			Pause.Children.Add(rect1);
			Pause.Children.Add(rect2);

			Geometry tri = Geometry.Parse("M 5,0 l 0,10 L 11,5");

			Playing = new GeometryGroup();
			Playing.Children.Add(tri);

		}


		//キャッシュパス
		public string Path { get; set; }



		//ストリーミング
		public NicoNicoStream Stream { get; set; }

		//インラインシークバー画像プレビュー
		public NicoNicoStoryBoard StoryBoard { get; set; }

		//API結果
		public VideoData VideoData { get; set; }


		//各種シークバー関連の時間管理
		public VideoTime Time { get; set; }



		#region BPS変更通知プロパティ
		private string _BPS;

		public string BPS {
			get { return _BPS; }
			set {
				if(_BPS == value)
					return;
				_BPS = value;
				RaisePropertyChanged();
			}
		}
		#endregion



		#region IconData変更通知プロパティ
		private GeometryGroup _IconData = Playing;

		public GeometryGroup IconData {
			get { return _IconData; }
			set {
				if(_IconData == value)
					return;
				_IconData = value;
				RaisePropertyChanged();
			}
		}
		#endregion


		#region SeekCursor変更通知プロパティ
		private Thickness _SeekCursor;

		public Thickness SeekCursor {
			get { return _SeekCursor; }
			set {
				if(_SeekCursor == value)
					return;
				_SeekCursor = value;
				RaisePropertyChanged();
			}
		}
		#endregion


		#region LoadStatus変更通知プロパティ
		private string _LoadStatus;

		public string LoadStatus {
			get { return _LoadStatus; }
			set {
				if(_LoadStatus == value)
					return;
				_LoadStatus = value;
				RaisePropertyChanged();
			}
		}
		#endregion


        public VideoViewModel(WatchApiData apiData) {
            
            //ViewをVideoに変える
            App.ViewModelRoot.RightContent = this;
            App.ViewModelRoot.CurrentVideo = this;


            VideoData = new VideoData();
            VideoData.ApiData = apiData;

        }



        public VideoViewModel(string videoUrl) {


            //ViewをVideoに変える
            App.ViewModelRoot.RightContent = this;
            App.ViewModelRoot.CurrentVideo = this;


            VideoData = new VideoData();
            VideoData.ApiData = NicoNicoWatchApi.GetWatchApiData(videoUrl);

            
		}





        


	}
}
