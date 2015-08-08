using System;
using System.Windows;
using System.Windows.Controls;

using xZune.Vlc.Wpf;
using xZune.Vlc;

using Livet;
using System.ComponentModel;

using SRNicoNico.ViewModels;
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.Views.Contents.Video {
	/// <summary>
	/// VideoInternal.xaml の相互作用ロジック
	/// </summary>
	public partial class VideoInternal : UserControl {


		//プレイヤーインスタンス
		public VlcPlayer Player { get; set; }


		//メディアプレイヤーインスタンス
		public VlcMediaPlayer Media { get; set; }

		public VideoViewModel Video;

		public VideoInternal() {
			InitializeComponent();
		}

		private void VlcPlayer_Initialized(object sender, EventArgs e) {

			//デザイナはここで止まる
#if DEBUG
			if ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue)) {
				return;
			}
#endif



			Player = VlcPlayer;
			Media = Player.VlcMediaPlayer;

			Media.EncounteredError += Media_EncounteredError;
			Media.SeekableChanged += Media_SeekableChanged;
			Media.EndReached += Media_EndReached;
			App.ViewModelRoot.CurrentVideo.Player = Player;
			App.ViewModelRoot.CurrentVideo.Initialize();
		}


		private void Media_EndReached(object sender, EventArgs e) {

			

		}


		private void Media_SeekableChanged(object sender, EventArgs e) {

			Console.WriteLine("シーク可能");

		}

		private void Media_EncounteredError(object sender, EventArgs e) {

			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {
				FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage("エラーが発生しました。(-1221)", "EncounteredError", MessageBoxButton.OK);
			}));
		}

		private void VlcPlayer_PositionChanged(object sender, DependencyPropertyChangedEventArgs e) {

			if ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue)) {
				return;
			}

			VideoTime time = App.ViewModelRoot.CurrentVideo.Time;


			
			time.CurrentTime = (long)(App.ViewModelRoot.CurrentVideo.Length * Media.Position);
			Console.WriteLine("ポジション:" + App.ViewModelRoot.CurrentVideo.Time.CurrentTime);
			time.CurrentTimeWidth = ActualWidth / App.ViewModelRoot.CurrentVideo.Length * time.CurrentTime;
			Console.WriteLine("シークバー:" + time.CurrentTimeWidth);

			App.ViewModelRoot.CurrentVideo.SeekCursor = new Thickness(time.CurrentTimeWidth,0, 0, 0);







			if (App.ViewModelRoot.CurrentVideo.Time.CurrentTime == App.ViewModelRoot.CurrentVideo.Length) {

				Player.Position = 0F;
			}


			App.ViewModelRoot.CurrentVideo.BPS = (Models.NicoNicoViewer.BPSCounter.Bps / 1024) + "KiB/秒";

		}
	}
}
