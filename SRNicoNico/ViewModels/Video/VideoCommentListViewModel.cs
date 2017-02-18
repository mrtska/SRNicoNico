using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class VideoCommentListViewModel : ViewModel {

        #region ListName変更通知プロパティ
        private string _ListName;

        public string ListName {
            get { return _ListName; }
            set { 
                if(_ListName == value)
                    return;
                _ListName = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CommentList変更通知プロパティ
        private DispatcherCollection<VideoCommentEntryViewModel> _CommentList = new DispatcherCollection<VideoCommentEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<VideoCommentEntryViewModel> CommentList {
            get { return _CommentList; }
            set { 
                if(_CommentList == value)
                    return;
                _CommentList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        internal NicoNicoCommentList CommentListInstance;

        public VideoCommentListViewModel(NicoNicoCommentList instance) {

            CommentListInstance = instance;
            ListName = instance.ListName;
        }








    }
}
