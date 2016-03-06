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
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class NicoRepoViewModel : TabItemViewModel {
        

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

        public void Refresh() {

            Status = "ニコレポ取得中";
            IsActive = true;
            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => NicoRepoListCollection.Clear()));
            
            Task.Run(() => {

                IList<NicoNicoNicoRepoListEntry> result = NicoRepoList.GetNicoRepoList();

                foreach(NicoNicoNicoRepoListEntry entry in result) {

                    NicoRepoListViewModel vm = new NicoRepoListViewModel(this, entry.Title, entry.Id);
                    Status = "ニコレポ取得中(" + vm.Name + ")";
                    vm.OpenNicoRepoList();
                    DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => NicoRepoListCollection.Add(vm)));
                }


                IsActive = false;

                Status = "";

            });
        }

        public void InitNicoRepo() {

            Refresh();
        }

        public override void KeyDown(KeyEventArgs e) {

            if(e.Key == Key.F5) {

                if(e.KeyboardDevice.Modifiers == ModifierKeys.Control || e.KeyboardDevice.Modifiers == ModifierKeys.Shift) {

                    Refresh();
                } else if(SelectedList != null) {

                    SelectedList.Refresh();
                } else {

                    Refresh();
                }
            }
        }

    }
}
