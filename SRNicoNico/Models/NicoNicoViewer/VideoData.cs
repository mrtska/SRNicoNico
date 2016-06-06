using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using SRNicoNico.Models.NicoNicoWrapper;

using SRNicoNico.ViewModels;
using System.Collections.ObjectModel;

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

        //動画タイプ mp4とかswfとかrtmpとか
        #region VideoType変更通知プロパティ
        private NicoNicoVideoType? _VideoType;

        public NicoNicoVideoType? VideoType {
            get { return _VideoType; }
            set { 
                if(_VideoType == value)
                    return;
                _VideoType = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Resolution変更通知プロパティ
        private string _Resolution;

        public string Resolution {
            get { return _Resolution; }
            set { 
                if(_Resolution == value)
                    return;
                _Resolution = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region BitRate変更通知プロパティ
        private string _BitRate;

        public string BitRate {
            get { return _BitRate; }
            set { 
                if(_BitRate == value)
                    return;
                _BitRate = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region FrameRate変更通知プロパティ
        private string _FrameRate;

        public string FrameRate {
            get { return _FrameRate; }
            set { 
                if(_FrameRate == value)
                    return;
                _FrameRate = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region FileSize変更通知プロパティ
        private string _FileSize;

        public string FileSize {
            get { return _FileSize; }
            set { 
                if(_FileSize == value)
                    return;
                _FileSize = value;
                RaisePropertyChanged();
            }
        }
        #endregion
    }
}
