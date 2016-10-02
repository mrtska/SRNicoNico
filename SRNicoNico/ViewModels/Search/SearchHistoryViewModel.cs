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
    public class SearchHistoryViewModel : ViewModel {

        public SearchViewModel Owner { get; set; }

        #region Content変更通知プロパティ
        private string _Content;

        public string Content {
            get { return _Content; }
            set { 
                if(_Content == value)
                    return;
                _Content = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public SearchHistoryViewModel(SearchViewModel owner, string text) {

            Owner = owner;
            Content = text;
        }
    }
}
