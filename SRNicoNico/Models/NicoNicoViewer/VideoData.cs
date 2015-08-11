using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Models.NicoNicoViewer {
	public class VideoData : NotificationObject {
	
		//WatchAPIデータ
		public WatchApiData ApiData { get; set; }



		//ストーリーボードデータ
		public NicoNicoStoryBoardData StoryBoardData { get; set; }



	}
}
