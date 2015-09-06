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

using FirstFloor.ModernUI.Presentation;
using SRNicoNico.Models.NicoNicoViewer;
using System.Collections.ObjectModel;

namespace SRNicoNico.ViewModels {
    public class NicoRepoViewModel : TabItemViewModel {



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
        private ObservableCollection<NicoRepoListViewModel> _NicoRepoListCollection = new ObservableCollection<NicoRepoListViewModel>();

        public ObservableCollection<NicoRepoListViewModel> NicoRepoListCollection {
            get { return _NicoRepoListCollection; }
            set { 
                if(_NicoRepoListCollection == value)
                    return;
                _NicoRepoListCollection = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region SelectedList変更通知プロパティ
        private NicoRepoListViewModel _SelectedList;

        public NicoRepoListViewModel SelectedList {
            get { return _SelectedList; }
            set { 
                if(_SelectedList == value)
                    return;
                _SelectedList = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        public NicoNicoNicoRepoList NicoRepoList = new NicoNicoNicoRepoList();

        public NicoNicoNicoRepoData Data { get; set; }
        
        public NicoRepoViewModel() : base("ニコレポ") {}

        

        public void Reflesh() {

            IsActive = true;
            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => NicoRepoListCollection.Clear()));
            

            Task.Run(() => {


                IList<NicoRepoListViewModel> result = NicoRepoList.GetNicoRepoList();
                NicoRepoListCollection = new ObservableCollection<NicoRepoListViewModel>(result);

                



                IsActive = false;

            });
            

           
        }

        public void InitNicoRepo() {



            Reflesh();




        }





    }
}
