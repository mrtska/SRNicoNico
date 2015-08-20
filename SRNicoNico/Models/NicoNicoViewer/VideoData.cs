using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using SRNicoNico.Models.NicoNicoWrapper;

using SRNicoNico.ViewModels;

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

		#region CommentData変更通知プロパティ
		private DispatcherCollection<CommentEntryViewModel> _CommentData = new DispatcherCollection<CommentEntryViewModel>(DispatcherHelper.UIDispatcher);

		public DispatcherCollection<CommentEntryViewModel> CommentData {
			get { return _CommentData; }
			set { 
				if(_CommentData == value)
					return;
				_CommentData = value;
				RaisePropertyChanged();
			}
		}
		#endregion



	}
}
