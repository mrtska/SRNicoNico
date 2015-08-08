using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using Livet;

using xZune.Vlc.Wpf;
using xZune.Vlc.Interop.Media;

using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {

	public class VideoViewModel : ViewModel {


		private static GeometryGroup Pause;
		private static GeometryGroup Playing;

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




		//動画ID
		public string Cmsid { get; set; }

		//キャッシュパス
		public string Path { get; set; }

		//プレイヤーインスタンス
		public VlcPlayer Player { get; set; }

		public NicoNicoStream Stream { get; set; }


        //動画時間 long型
		#region Length変更通知プロパティ
		private long _Length;

		public long Length {
			get { return _Length; }
			set { 
				if (_Length == value)
					return;
				_Length = value;
				RaisePropertyChanged();
			}
		}
		#endregion

        //各種シークバー関連の時間管理
		public VideoTime Time { get; set; }
		


		#region BPS変更通知プロパティ
		private string _BPS;

		public string BPS {
			get { return _BPS; }
			set { 
				if (_BPS == value)
					return;
				_BPS = value;
				RaisePropertyChanged();
			}
		}
		#endregion


		//動画情報
		#region ThumInfo変更通知プロパティ
		private NicoNicoGetThumbInfoData _ThumbInfo;
		public NicoNicoGetThumbInfoData ThumbInfo {

			get {
				return _ThumbInfo;
			}
			private set {

				if (_ThumbInfo == value) {

					return;
				}
				_ThumbInfo = value;
				RaisePropertyChanged();
			}
		}
		#endregion



		#region IconData変更通知プロパティ
		private GeometryGroup _IconData = Playing;

		public GeometryGroup IconData {
			get { return _IconData; }
			set { 
				if (_IconData == value)
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
				if (_SeekCursor == value)
					return;
				_SeekCursor = value;
				RaisePropertyChanged();
			}
		}
		#endregion





		public void Initialize() {

			//動画情報取得
			Task.Run(() => {

				SeekCursor = new Thickness();
				Time = new VideoTime();
				ThumbInfo = NicoNicoGetThumbInfo.GetThumbInfo(Cmsid);
				Length = NicoNicoUtil.GetTimeOfLong(ThumbInfo.Length);
			});
		}

		public void Play() {

			if(Player.State == MediaState.Playing) {

				IconData = Playing;
				Player.PauseOrResume();
				return;
			}
			if (Player.State == MediaState.Paused) {

				IconData = Pause;

				Player.PauseOrResume();
				return;
			}

			//準備できてない
			if(Path == null) {

				return;
			}


			IconData = Pause;


			Player.LoadMedia(Path);
			Player.Play();
		}

		public void DisposePlayer() {

			Stream.Dispose();
			if (Player != null) {

				Player.Dispose();
			}
		}
	}
}
