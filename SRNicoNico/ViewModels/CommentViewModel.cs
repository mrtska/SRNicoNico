using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Views.Controls;

namespace SRNicoNico.ViewModels {
    public class CommentViewModel : ViewModel {


        private readonly VideoViewModel Video;



        #region View変更通知プロパティ
        private CommentView _View;

        public CommentView View {
            get { return _View; }
            set { 
                if(_View == value)
                    return;
                _View = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        public CommentViewModel(VideoViewModel video) {

            Video = video;


            

        }

        






    }
}
