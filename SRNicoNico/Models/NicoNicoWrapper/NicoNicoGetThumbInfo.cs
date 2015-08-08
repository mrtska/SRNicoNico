using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codeplex.Data;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Models.NicoNicoWrapper {
	public class NicoNicoGetThumbInfo {

		//動画情報を取得するAPI
		private const string GetThumbInfoUrl = @"http://ext.nicovideo.jp/api/getthumbinfo/";

		//API叩き 非同期
		private static async Task<string> GetThumbInfoAsync(string cmsid) {

			return await NicoNicoWrapperMain.GetSession().HttpClient.GetStringAsync(GetThumbInfoUrl + cmsid);
		}


		//
		public static NicoNicoGetThumbInfoData GetThumbInfo(string cmsid) {
			

			string result = GetThumbInfoAsync(cmsid).Result;

			//json
			string jsonString = NicoNicoUtil.XmlToJson(result);

			//dynamic型json
			var json = DynamicJson.Parse(jsonString);

			//レスポンス
			var response = json.nicovideo_thumb_response;

			//本体
			var thumb = response.thumb;

			//データ
			NicoNicoGetThumbInfoData ret = new NicoNicoGetThumbInfoData() {

				Cmsid = thumb.video_id,
				Title = thumb.title,
				Description = thumb.description,
				ThumbNailUrl = thumb.thumbnail_url,
				FirstRetrieve = thumb.first_retrieve,
				Length = thumb.length,
				MovieType = thumb.movie_type,
				SizeHigh = thumb.size_high,
				SizeLow = thumb.size_low,
				ViewCounter = thumb.view_counter,
				CommentCounter = thumb.comment_num,
				MylistCounter = thumb.mylist_counter

			};


			return ret;
		}


	}

	public class NicoNicoGetThumbInfoData {

		//動画ID
		public string Cmsid { get; set; }

		//動画タイトル
		public string Title { get; set; }

		//動画説明
		public string Description { get; set; }

		//サムネイルURL
		public string ThumbNailUrl { get; set; }

		//動画投稿日
		public string FirstRetrieve { get; set; }

		//動画時間
		public string Length { get; set; }

		//動画フォーマット mp4やflvやswfなどかな？
		public string MovieType { get; set; }

		//動画サイズ 通常画質時？
		public string SizeHigh { get; set; }

		//動画サイズ エコノミー時かな？
		public string SizeLow { get; set; }

		//再生数
		public string ViewCounter { get; set; }

		//コメント数
		public string CommentCounter { get; set; }

		//マイリスト数
		public string MylistCounter { get; set; }

	}
}
