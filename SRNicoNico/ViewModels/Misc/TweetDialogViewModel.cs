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

using SRNicoNico.Models;

namespace SRNicoNico.ViewModels {
    public class TweetDialogViewModel : ViewModel {


        #region OriginalUri変更通知プロパティ
        private Uri _OriginalUri;

        public Uri OriginalUri {
            get { return _OriginalUri; }
            set { 
                if(_OriginalUri == value)
                    return;
                _OriginalUri = value;
                RaisePropertyChanged();
            }
        }
        #endregion

    }
}
