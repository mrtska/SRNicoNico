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
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Installer.ViewModels {
    public class MainWindowViewModel : ViewModel {


        #region Index変更通知プロパティ
        private int _Index = 0;

        public int Index {
            get { return _Index; }
            set { 
                if(_Index == value)
                    return;
                _Index = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region PrevButtonAvailable変更通知プロパティ
        private bool _PrevButtonAvailable = false;

        public bool PrevButtonAvailable {
            get { return _PrevButtonAvailable; }
            set { 
                if(_PrevButtonAvailable == value)
                    return;
                _PrevButtonAvailable = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region NextButtonAvailable変更通知プロパティ
        private bool _NextButtonAvailable = true;

        public bool NextButtonAvailable {
            get { return _NextButtonAvailable; }
            set { 
                if(_NextButtonAvailable == value)
                    return;
                _NextButtonAvailable = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        #region InstallLocationViewModel変更通知プロパティ
        private InstallLocationViewModel _InstallLocationViewModel;

        public InstallLocationViewModel InstallLocationViewModel {
            get { return _InstallLocationViewModel; }
            set { 
                if(_InstallLocationViewModel == value)
                    return;
                _InstallLocationViewModel = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region ProgressViewModel変更通知プロパティ
        private ProgressViewModel _ProgressViewModel;

        public ProgressViewModel ProgressViewModel {
            get { return _ProgressViewModel; }
            set { 
                if(_ProgressViewModel == value)
                    return;
                _ProgressViewModel = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region InstallFinishViewModel変更通知プロパティ
        private InstallFinishViewModel _InstallFinishViewModel;

        public InstallFinishViewModel InstallFinishViewModel {
            get { return _InstallFinishViewModel; }
            set { 
                if(_InstallFinishViewModel == value)
                    return;
                _InstallFinishViewModel = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public MainWindowViewModel() {

            InstallLocationViewModel = new InstallLocationViewModel();
            ProgressViewModel = new ProgressViewModel(this);
            InstallFinishViewModel = new InstallFinishViewModel(this);
        }


        public void Initialize() {
        }

        public void Prev() {

            Index--;
        }

        public void Next() {

            //強制的に終わり
            if(Index == 3) {

                InstallFinishViewModel.EndOfLife();
                return;
            }
            Index++;
        }




    }
}
