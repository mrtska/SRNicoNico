using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Models.NicoNicoViewer {
	public class VideoData : NotificationObject {
	
		//WatchAPIデータ
		#region ApiData変更通知プロパティ
		private WatchApiData _ApiData;

		public WatchApiData ApiData {
			get { return _ApiData; }
			set { 
				if(_ApiData == value)
					return;
				_ApiData = value;
				RaisePropertyChanged();
			}
		}
		#endregion


		//ストーリーボードデータ
		public NicoNicoStoryBoardData StoryBoardData { get; set; }

		//コメントデータ
		public List<NicoNicoCommentEntry> CommentData { get; set; }


	}
}
