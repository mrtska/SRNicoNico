using System;
using System.Windows;
using System.Threading.Tasks;
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
    public class NicoRepoViewModel : ViewModel {



        #region IsActive変更通知プロパティ
        private bool _IsActive = false;

        public bool IsActive {
            get { return _IsActive; }
            set { 
                if(_IsActive == value)
                    return;
                _IsActive = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        #region NicoRepoListCollection変更通知プロパティ
        private ObservableSynchronizedCollection<NicoRepoListViewModel> _NicoRepoListCollection = new ObservableSynchronizedCollection<NicoRepoListViewModel>();

        public ObservableSynchronizedCollection<NicoRepoListViewModel> NicoRepoListCollection {
            get { return _NicoRepoListCollection; }
            set { 
                if(_NicoRepoListCollection == value)
                    return;
                _NicoRepoListCollection = value;
                RaisePropertyChanged();
            }
        }
        #endregion




        public NicoNicoNicoRepoList NicoRepoList = new NicoNicoNicoRepoList();

        public NicoNicoNicoRepoData Data { get; set; }
        


        public void Reflesh() {

            IsActive = true;
            NicoRepoListCollection.Clear();

            Task.Run(() => {

                NicoRepoListCollection = NicoRepoList.GetNicoRepoList();

                IsActive = false;

            });
            

           
        }

        public void InitNicoRepo() {



            Reflesh();




        }





    }
}
