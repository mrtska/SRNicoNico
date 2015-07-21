using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using xZune.Vlc.Wpf;

using SRNicoNico.Models;

namespace SRNicoNico.ViewModels {

	public class VideoViewModel : ViewModel {


		public Uri Uri { get; set; }
		public string Path { get; set; }

		public VlcPlayer Player { get; set; }

		public FileStream CacheStream { get; set; }
		public Stream VideoStream { get; set; }

		public void Initialize() {


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
