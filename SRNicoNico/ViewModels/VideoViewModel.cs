using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using xZune.Vlc.Wpf;
using xZune.Vlc;

using SRNicoNico.Models;

namespace SRNicoNico.ViewModels {

	public class VideoViewModel : ViewModel {


		public Uri Uri { get; set; }
		public string Path { get; set; }

		public VlcPlayer Player { get; set; }
		public VlcMediaPlayer Media { get; set; }

		public FileStream CacheStream { get; set; }
		public Stream VideoStream { get; set; }

		public bool CacheExists { get; set; }


		public void Initialize() {

			Media = Player.VlcMediaPlayer;
			Media.EncounteredError += Media_EncounteredError;
			Media.SeekableChanged += Media_SeekableChanged;
			Media.PositionChanged += Media_PositionChanged;
			
		}

		private void Media_PositionChanged(object sender, EventArgs e) {


			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

				Console.WriteLine("ポジション変更:" + Player.Position);
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
				if(CacheExists) {

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

			if (VideoStream != null) {

				VideoStream.Close();
				VideoStream.Dispose();
			}

		}


	}
}
