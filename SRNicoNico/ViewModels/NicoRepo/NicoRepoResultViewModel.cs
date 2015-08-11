using System;
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
    public class NicoRepoResultViewModel : ViewModel {



        #region IsActive変更通知プロパティ
        private bool _IsActive;

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


        #region OwnerViewModel変更通知プロパティ
        private ViewModel _OwnerViewModel;

        public ViewModel OwnerViewModel {
            get { return _OwnerViewModel; }
            set { 
                if(_OwnerViewModel == value)
                    return;
                _OwnerViewModel = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region NicoRepo変更通知プロパティ
        private ObservableSynchronizedCollection<NicoRepoResultEntryViewModel> _NicoRepo = new ObservableSynchronizedCollection<NicoRepoResultEntryViewModel>();

        public ObservableSynchronizedCollection<NicoRepoResultEntryViewModel> NicoRepo {
            get { return _NicoRepo; }
            set { 
                if(_NicoRepo == value)
                    return;
                _NicoRepo = value;
                RaisePropertyChanged();
            }
        }
        #endregion


    }
}
