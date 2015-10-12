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

using SRNicoNico.Models;

namespace SRNicoNico.ViewModels {

    public class MylistEditModeViewModel : ViewModel {

        #region AllSelect変更通知プロパティ
        private bool _AllSelect;

        public bool AllSelect {
            get { return _AllSelect; }
            set {
                if(_AllSelect == value)
                    return;
                _AllSelect = value;


                foreach(MylistListEntryViewModel entry in List.Mylist) {

                    entry.IsChecked = value;
                }
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Process変更通知プロパティ
        private string _Process;

        public string Process {
            get { return _Process; }
            set { 
                if(_Process == value)
                    return;
                _Process = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region IsAnyoneChecked変更通知プロパティ

        public bool IsAnyoneChecked {
            get {
                foreach(MylistListEntryViewModel entry in List.Mylist) {

                    if(entry.IsChecked) {

                        return true;
                    }
                }
                return false;
            }
            set { 
                RaisePropertyChanged();
            }
        }
        #endregion


        #region SelectedMylist変更通知プロパティ
        private DispatcherCollection<MylistListEntryViewModel> _SelectedMylist = new DispatcherCollection<MylistListEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<MylistListEntryViewModel> SelectedMylist {
            get { return _SelectedMylist; }
            set { 
                if(_SelectedMylist == value)
                    return;
                _SelectedMylist = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public MylistListViewModel List { get; set; }

        public MylistEditModeViewModel(MylistListViewModel vm) {

            List = vm;
        }


        public void ShowDialog(string process) {

            Process = process;

            SelectedMylist.Clear();

            foreach(MylistListEntryViewModel entry in List.Mylist) {

                if(entry.IsChecked) {

                    SelectedMylist.Add(entry);
                }
            }

            App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.Contents.Mylist.EditConfirmDialog), this, TransitionMode.Modal));
        }


        public void DoProcess(MylistListViewModel list) {

            switch(Process) {
                case "削除":
                    
                    break;
                case "コピー":
                    MylistViewModel.MylistInstance.CopyMylist(SelectedMylist, list);
                    break;
            }


        }









    }
}
