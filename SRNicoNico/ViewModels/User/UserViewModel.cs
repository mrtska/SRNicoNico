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
    public class UserViewModel : TabItemViewModel {

        private NicoNicoUser UserInstance;

        #region UserEntry変更通知プロパティ
        private NicoNicoUserEntry _UserEntry;

        public NicoNicoUserEntry UserEntry {
            get { return _UserEntry; }
            set { 
                if(_UserEntry == value)
                    return;
                _UserEntry = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public UserViewModel(string url) {

            UserInstance = new NicoNicoUser(url);

            UserEntry = UserInstance.GetUserInfo();

            Name = UserEntry.UserName;
            App.ViewModelRoot.TabItems.Add(this);
            App.ViewModelRoot.SelectedTab = this;
        }




    }
}
