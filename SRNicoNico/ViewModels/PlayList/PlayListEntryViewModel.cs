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
using System.Windows.Input;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class PlayListEntryViewModel : TabItemViewModel {


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

        public string  ContentUrl { get; private set; }

        public PlayListEntryViewModel(NicoNicoMylistEntry item) : base(item.Title) {

            ThumbNailUrl = item.ThumbNailUrl;
            ContentUrl = item.ContentUrl;
        }

        public PlayListEntryViewModel(NicoNicoSearchResultEntry item) : base(item.Title) {

            ThumbNailUrl = item.ThumbnailUrl;
            ContentUrl = item.ContentUrl;
        }
    }
}
