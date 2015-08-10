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

namespace SRNicoNico.ViewModels {
    public class NicoRepoViewModel : ViewModel {



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

        public NicoNicoNicoRepoData Data { get; set; }
        





        public void Initialize() {
        }
    }
}
