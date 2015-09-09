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
    public class NicoRepoListViewModel : TabItemViewModel {




        private readonly string Id;



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

        public NicoRepoListViewModel(string title, string id) : base(title) {

            Id = id;
            OpenNicoRepoList();
        }


        //ニコレポリストを開く
        public void OpenNicoRepoList() {


            Result.IsActive = true;
            Result.OwnerViewModel = this;
            Result.NicoRepo.Clear();

            Task.Run(() => {

                NicoRepo = new NicoNicoNicoRepo(Id);

                NicoNicoNicoRepoData data = NicoRepo.GetNicoRepo();

                foreach(NicoNicoNicoRepoDataEntry entry in data.DataCollection) {

					Result.NicoRepo.Add(new NicoRepoResultEntryViewModel(entry));
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

                    Result.NicoRepo.Add(new NicoRepoResultEntryViewModel(entry));
                }

                Result.IsActive = false;
            });






        }

    }
}
