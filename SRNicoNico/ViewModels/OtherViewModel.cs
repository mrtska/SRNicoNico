using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class OtherViewModel : ViewModel {
       

        public void OpenHistory() {


            HistoryResultViewModel History = new HistoryResultViewModel();

            History.IsActive = true;
            App.ViewModelRoot.Content = History;

            Task.Run(() => {



                foreach(NicoNicoHistoryData data in new NicoNicoHistory().GetHistroyData()) {

                    HistoryResultEntryViewModel entry = new HistoryResultEntryViewModel() {

                        Data = data
                    };

                    
                    History.List.Add(entry);
                }



                History.IsActive = false;

                



            });

            ;


        }
    }
}
