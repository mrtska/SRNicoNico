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
		#region ThumbInfoData変更通知プロパティ
		private NicoNicoGetThumbInfoData _ThumbInfoData;

		public NicoNicoGetThumbInfoData ThumbInfoData {
			get { return _ThumbInfoData; }
			set { 
				if(_ThumbInfoData == value)
					return;
				_ThumbInfoData = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		//ストーリーボードデータ
		public NicoNicoStoryBoardData StoryBoardData { get; set; }



	}
}
