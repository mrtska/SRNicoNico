using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;
using System.Threading.Tasks;

namespace SRNicoNico.ViewModels {
    public class MylistViewModel : TabItemViewModel {


        private static NicoNicoMylist MylistInstance = new NicoNicoMylist();


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




        #region NicoRepoListCollection変更通知プロパティ
        private DispatcherCollection<MylistListViewModel> _MylistListCollection = new DispatcherCollection<MylistListViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<MylistListViewModel> MylistListCollection {
            get { return _MylistListCollection; }
            set {
                if(_MylistListCollection == value)
                    return;
                _MylistListCollection = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region SelectedList変更通知プロパティ
        private MylistListViewModel _SelectedList;

        public MylistListViewModel SelectedList {
            get { return _SelectedList; }
            set {
                if(_SelectedList == value)
                    return;
                _SelectedList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public MylistViewModel() : base("マイリスト") {



        }



        public void Reflesh() {

            IsActive = true;


            Task.Run(() => {

                MylistListCollection.Clear();

                App.ViewModelRoot.Status = "マイリスト取得中(とりあえずマイリスト)";
                MylistListCollection.Add(new MylistListViewModel("とりあえずマイリスト", MylistInstance.GetDefMylist()));

                foreach(NicoNicoMylistGroupData group in MylistInstance.GetMylistGroup()) {

                    App.ViewModelRoot.Status = "マイリスト取得中(" + group.Name + ")";
                    MylistListCollection.Add(new MylistListViewModel(group.Name, MylistInstance.GetMylist(group.Id)));


                    
                }
                App.ViewModelRoot.Status = "マイリスト取得完了";


                IsActive = false;
            });

            









        }


    }
}
