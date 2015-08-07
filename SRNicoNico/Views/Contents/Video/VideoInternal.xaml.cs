using System;
using System.Windows;
using System.Windows.Controls;

using xZune.Vlc.Wpf;
using xZune.Vlc;

using Livet;
using System.ComponentModel;

namespace SRNicoNico.Views.Contents.Video {
	/// <summary>
	/// VideoInternal.xaml の相互作用ロジック
	/// </summary>
	public partial class VideoInternal : UserControl {


		//プレイヤーインスタンス
		public VlcPlayer Player { get; set; }


		//メディアプレイヤーインスタンス
		public VlcMediaPlayer Media { get; set; }



		public VideoInternal() {
			InitializeComponent();
		}

		private void VlcPlayer_Initialized(object sender, EventArgs e) {

			if ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue)) {
				return;
			}

			Player = VlcPlayer;
			Media = Player.VlcMediaPlayer;

			Media.EncounteredError += Media_EncounteredError;
			Media.SeekableChanged += Media_SeekableChanged;
			Media.EndReached += Media_EndReached;
			App.ViewModelRoot.Video.Player = Player;
			App.ViewModelRoot.Video.Initialize();
		}


		private void Media_EndReached(object sender, EventArgs e) {

			

		}


		private void Media_SeekableChanged(object sender, EventArgs e) {

			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {
				FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage("シークが可能になります。", "SeekableChanged", MessageBoxButton.OK);

			}));
			

		}

		private void Media_EncounteredError(object sender, EventArgs e) {

			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {
				FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage("エラーが発生しました。(-1221)", "EncounteredError", MessageBoxButton.OK);
			}));
		}

		private void VlcPlayer_PositionChanged(object sender, DependencyPropertyChangedEventArgs e) {



			
			App.ViewModelRoot.Video.CurrentTime = (long)(App.ViewModelRoot.Video.Length * Media.Position);
			App.ViewModelRoot.Video.BPS = (Models.NicoNicoViewer.BPSCounter.Bps / 1024) + "KiB/秒";
			Console.WriteLine("ポジション:" + App.ViewModelRoot.Video.CurrentTime);
			if(App.ViewModelRoot.Video.CurrentTime == App.ViewModelRoot.Video.Length) {

				Player.Position = 0F;
			}

		}
	}
}
