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

        public IList<NicoNicoNicoRepoDataEntry> Data { get; set; }
        
        public NicoRepoViewModel() : base("ニコレポ") {}

        

        public void Reflesh() {

            Status = "ニコレポ取得中";
            IsActive = true;
            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => NicoRepoListCollection.Clear()));
            
            Task.Run(() => {

                IList<NicoNicoNicoRepoListEntry> result = NicoRepoList.GetNicoRepoList();

                foreach(NicoNicoNicoRepoListEntry entry in result) {

                    NicoRepoListViewModel vm = new NicoRepoListViewModel(this, entry.Title, entry.Id);
                    Status = "ニコレポ取得中(" + vm.Name + ")";
                    vm.Result.IsActive = true;
                    vm.OpenNicoRepoList();
                    DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => NicoRepoListCollection.Add(vm)));
                }


                IsActive = false;

                Status = "ニコレポ取得完了";

            });
        }

        public void InitNicoRepo() {



            Reflesh();




        }





    }
}
