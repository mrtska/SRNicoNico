using System;

using System.Windows.Controls;

using xZune.Vlc.Wpf;
using xZune.Vlc;

using Livet;

namespace SRNicoNico.Views.Contents.Video {
	/// <summary>
	/// VideoInternal.xaml の相互作用ロジック
	/// </summary>
	public partial class VideoInternal : UserControl {
		public VideoInternal() {
			InitializeComponent();
		}

		//プレイヤーインスタンス
		public VlcPlayer Player { get; set; }


		//メディアプレイヤーインスタンス
		public VlcMediaPlayer Media { get; set; }


		private void VlcPlayer_Initialized(object sender, EventArgs e) {

			Player = VlcPlayer;
			Media = Player.VlcMediaPlayer;

			Media.EncounteredError += Media_EncounteredError;
			Media.SeekableChanged += Media_SeekableChanged;
			Media.PositionChanged += Media_PositionChanged;
			Media.EndReached += Media_EndReached;
		}


		private void Media_EndReached(object sender, EventArgs e) {


			Player.Position = 0;
			Player.Play();

		}

		private void Media_PositionChanged(object sender, EventArgs e) {



			Console.WriteLine("ポジション:" + Media.Position);

		}


		private void Media_SeekableChanged(object sender, EventArgs e) {

			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {
				FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage("シークが可能になります。", "SeekableChanged", System.Windows.MessageBoxButton.OK);

			}));
			

		}

		private void Media_EncounteredError(object sender, EventArgs e) {

			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {
				FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage("エラーが発生しました。(-1221)", "EncounteredError", System.Windows.MessageBoxButton.OK);
			}));
		}
	}
}
