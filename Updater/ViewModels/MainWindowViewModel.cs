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

using Updater.Models;
using System.Windows;

namespace Updater.ViewModels {
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

         
        internal string InstallLocation = "";

        public MainWindowViewModel() {

            var args = Environment.GetCommandLineArgs();

            if(args.Length == 1 || args[1] != "iris") {

                Environment.Exit(0);
            }

            InstallLocation = args[2] + @"\";

            ProgressViewModel = new ProgressViewModel(this);
        }


        public void Initialize() {
        }

        public void Prev() {

            Index--;
        }

        public void Next() {

            Index++;
        }


    }
}
