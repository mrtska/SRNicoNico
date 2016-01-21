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
    public class CommunityTopViewModel : TabItemViewModel {



        #region Owner変更通知プロパティ
        private CommunityViewModel _Owner;

        public CommunityViewModel Owner {
            get { return _Owner; }
            set { 
                if(_Owner == value)
                    return;
                _Owner = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public CommunityTopViewModel(CommunityViewModel vm) : base("コミュニティトップ") {

            Owner = vm;
        }

    }
}
