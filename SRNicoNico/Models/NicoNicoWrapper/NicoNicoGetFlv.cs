using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Net.Http;
using System.Net.Http.Headers;

using Livet;

namespace SRNicoNico.Models.NicoNicoWrapper {
	public class NicoNicoGetFlv : NotificationObject {
		/*
		 * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
		 */

		//getFlvAPIのURL
		public const string GetFlvURL = "http://flapi.nicovideo.jp/api/getflv/";

		//getFlvAPIからデータを取得
		public static NicoNicoGetFlvData GetFlv(string cmsid) {


			return NicoNicoWrapperMain.getSession().HttpClient.GetStringAsync(GetFlvURL + cmsid).ContinueWith<NicoNicoGetFlvData>(task => {

				string response = task.Result;
				Console.WriteLine(System.Web.HttpUtility.UrlDecode(response));

				Dictionary<string, string> data = response.Split(new char[] { '&' }).ToDictionary(source => source.Substring(0, source.IndexOf('=')),
				source => Uri.UnescapeDataString(source.Substring(source.IndexOf('=') + 1)));



				return new NicoNicoGetFlvData(data);
			}).Result;
		}


		public const string WatchURL = "http://www.nicovideo.jp/watch/";


		private static void AccessVideoPage(string cmsid) {

			HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Head, WatchURL + cmsid);
			var a = NicoNicoWrapperMain.getSession().HttpClient.SendAsync(message).Result;
			System.Diagnostics.Debug.WriteLine(a);

		}

		public static Stream GetFlvStream(string cmsid, string videoUrl) {

			AccessVideoPage(cmsid);

			return NicoNicoWrapperMain.getSession().HttpClient.GetStreamAsync(videoUrl).Result;
		}





		public static Stream GetFlvStreamRange(string cmsid, string videoUrl, long length) {

			AccessVideoPage(cmsid);

			HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, videoUrl);
			message.Headers.Range = new RangeHeaderValue(length, null);
			
			HttpResponseMessage response = NicoNicoWrapperMain.getSession().HttpClient.SendAsync(message).Result;

			;

			return response.Content.ReadAsStreamAsync().Result;
		}
	}
}
