using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class HistoryViewModel : TabItemViewModel {



        #region HistoryResult変更通知プロパティ
        private HistoryResultViewModel _HistoryResult;

        public HistoryResultViewModel HistoryResult {
            get { return _HistoryResult; }
            set { 
                if(_HistoryResult == value)
                    return;
                _HistoryResult = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public HistoryViewModel() : base("視聴履歴") {

        }


        public void OpenHistory() {


            HistoryResultViewModel History = new HistoryResultViewModel();

            HistoryResult = History;
            History.IsActive = true;

            Task.Run(() => {

                foreach(NicoNicoHistoryData data in new NicoNicoHistory(this).GetHistroyData()) {

                    HistoryResultEntryViewModel entry = new HistoryResultEntryViewModel() {

                        Data = data
                    };


                    History.List.Add(entry);
                }
                History.IsActive = false;
            });
        }
    }
}
