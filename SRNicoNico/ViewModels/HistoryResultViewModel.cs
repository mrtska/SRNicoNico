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
    public class HistoryResultViewModel : ViewModel {




        #region IsActive変更通知プロパティ
        private bool _IsActive;

        public bool IsActive {
            get { return _IsActive; }
            set { 
                if(_IsActive == value)
                    return;
                _IsActive = value;
                RaisePropertyChanged();
            }
        }
        #endregion




        #region List変更通知プロパティ
        private DispatcherCollection<HistoryResultEntryViewModel> _List = new DispatcherCollection<HistoryResultEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<HistoryResultEntryViewModel> List {
            get { return _List; }
            set { 
                if(_List == value)
                    return;
                _List = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        public void OpenHistory() {




            ;
        }







    }
}
