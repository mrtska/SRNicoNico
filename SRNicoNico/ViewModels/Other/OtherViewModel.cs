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
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.ViewModels {
    public class OtherViewModel : TabItemViewModel {



        #region OtherList変更通知プロパティ
        private DispatcherCollection<TabItemViewModel> _OtherList = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);
        
        public DispatcherCollection<TabItemViewModel> OtherList {
            get { return _OtherList; }
            set { 
                if(_OtherList == value)
                    return;
                _OtherList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public OtherViewModel() : base("その他") {

            OtherList.Add(new OverViewViewModel(this));
            OtherList.Add(new OSSViewModel());
        }
    }
}
