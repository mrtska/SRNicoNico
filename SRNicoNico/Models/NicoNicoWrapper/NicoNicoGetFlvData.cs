using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

namespace SRNicoNico.Models.NicoNicoWrapper {
	public class NicoNicoGetFlvData : NotificationObject {
		/*
		 * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
		 */


		//getflvAPIで取得できるデータ

		//スレッドID
		public uint threadID { get; internal set; }

		//長さ
		public uint length { get; internal set; }

		//動画URL
		public string videoUrl { get; internal set; }

		//コメントサーバーURL
		public Uri commentServerUrl { get; internal set; }

		//サブコメントサーバーURL
		public Uri subCommentServerUrl { get; internal set; }

		//非公開理由
		public int closedReason { get; internal set; }

		public NicoNicoGetFlvData(Dictionary<string, string> wwwData) {

			this.threadID = uint.Parse(wwwData["thread_id"]);
			this.length = uint.Parse(wwwData["l"]);
			this.videoUrl = wwwData["url"];
			this.commentServerUrl = new Uri(wwwData["ms"]);
			this.subCommentServerUrl = new Uri(wwwData["ms_sub"]);
		}

	}
}
