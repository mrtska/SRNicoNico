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
		public uint ThreadID { get; internal set; }

		//長さ
		public uint Length { get; internal set; }

		//動画URL
		public string VideoUrl { get; internal set; }

		//コメントサーバーURL
		public Uri CommentServerUrl { get; internal set; }

		//サブコメントサーバーURL
		public Uri SubCommentServerUrl { get; internal set; }

		//非公開理由
		public int ClosedReason { get; internal set; }

		public NicoNicoGetFlvData(Dictionary<string, string> wwwData) {

			this.ThreadID = uint.Parse(wwwData["thread_id"]);
			this.Length = uint.Parse(wwwData["l"]);
			this.VideoUrl = wwwData["url"];
			this.CommentServerUrl = new Uri(wwwData["ms"]);
			this.SubCommentServerUrl = new Uri(wwwData["ms_sub"]);
		}

	}
}
