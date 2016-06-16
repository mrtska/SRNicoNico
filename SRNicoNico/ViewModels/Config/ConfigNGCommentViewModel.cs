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
using System.Windows;
using Codeplex.Data;

namespace SRNicoNico.ViewModels {
    public class ConfigNGCommentViewModel : ConfigViewModelBase {


        #region NGList変更通知プロパティ
        private DispatcherCollection<NGCommentEntry> _NGList = new DispatcherCollection<NGCommentEntry>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<NGCommentEntry> NGList {
            get { return _NGList; }
            set { 
                if(_NGList == value)
                    return;
                _NGList = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public ConfigNGCommentViewModel() : base("コメントNG機能") {

            var entry = new NGCommentEntry();
            entry.IsEnabled = true;
            entry.Type = NGType.RegEx;
            entry.Content = "\\d+";

            NGList.Add(entry);
        }

        public void AddEmptyNGEntry() {

            var entry = new NGCommentEntry();
            entry.IsEnabled = true;
            entry.Type = NGType.Word;
            entry.Content = "";

            NGList.Add(entry);
        }

        public void AddNGEntry(NGType type, string content) {

            var entry = new NGCommentEntry();
            entry.IsEnabled = true;
            entry.Type = type;
            entry.Content = content;

            NGList.Add(entry);
        }

    }
}
