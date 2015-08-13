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



        #region Result変更通知プロパティ
        private NicoRepoResultViewModel _Result = new NicoRepoResultViewModel();

        public NicoRepoResultViewModel Result {
            get { return _Result; }
            set { 
                if(_Result == value)
                    return;
                _Result = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private NicoNicoNicoRepo NicoRepo;


        //ニコレポリストを開く
        public void OpenNicoRepoList() {


            Result.IsActive = true;
            Result.OwnerViewModel = this;
            App.ViewModelRoot.RightContent = Result;
            Result.NicoRepo.Clear();

            Task.Run(() => {

                NicoRepo = new NicoNicoNicoRepo(Id);

                NicoNicoNicoRepoData data = NicoRepo.GetNicoRepo();

                foreach(NicoNicoNicoRepoDataEntry entry in data.DataCollection) {

                    NicoRepoResultEntryViewModel ventry = new NicoRepoResultEntryViewModel();
                    ventry.Entry = entry;

                    DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                        Result.NicoRepo.Add(ventry);
                    }));
                }

                Result.IsActive = false;
            });
        }

        public void NextNicoRepoList() {

            Result.IsActive = true;

            Task.Run(() => {

                NicoNicoNicoRepoData data = NicoRepo.NextNicoRepo();

                if(data == null) {

                    Result.IsActive = false;
                    return;
                }


                foreach(NicoNicoNicoRepoDataEntry entry in data.DataCollection) {

                    NicoRepoResultEntryViewModel ventry = new NicoRepoResultEntryViewModel();
                    ventry.Entry = entry;

                    DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                        Result.NicoRepo.Add(ventry);
                    }));
                }

                Result.IsActive = false;
            });






        }

    }
}
