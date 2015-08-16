using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

using Livet;

using SRNicoNico.ViewModels;


namespace SRNicoNico.Models.NicoNicoWrapper {
	public class NicoNicoStream : NotificationObject {
		/*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */


		public VideoViewModel Video;

		//キャッシュのストリーム
		public FileStream CacheStream { get; set; }

		//ストリーミングサーバーからのストリーム
		public Stream VideoStream { get; set; }


		//キャッシュが存在するか否か
		public bool CacheExists { get; set; }




		public NicoNicoStream(VideoViewModel Video) {

			this.Video = Video;
		}


		public void OpenVideo() {
			
            

			//GetFlvAPIを叩いてサーバーを取得
			NicoNicoGetFlvData data = Video.VideoData.ApiData.GetFlv;
            
			//cacheディレクトリを無ければ作成
			Directory.CreateDirectory(NicoNicoUtil.CurrentDirectory.DirectoryName + @"\cache");

			//キャッシュパス
			string path = NicoNicoUtil.CurrentDirectory.DirectoryName + @"\cache\" + Video.VideoData.ApiData.Cmsid;
			FileInfo cache = new FileInfo(path);

			//キャッシュが存在したら
			if (cache.Exists) {

				long length = cache.Length;

				CacheStream = new FileStream(path, FileMode.Open, FileAccess.Write);
				Video.LoadStatus = "ストリーミング開始（キャッシュ有）";
				VideoStream = GetFlvStreamRange(length);
				CacheExists = true;

			} else {

				CacheStream = new FileStream(path, FileMode.Create, FileAccess.Write);
				Video.LoadStatus = "ストリーミング開始（キャッシュ無）";
				VideoStream = GetFlvStream();

				CacheExists = false;
			}
            
				

			Video.Path = path;

			//ストリーミング開始
			StartStreaming();
			
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

        //動画本体のストリームを返す
        public Stream GetFlvStream() {

            return NicoNicoWrapperMain.GetSession().HttpClient.GetStreamAsync(Video.VideoData.ApiData.GetFlv.VideoUrl).Result;
        }

        //指定した場所からストリームを返す
        public Stream GetFlvStreamRange(long length) {

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Video.VideoData.ApiData.GetFlv.VideoUrl);
            request.Headers.Range = new RangeHeaderValue(length, null);

            HttpResponseMessage response = NicoNicoWrapperMain.GetSession().HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;

            return response.Content.ReadAsStreamAsync().Result;
        }
    }
}
