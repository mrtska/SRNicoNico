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

using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Collections.ObjectModel;

using GongSolutions.Wpf.DragDrop;

namespace SRNicoNico.ViewModels {
    public sealed class MylistListViewModel : TabItemViewModel, IDragSource  {

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


        #region SortIndex変更通知プロパティ
        public int SortIndex {
            get { return Settings.Instance.PlayListOrder; }
            set { 
                if(Settings.Instance.PlayListOrder == value)
                    return;
                Settings.Instance.PlayListOrder = value;
                Sort(value);
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
            Sort(0);

        }

        /*
            <ComboBoxItem Content="登録が新しい順" />
            <ComboBoxItem Content="登録が古い順" />
            <ComboBoxItem Content="タイトル昇順" />
            <ComboBoxItem Content="タイトル降順" />
            <ComboBoxItem Content="マイリストコメント昇順" />
            <ComboBoxItem Content="マイリストコメント降順" />
            <ComboBoxItem Content="投稿が新しい順" />
            <ComboBoxItem Content="投稿が古い順" />
            <ComboBoxItem Content="再生数が多い順" />
            <ComboBoxItem Content="再生数が少ない順" />
            <ComboBoxItem Content="コメントが新しい順" />
            <ComboBoxItem Content="コメントが古い順" />
            <ComboBoxItem Content="コメントが多い順" />
            <ComboBoxItem Content="コメントが少ない順" />
            <ComboBoxItem Content="マイリスト登録が多い順" />
            <ComboBoxItem Content="マイリスト登録が少ない順" />
        */
        //ソート
        public void Sort(int index) {

            IOrderedEnumerable<MylistListEntryViewModel> sorted = null;

            if(Mylist == null) {

                return;
            }

            var tmp = Mylist.ToArray();
            
            //並び替え
            switch(index) {
                case 0:
                    sorted = tmp.OrderByDescending(r => r.Entry.CreateTime);
                    break;
                case 1:
                    sorted = tmp.OrderBy(r => r.Entry.CreateTime);
                    break;
                case 2:
                    sorted = tmp.OrderBy(r => r.Entry.Title);
                    break;
                case 3:
                    sorted = tmp.OrderByDescending(r => r.Entry.Title);
                    break;
                case 4:
                    sorted = tmp.OrderBy(r => r.Entry.Description);
                    break;
                case 5:
                    sorted = tmp.OrderByDescending(r => r.Entry.Description);
                    break;
                case 6:
                    sorted = tmp.OrderByDescending(r => r.Entry.FirstRetrieve);
                    break;
                case 7:
                    sorted = tmp.OrderBy(r => r.Entry.FirstRetrieve);
                    break;
                case 8:
                    sorted = tmp.OrderByDescending(r => r.Entry.ViewCounter);
                    break;
                case 9:
                    sorted = tmp.OrderBy(r => r.Entry.ViewCounter);
                    break;
                case 10:
                    sorted = tmp.OrderByDescending(r => r.Entry.UpdateTime);
                    break;
                case 11:
                    sorted = tmp.OrderBy(r => r.Entry.UpdateTime);
                    break;
                case 12:
                    sorted = tmp.OrderByDescending(r => r.Entry.CommentCounter);
                    break;
                case 13:
                    sorted = tmp.OrderBy(r => r.Entry.CommentCounter);
                    break;
                case 14:
                    sorted = tmp.OrderByDescending(r => r.Entry.MylistCounter);
                    break;
                case 15:
                    sorted = tmp.OrderBy(r => r.Entry.MylistCounter);
                    break;
            }

            //一度空にする
            Mylist.Clear();

            //ソートしたマイリストを再度追加
            foreach(var entry in sorted) {

                Mylist.Add(entry);
            }
        }

        //選択したマイリストを開く
        public void Open() {

            if(SelectedItem != null) {

                if(SelectedItem.Entry.Type == 0) {

                     NicoNicoOpener.Open("http://www.nicovideo.jp/watch/" + SelectedItem.Entry.Id);
                } else if(SelectedItem.Entry.Type == 5) {

                    System.Diagnostics.Process.Start("http://seiga.nicovideo.jp/watch/mg" + SelectedItem.Entry.Id);
                } else if(SelectedItem.Entry.Type == 6) {

                    System.Diagnostics.Process.Start("http://seiga.nicovideo.jp/watch/bk" + SelectedItem.Entry.Id);
                } else if(SelectedItem.Entry.Type == 11) {

                    //System.Diagnostics.Process.Start("http://ch.nicovideo.jp/" + SelectedItem.Entry);
                }
                SelectedItem = null;
            }
        }

        //更新
        public void Refresh() {

            IsActive = true;

            Task.Run(() => {

                Mylist.Clear();

                foreach(var data in MylistViewModel.MylistInstance.GetMylist(Group.Id)) {

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

        //プレイリストをこのマイリストから作る
        public void MakePlayList() {

            App.ViewModelRoot.AddTabAndSetCurrent(new PlayListViewModel(Mylist.ToList(), Name));
        }

        //マイリスト削除
        public void DeleteMylist() {
            
            Owner.Status = Group.Name + " を削除しています";
            Task.Run(() => {

                MylistViewModel.MylistGroupInstance.DeleteMylistGroup(Group.Id);
                CloseDialog();
                Owner.Refresh();
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
