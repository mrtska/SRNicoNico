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

			return await NicoNicoWrapperMain.getSession().HttpClient.GetStringAsync(GetThumbInfoUrl + cmsid);
		}


		//
		public static NicoNicoGetThumbInfoData GetThumbInfo(string cmsid) {

			return Task.Run(new Func<NicoNicoGetThumbInfoData>(() => {

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
			})).Result;
		}


	}
}
