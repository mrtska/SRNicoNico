using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Models.NicoNicoWrapper {
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
