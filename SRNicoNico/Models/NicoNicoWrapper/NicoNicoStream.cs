using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

using Livet;



namespace SRNicoNico.Models.NicoNicoWrapper {
	public class NicoNicoStream : NotificationObject {
		/*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */


		//キャッシュのストリーム
		public FileStream CacheStream { get; set; }

		//ストリーミングサーバーからのストリーム
		public Stream VideoStream { get; set; }


		//キャッシュが存在するか否か
		public bool CacheExists { get; set; }

		public void OpenVideo(NicoNicoSearchResultNode Node) {


			//UIスレッドは使わない
			Task.Run(() => {

				App.ViewModelRoot.Video.Cmsid = Node.cmsid;
				//ViewをVideoに変える
				App.ViewModelRoot.Content = App.ViewModelRoot.Video;


				NicoNicoGetFlv.AccessVideoPage(Node.cmsid);

				//GetFlvAPIを叩いてサーバーを取得
				NicoNicoGetFlvData data = NicoNicoGetFlv.GetFlv(Node.cmsid);

				//cacheディレクトリを無ければ作成
				Directory.CreateDirectory(NicoNicoUtil.CurrentDirectory.DirectoryName + @"\cache");

				//キャッシュパス
				string path = NicoNicoUtil.CurrentDirectory.DirectoryName + @"\cache\" + Node.cmsid;
				FileInfo cache = new FileInfo(path);

				//キャッシュが存在したら
				if (cache.Exists) {

					long length = cache.Length;

					CacheStream = new FileStream(path, FileMode.Open, FileAccess.Write);
					VideoStream = NicoNicoGetFlv.GetFlvStreamRange(Node.cmsid, data.VideoUrl, length);
					CacheExists = true;

				} else {

					CacheStream = new FileStream(path, FileMode.Create, FileAccess.Write);
					VideoStream = NicoNicoGetFlv.GetFlvStream(Node.cmsid, data.VideoUrl);
					CacheExists = false;
				}

				App.ViewModelRoot.Video.Path = path;

				//ストリーミング開始
				StartStreaming();
			});
		}

		public void StartStreaming() {

			Task.Run(() => {

				//キャッシュが存在したら最後までシークする
				if (CacheExists) {

					CacheStream.Seek(0, SeekOrigin.End);
				}

				VideoStream.CopyTo(CacheStream);
			});
		}


		public void Dispose() {

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
