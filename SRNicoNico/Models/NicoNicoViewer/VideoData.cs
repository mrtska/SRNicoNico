using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Models.NicoNicoViewer {
	public class VideoData : NotificationObject {
	
		//GetFlvAPI結果
		public NicoNicoGetFlvData GetFlvData { get; set; }

		//動画情報取得API結果
		public NicoNicoGetThumbInfoData ThumbInfoData { get; set; }


	}
}
