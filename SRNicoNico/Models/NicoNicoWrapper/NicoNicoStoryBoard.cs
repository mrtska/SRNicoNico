using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Net;

using Livet;

using Codeplex.Data;


namespace SRNicoNico.Models.NicoNicoWrapper {
	public class NicoNicoStoryBoard : NotificationObject {
		/*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */

		//ストリーミングサーバーのURL
		public string StoryBoardApiBaseUrl { get; set; }


        //ビデオURLを指定
		public NicoNicoStoryBoard(string url) {

			StoryBoardApiBaseUrl = url;
		}



		//取得したストーリーボードのデータにBitmapオブジェクトを追加して返す
		public NicoNicoStoryBoardData GetStoryBoardData() {

            //エコノミーだったら
            if(StoryBoardApiBaseUrl.Contains("low")) {

                return null;
            }

			NicoNicoStoryBoardData data = GetStoryBoardInternalData();
			
			//ストーリーボードが無かったら
			if(data == null) {

				return null;
			}



			//APIURL
			string uri = StoryBoardApiBaseUrl + "&sb=" + data.Id + "&board=";


			int bitmapindex = 0;
			for(int i = 1; i <= data.Count; i++) {


				var response = NicoNicoWrapperMain.GetSession().GetStreamAsync(uri + i).Result;

				Bitmap bitmap = new Bitmap(response);

				for(int j = 0; j < data.Cols; j++) {

					for(int k = 0; k < data.Rows; k++) {

						Rectangle rect = new Rectangle(data.Width * k, data.Height * j, data.Width, data.Height);
						
						data.BitmapCollection[bitmapindex] = bitmap.Clone(rect, bitmap.PixelFormat);
						bitmapindex += data.Interval;
					}
				}
			}

			return data;
		}


		//ストーリーボードのデータを取得する
		private NicoNicoStoryBoardData GetStoryBoardInternalData() {

			var result = NicoNicoWrapperMain.GetSession().GetResponseAsync(StoryBoardApiBaseUrl + "&sb=1").Result;
			
			//見つからなかったり見せてもらえなかったりしたら
			if(result.StatusCode == HttpStatusCode.Forbidden || result.StatusCode == HttpStatusCode.NotFound || result.Content.Headers.ContentDisposition.FileName.Contains("smile")) {

				return null;
			}

			byte[] response = result.Content.ReadAsByteArrayAsync().Result;

			string xml = new string(Encoding.ASCII.GetChars(response));
			xml = xml.Substring(39);
            string json = NicoNicoUtil.XmlToJson(xml);
			json = json.Replace("@", "");

			var root = DynamicJson.Parse(json);

			if(root.smile.storyboard.IsArray) {


				foreach(var entry in root.smile.storyboard) {
					
					return new NicoNicoStoryBoardData() {

						Id = entry.IsDefined("id") ? entry.id : 1,
						Cols = int.Parse(entry.board_cols),
						Rows = int.Parse(entry.board_rows),
						Count = int.Parse(entry.board_number),
						Width = int.Parse(entry.thumbnail_width),
						Height = int.Parse(entry.thumbnail_height),
						Interval = int.Parse(entry.thumbnail_interval),
						Number = int.Parse(entry.thumbnail_number)
					};
					
				}
			} else {
				var entry = root.smile.storyboard;

				return new NicoNicoStoryBoardData() {

					Id = entry.IsDefined("id") ? entry.id : "1",
					Cols = int.Parse(entry.board_cols),
					Rows = int.Parse(entry.board_rows),
					Count = int.Parse(entry.board_number),
					Width = int.Parse(entry.thumbnail_width),
					Height = int.Parse(entry.thumbnail_height),
					Interval = int.Parse(entry.thumbnail_interval),
					Number = int.Parse(entry.thumbnail_number)
				};
			}

			return null;
		}












	}


	public class NicoNicoStoryBoardData {

        //コンストラクタ使用不可
        internal NicoNicoStoryBoardData() { }

		//ストーリーボードID
		public string Id { get; set; }

		//サムネイル一つの横幅
		public int Width { get; set; }

		//サムネイル一つの縦幅
		public int Height { get; set; }

		//サムネイルの数
		public int Number { get; set; }

		//サムネイルの間隔
		public int Interval { get; set; }

		//縦のサムネイル数
		public int Rows { get; set; }

		//横のサムネイル数
		public int Cols { get; set; }

		//ボード数
		public int Count { get; set; }


		public Dictionary<int,Bitmap> BitmapCollection = new Dictionary<int, Bitmap>();

	}
}
