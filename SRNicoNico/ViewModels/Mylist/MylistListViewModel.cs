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
using System.Collections.ObjectModel;

using GongSolutions.Wpf.DragDrop;

namespace SRNicoNico.ViewModels {
    public class MylistListViewModel : TabItemViewModel, IDragSource  {

        #region Mylist変更通知プロパティ
        private DispatcherCollection<MylistListEntryViewModel> _Mylist;

        public DispatcherCollection<MylistListEntryViewModel> Mylist {
            get { return _Mylist; }
            set { 
                if(_Mylist == value)
                    return;
                _Mylist = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SelectedItem変更通知プロパティ
        private MylistListEntryViewModel _SelectedItem;

        public MylistListEntryViewModel SelectedItem {
            get { return _SelectedItem; }
            set {
                if(_SelectedItem == value)
                    return;
                _SelectedItem = value;
                RaisePropertyChanged();
            }
        }
        #endregion


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

        public NicoNicoMylistGroupData Group;

        public MylistViewModel Owner { get; private set; }

        public MylistListViewModel(MylistViewModel vm, NicoNicoMylistGroupData group, List<NicoNicoMylistData> list) : base(group.Name) {

            Owner = vm;
            Group = group;
            Mylist = new DispatcherCollection<MylistListEntryViewModel>(DispatcherHelper.UIDispatcher);
            foreach(NicoNicoMylistData data in list) {

                Mylist.Add(new MylistListEntryViewModel(this, data));
            }

        }

        public void Open() {

            if(SelectedItem != null) {

                if(SelectedItem.Entry.Type == 0) {

                    new VideoViewModel("http://www.nicovideo.jp/watch/" + SelectedItem.Entry.Id);
                }
                SelectedItem = null;
            }
        }


        public void Reflesh() {

            IsActive = true;
            Mylist.Clear();

            Task.Run(() => {

                Mylist = new DispatcherCollection<MylistListEntryViewModel>(DispatcherHelper.UIDispatcher);


                foreach(NicoNicoMylistData data in MylistViewModel.MylistInstance.GetMylist(Group.Id)) {

                    Mylist.Add(new MylistListEntryViewModel(this, data));
                }

                IsActive = false;
            });
        }

        void IDragSource.StartDrag(IDragInfo dragInfo) {

            dragInfo.Data = SelectedItem;
            dragInfo.Effects = System.Windows.DragDropEffects.All;
        }

        bool IDragSource.CanStartDrag(IDragInfo dragInfo) {

            return true;
        }

        void IDragSource.Dropped(IDropInfo dropInfo) {

        }

        void IDragSource.DragCancelled() {

            SelectedItem = null;
            Owner.Status = "";
        }
    }
}
