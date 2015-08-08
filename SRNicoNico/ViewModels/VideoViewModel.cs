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

		//動画ID
		public string Cmsid { get; set; }

		//キャッシュパス
		public string Path { get; set; }

		//プレイヤーインスタンス
		public VlcPlayer Player { get; set; }

		//ストリーミング
		public NicoNicoStream Stream { get; set; }

		//インラインシークバー画像プレビュー
		public NicoNicoStoryBoard StoryBoard { get; set; }

		//API結果
		public VideoData VideoData { get; set; }


		//動画時間 long型
		#region Length変更通知プロパティ
		private long _Length;

		public long Length {
			get { return _Length; }
			set {
				if(_Length == value)
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


		public VideoViewModel(NicoNicoSearchResultNode Node) {

			App.ViewModelRoot.CurrentVideo = this;
			VideoData = new VideoData();
			Stream = new NicoNicoStream(this);
			SeekCursor = new Thickness();
			Time = new VideoTime();

			//バックグラウンドでいろいろと処理をする
			Task.Run(() => {

				//ストリーミングサーバーオープン
				OpenVideo(Node);
				LoadStatus = "動画情報取得中";

				//動画説明文などを取得
				VideoData.ThumbInfoData = NicoNicoGetThumbInfo.GetThumbInfo(Node.cmsid);
				Length = NicoNicoUtil.GetTimeOfLong(VideoData.ThumbInfoData.Length);

				LoadStatus = "ストーリーボード取得中";
				VideoData.StoryBoardData = new NicoNicoStoryBoard(VideoData.GetFlvData.VideoUrl).GetStoryBoardData();
				


				LoadStatus = "";
			});
		}




		public void Initialize() {


		}

		private void OpenVideo(NicoNicoSearchResultNode Node) {

			Stream.OpenVideo(Node);
		}

		public void Play() {

			LoadStatus = "";


			if(Player.State == MediaState.Playing) {

				IconData = Playing;
				Player.PauseOrResume();
				return;
			}
			if(Player.State == MediaState.Paused) {

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
			if(Player != null) {

				Player.Dispose();
			}
		}
	}
}
