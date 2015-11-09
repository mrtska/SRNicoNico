using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.ObjectModel;

namespace SRNicoNico.ViewModels {
    public class AccessLogViewModel : ViewModel {

        #region Text変更通知プロパティ
        private DispatcherCollection<AccessLogEntryViewModel> _LogEntries = new DispatcherCollection<AccessLogEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<AccessLogEntryViewModel> LogEntries {
            get { return _LogEntries; }
            set {
                if(_LogEntries == value)
                    return;
                _LogEntries = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private Dictionary<HttpRequestMessage, AccessLogEntryViewModel> HashMap = new Dictionary<HttpRequestMessage, AccessLogEntryViewModel>();

        public void StartAccessUrl(HttpRequestMessage request) {


            string url = request.RequestUri.OriginalString;

            AccessLogEntryViewModel vm = new AccessLogEntryViewModel();
            vm.Status = "接続中";
            vm.Url = url;

            lock(HashMap) {

                HashMap[request] = vm;
                LogEntries.Add(vm);
            }


        }

        public void EndAccessUrl(HttpResponseMessage response) {

            AccessLogEntryViewModel vm = HashMap[response.RequestMessage];

            if(vm != null) {

                vm.Status = "完了";
            }
        }
    }
}
