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
    public class NicoRepoListViewModel : ViewModel {


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


        #region Id変更通知プロパティ
        private string _Id;

        public string Id {
            get { return _Id; }
            set { 
                if(_Id == value)
                    return;
                _Id = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public void OpenNicoRepoList() {

            NicoRepoResultViewModel result = new NicoRepoResultViewModel();

            result.IsActive = true;
            App.ViewModelRoot.Content = result;


            Task.Run(() => {

            NicoNicoNicoRepoData data = new NicoNicoNicoRepo(Id).GetNicoRepo();

            foreach(NicoNicoNicoRepoDataEntry entry in data.DataCollection) {

                NicoRepoResultEntryViewModel ventry = new NicoRepoResultEntryViewModel();
                ventry.Entry = entry;

                DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                    result.NicoRepo.Add(ventry);
                }));
                }

                result.IsActive = false;



            });
           


            ;
        }


    }
}
