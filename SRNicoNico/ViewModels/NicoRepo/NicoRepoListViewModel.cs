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
        private NicoRepoResultViewModel _Result;

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

        private NicoRepoViewModel NicoRepoVM;

        public NicoRepoListViewModel(NicoRepoViewModel vm, string title, string id) : base(title) {

            NicoRepoVM = vm;
            Id = id;
            Result = new NicoRepoResultViewModel(title);
        }


        //ニコレポリストを開く UIスレッドで呼んではいけない
        public void OpenNicoRepoList() {

            Result.IsActive = true;
           
            NicoRepoVM.Status = "ニコレポ取得中(" + Name + ")";


            Result.OwnerViewModel = this;
            Result.NicoRepo.Clear();


            NicoRepo = new NicoNicoNicoRepo(Id);

            var data = NicoRepo.GetNicoRepo();

            if(data == null) {

                NicoRepoVM.Status = "ニコレポ(" + Name + ") の取得に失敗しました";
                Result.IsActive = false;
                return;
            }

            foreach(NicoNicoNicoRepoDataEntry entry in data) {

                Result.NicoRepo.Add(new NicoRepoResultEntryViewModel(entry, this));
            }

            Result.IsActive = false;

        }

        public void NextNicoRepoList() {

            Result.IsActive = true;

            Task.Run(() => {

                IList<NicoNicoNicoRepoDataEntry> data = NicoRepo.NextNicoRepo();

                if(data == null) {

                    Result.IsActive = false;
                    return;
                }

                foreach(NicoNicoNicoRepoDataEntry entry in data) {

                    Result.NicoRepo.Add(new NicoRepoResultEntryViewModel(entry, this));
                }

                Result.IsActive = false;
            });
        }

        public void Refresh() {

            Task.Run(() => {

                OpenNicoRepoList();
                NicoRepoVM.Status = "";
            });


        }
    }
}
