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

using SRNicoNico.Models.NicoNicoWrapper;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class VideoTagViewModel : ViewModel {

        #region Tag変更通知プロパティ
        private NicoNicoTag _Tag;

        public NicoNicoTag Tag {
            get { return _Tag; }
            set { 
                if(_Tag == value)
                    return;
                _Tag = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private VideoViewModel Owner;

        public VideoTagViewModel(NicoNicoTag tag, VideoViewModel vm) {

            Tag = tag;
            Owner = vm;
        }


        public void TagClick() {

            Owner.Handler.Pause();
            App.ViewModelRoot.SearchText(SearchType.Tag, Tag.Tag);
            
        }
    }
}
