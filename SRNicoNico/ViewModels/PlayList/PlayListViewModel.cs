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
    public class PlayListViewModel : TabItemViewModel {



        #region PlayList変更通知プロパティ
        private DispatcherCollection<PlayListEntryViewModel> _PlayList = new DispatcherCollection<PlayListEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<PlayListEntryViewModel> PlayList {
            get { return _PlayList; }
            set { 
                if(_PlayList == value)
                    return;
                _PlayList = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        public PlayListViewModel(IList<MylistListEntryViewModel> list, string title) : base(title) {

            foreach(var entry in list) {

                PlayList.Add(new PlayListEntryViewModel(entry));
            }
        }
        public PlayListViewModel(IList<PlayListEntryViewModel> list, string title) : base(title) {

            foreach(var entry in list) {

                PlayList.Add(entry);
            }
        }





    }
}
