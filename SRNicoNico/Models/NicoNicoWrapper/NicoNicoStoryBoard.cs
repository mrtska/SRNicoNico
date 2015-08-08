using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

namespace SRNicoNico.Models.NicoNicoWrapper {
	public class NicoNicoStoryBoard : NotificationObject {
		/*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */

		//APIのURL 動画によって変わってくる
		public string StoryBoardApiBaseUrl { get; set; }

		//ストリーミングサーバーのURL
		public NicoNicoStoryBoard(string url) {

			StoryBoardApiBaseUrl = url;

		}

		public void Post() {



		}












	}


	public class NicoNicoStoryBoardEntry {

		//サムネイル一つの横幅
		public int Width { get; set; }

		//サムネイル一つの縦幅
		public int Height { get; set; }

		//サムネイルの数
		public int Number { get; set; }

		//サムネイルの間隔
		public int Interval { get; set; }

		//縦のサムネイル数
		public int Row { get; set; }

		//横のサムネイル数
		public int Cols { get; set; }

		//ボード数
		public int Count { get; set; }
	}
}
