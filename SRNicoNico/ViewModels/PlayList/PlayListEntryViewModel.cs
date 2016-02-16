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
    public class PlayListEntryViewModel : ViewModel {


        #region ThumbNailUrl変更通知プロパティ
        private string _ThumbNailUrl;

        public string ThumbNailUrl {
            get { return _ThumbNailUrl; }
            set { 
                if(_ThumbNailUrl == value)
                    return;
                _ThumbNailUrl = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Title変更通知プロパティ
        private string _Title;

        public string Title {
            get { return _Title; }
            set { 
                if(_Title == value)
                    return;
                _Title = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public PlayListEntryViewModel(string title, string thumbnail) {

            Title = title;
            ThumbNailUrl = thumbnail;
        }
        public PlayListEntryViewModel(MylistListEntryViewModel entry) {

            Title = entry.Entry.Title;
            ThumbNailUrl = entry.Entry.ThumbNailUrl;
        }

    }
}
