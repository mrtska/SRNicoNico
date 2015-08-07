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


using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {

	public class VideoViewModel : ViewModel {

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


		#region CurrentTime変更通知プロパティ
		private long _CurrentTime;

		public long CurrentTime {
			get { return _CurrentTime; }
			set { 
				if (_CurrentTime == value)
					return;
				_CurrentTime = value;
				RaisePropertyChanged();
			}
		}
		#endregion


		

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

		public void MouseOver() {

			IsMouseOver = Visibility.Visible;
		}

		public void MouseLeave() {

			IsMouseOver = Visibility.Hidden;
		}



		public void Initialize() {

			//動画情報取得
			Task.Run(() => {
				ThumbInfo = NicoNicoGetThumbInfo.GetThumbInfo(Cmsid);
				Length = NicoNicoUtil.GetTimeOfLong(ThumbInfo.Length);
			});
		}

		public void Play() {

			Player.LoadMedia(Path);
			Player.Play();
			Player.PauseOrResume();
		}

		public void DisposePlayer() {


			if (Player != null) {

				Player.Dispose();
			}
		}
	}
}
