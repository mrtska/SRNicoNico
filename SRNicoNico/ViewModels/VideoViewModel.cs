using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Runtime.Versioning;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using xZune.Vlc.Wpf;
using xZune.Vlc;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {

	public class VideoViewModel : ViewModel {

		//動画ID
		public string cmsid { get; set; }

		//キャッシュパス
		public string Path { get; set; }

		//プレイヤーインスタンス
		public VlcPlayer Player { get; set; }

		//メディアプレイヤーインスタンス
		public VlcMediaPlayer Media { get; set; }

		//キャッシュのストリーム
		public FileStream CacheStream { get; set; }

		//ストリーミングサーバーからのストリーム
		public Stream VideoStream { get; set; }

		//キャッシュが存在するか否か
		public bool CacheExists { get; set; }



		#region IsMouseOver変更通知プロパティ
		private Visibility _IsMouseOver = Visibility.Hidden;

		public Visibility IsMouseOver {
			get { return _IsMouseOver; }
			set {
				if (_IsMouseOver == value)
					return;
				_IsMouseOver = value;
				RaisePropertyChanged();
			}
		}
		#endregion


		//動画情報
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


		public void MouseOver() {

			IsMouseOver = Visibility.Visible;
		}

		public void MouseLeave() {

			IsMouseOver = Visibility.Hidden;
		}

		public void Initialize() {

            



			//動画情報取得
			Task.Run(new Action(() => {
				ThumbInfo = NicoNicoGetThumbInfo.GetThumbInfo(cmsid);
			}));

			Media = Player.VlcMediaPlayer;
			Media.EncounteredError += Media_EncounteredError;
			Media.SeekableChanged += Media_SeekableChanged;
			Media.PositionChanged += Media_PositionChanged;
			Media.EndReached += Media_EndReached;

			Console.WriteLine(Process.GetCurrentProcess().Id);


		}

		private void Media_EndReached(object sender, EventArgs e) {

			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

				Player.Position = 0;
				Player.Play();
			}));
		}

		private void Media_PositionChanged(object sender, EventArgs e) {


			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

				Console.WriteLine("ポジション:" + Player.Position);
			}));

		}


		private void Media_SeekableChanged(object sender, EventArgs e) {

			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

				FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage("シークが可能になります。", "SeekableChanged", System.Windows.MessageBoxButton.OK);
			}));

		}

		private void Media_EncounteredError(object sender, EventArgs e) {


			FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage("エラーが発生しました。(-1221)", "EncounteredError", System.Windows.MessageBoxButton.OK);

		}

		public void StartStreaming() {

			Task.Run(new Action(() => {

				//キャッシュが存在したら最後までシークする
				if (CacheExists) {

					CacheStream.Seek(0, SeekOrigin.End);
				}

				VideoStream.CopyTo(CacheStream);
			}));

			Thread.Sleep(1000);
			Player.LoadMedia(Path);
		}




		public void Play() {

			Player.Play();
			Player.PauseOrResume();
		}



		public void DisposePlayer() {


			if (Player != null) {

				Player.Dispose();
			}

			if (CacheStream != null) {

				CacheStream.Close();
				CacheStream.Dispose();
			}

			if (VideoStream != null) {

				VideoStream.Close();
				VideoStream.Dispose();
			}

		}


	}
}
