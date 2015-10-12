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


        #region EditMode変更通知プロパティ
        private bool _EditMode;

        public bool EditMode {
            get { return _EditMode; }
            set { 
                if(_EditMode == value)
                    return;
                _EditMode = value;
                if(value) {

                    Group.BeforeName = Group.Name;
                    Group.BeforeDescription = Group.Description;
                }
                RaisePropertyChanged();
            }
        }
        #endregion

        //リスト情報
        public NicoNicoMylistGroupData Group { get; private set; }

        //オーナー
        public MylistViewModel Owner { get; private set; }

        //エディットモード時
        public MylistEditModeViewModel EditModeViewModel { get; set; }

        public MylistListViewModel(MylistViewModel vm, NicoNicoMylistGroupData group, List<NicoNicoMylistData> list) : base(group.Name) {

            EditModeViewModel = new MylistEditModeViewModel(this);
            Owner = vm;
            Group = group;
            Mylist = new DispatcherCollection<MylistListEntryViewModel>(DispatcherHelper.UIDispatcher);
            foreach(NicoNicoMylistData data in list) {

                Mylist.Add(new MylistListEntryViewModel(this, data));
            }

        }

        //選択したマイリストを開く
        public void Open() {

            if(SelectedItem != null) {

                if(SelectedItem.Entry.Type == 0) {

                    new VideoViewModel("http://www.nicovideo.jp/watch/" + SelectedItem.Entry.Id);
                }
                SelectedItem = null;
            }
        }

        //更新
        public void Reflesh() {

            IsActive = true;
            Mylist.Clear();

            Task.Run(() => {

                Mylist = new DispatcherCollection<MylistListEntryViewModel>(DispatcherHelper.UIDispatcher);

                foreach(NicoNicoMylistData data in MylistViewModel.MylistInstance.GetMylist(Group.Id)) {

                    Mylist.Add(new MylistListEntryViewModel(this, data));
                }

                //エディットモードの情報をクリア
                EditModeViewModel.AllSelect = false;
                EditModeViewModel.IsAnyoneChecked = false;
                IsActive = false;
            });
        }

        //マイリスト削除ダイアログ表示
        public void ShowDeleteDialog() {

            App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.Contents.Mylist.DeleteMylistDialog), this, TransitionMode.Modal));
        }

        //マイリスト削除
        public void DeleteMylist() {

            Owner.Status = Group.Name + " を削除しています";
            Task.Run(() => {

                MylistViewModel.MylistInstance.DeleteMylistGroup(Group.Id);
                CloseDialog();
                Owner.Reflesh();
                Owner.Status = Group.Name + " を削除しました";

            });
        }

        //ドラッグ開始
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
