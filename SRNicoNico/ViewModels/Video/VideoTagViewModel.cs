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
    public class VideoTagViewModel : ViewModel {



        #region Tag変更通知プロパティ
        private NicoNicoVideoTag _Tag;

        public NicoNicoVideoTag Tag {
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

        public VideoTagViewModel(NicoNicoVideoTag tag, VideoViewModel vm) {

            Tag = tag;
            Owner = vm;
        }

        public void TagClick() {

            //一時停止
            Owner.Handler.Pause();

            //検索タブに遷移
            App.ViewModelRoot.Search.SearchType = SearchType.Tag;
            App.ViewModelRoot.Search.SearchText = Tag.Tag;
            App.ViewModelRoot.Search.Search(Tag.Tag);
            App.ViewModelRoot.MainContent.SelectedTab = App.ViewModelRoot.Search;
        }
    }
}
