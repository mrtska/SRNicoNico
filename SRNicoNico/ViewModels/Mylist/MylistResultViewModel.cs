using Livet;
using Livet.Messaging;
using Livet.Messaging.Windows;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class MylistResultViewModel : TabItemViewModel {

        internal readonly NicoNicoMylist MylistInstance;

        #region Group変更通知プロパティ
        private NicoNicoMylistGroupEntry _Group;

        public NicoNicoMylistGroupEntry Group {
            get { return _Group; }
            set { 
                if(_Group == value)
                    return;
                _Group = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SelectedIndex変更通知プロパティ
        private int _SelectedIndex = 0;

        public int SelectedIndex {
            get { return _SelectedIndex; }
            set { 
                if(_SelectedIndex == value)
                    return;
                _SelectedIndex = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region IsEmpty変更通知プロパティ
        private bool _IsEmpty;

        public bool IsEmpty {
            get { return _IsEmpty; }
            set { 
                if(_IsEmpty == value)
                    return;
                _IsEmpty = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region MylistList変更通知プロパティ
        private DispatcherCollection<MylistResultEntryViewModel> _MylistList = new DispatcherCollection<MylistResultEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<MylistResultEntryViewModel> MylistList {
            get { return _MylistList; }
            set { 
                if(_MylistList == value)
                    return;
                _MylistList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region IsEditMode変更通知プロパティ
        private bool _IsEditMode = false;

        public bool IsEditMode {
            get { return _IsEditMode; }
            set { 
                if(_IsEditMode == value)
                    return;
                _IsEditMode = value;
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

        #region IsDefList変更通知プロパティ
        private bool _IsDefList = false;

        public bool IsDefList {
            get { return _IsDefList; }
            set { 
                if(_IsDefList == value)
                    return;
                _IsDefList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private MylistViewModel Owner;

        public MylistResultViewModel(MylistViewModel owner, NicoNicoMylistGroupEntry group, NicoNicoMylist mylist) : base(group.Name) {

            Owner = owner;
            MylistInstance = mylist;
            Group = group;
        }

        //グループを指定しなかったらとりあえずマイリストにしちゃう
        public MylistResultViewModel(MylistViewModel owner, NicoNicoMylist mylist) : base("とりあえずマイリスト") {

            Owner = owner;
            MylistInstance = mylist;
            IsDefList = true;
        }

        public async void Initialize() {

            IsActive = true;
            MylistList.Clear();

            List<NicoNicoMylistEntry> list;
            if(IsDefList) {

                list = await MylistInstance.Item.GetDefListAsync();
            } else {

                list = await MylistInstance.Item.GetMylistAsync(Group);
            }

            if (list != null) {

                foreach (var entry in list) {

                    MylistList.Add(new MylistResultEntryViewModel(this, entry));
                }
            }
            if (MylistList.Count == 0) {

                IsEmpty = true;
            }

            Sort(SortIndex);
            IsActive = false;
        }

        public void Sort(int index) {

            IOrderedEnumerable<MylistResultEntryViewModel> sorted = null;

            var tmp = MylistList.ToArray();
            //並び替え
            switch(index) {
                case 0:
                    sorted = tmp.OrderByDescending(r => r.Item.CreateTime);
                    break;
                case 1:
                    sorted = tmp.OrderBy(r => r.Item.CreateTime);
                    break;
                case 2:
                    sorted = tmp.OrderBy(r => r.Item.Title);
                    break;
                case 3:
                    sorted = tmp.OrderByDescending(r => r.Item.Title);
                    break;
                case 4:
                    sorted = tmp.OrderBy(r => r.Item.Description);
                    break;
                case 5:
                    sorted = tmp.OrderByDescending(r => r.Item.Description);
                    break;
                case 6:
                    sorted = tmp.OrderByDescending(r => r.Item.FirstRetrieve);
                    break;
                case 7:
                    sorted = tmp.OrderBy(r => r.Item.FirstRetrieve);
                    break;
                case 8:
                    sorted = tmp.OrderByDescending(r => r.Item.ViewCount);
                    break;
                case 9:
                    sorted = tmp.OrderBy(r => r.Item.ViewCount);
                    break;
                case 10:
                    sorted = tmp.OrderByDescending(r => r.Item.UpdateTime);
                    break;
                case 11:
                    sorted = tmp.OrderBy(r => r.Item.UpdateTime);
                    break;
                case 12:
                    sorted = tmp.OrderByDescending(r => r.Item.CommentCount);
                    break;
                case 13:
                    sorted = tmp.OrderBy(r => r.Item.CommentCount);
                    break;
                case 14:
                    sorted = tmp.OrderByDescending(r => r.Item.MylistCount);
                    break;
                case 15:
                    sorted = tmp.OrderBy(r => r.Item.MylistCount);
                    break;
            }
            //一度空にする
            MylistList.Clear();

            //ソートしたマイリストを再度追加
            foreach(var entry in sorted) {

                MylistList.Add(entry);
            }
        }

        public void Refresh() {

            Initialize();
        }
        //マイリスト自体を削除
        public void DeleteMylist() {

            App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.RemoveMylistView), this, TransitionMode.Modal));
        }
        //実際の削除処理
        public async void DeleteMylistCore() {

            if(await MylistInstance.Group.DeleteMylistAsync(Group, await MylistInstance.GetMylistTokenAsync())) {

                //マイリストを削除したらリストから消す
                Owner.MylistList.Remove(this);
            }
            DeleteCancel();
        }
        public void DeleteCancel() {

            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "RemoveMylist"));
        }

        //マイリストを削除
        public async void ShowDeleteView(ICollection targetRaw) {

            var target = targetRaw.Cast<MylistResultEntryViewModel>();

            var operation = new MylistOperationViewModel(Owner, this, target);

            App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.MylistDeleteView), operation, TransitionMode.Modal));

            if(operation.IsCanceled || operation.Operation != MylistOperation.Delete) {

                return;
            }

            //削除が出来たらリストから消す
            if(await MylistInstance.Item.DeleteMylistAsync(operation.SelectedMylist, await MylistInstance.GetMylistTokenAsync())) {

                foreach(var entry in operation.SelectedMylist) {

                    //元のリストから削除
                    MylistList.Remove(entry);
                }
                Sort(SortIndex);
            }
        }

        public async void UpdateMylist() {

            if(!IsDefList) {

                if(Group.Description != Group.DescriptionOriginal || Group.Name != Group.NameOriginal) {

                    await MylistInstance.Group.UpdateMylistAsync(Group, await MylistInstance.GetMylistTokenAsync());
                    Name = Group.Name;
                }
            }

            IsEditMode = false;
        }

        public void MakePlayList() {

            var filteredList = MylistList.Where(e => e.Item is NicoNicoMylistVideoEntry).Select(e => e.Item);

            if(filteredList.Count() == 0) {

                Owner.Status = "連続再生できるマイリストがありません";
                return;
            }

            var vm = new PlayListViewModel(Group == null ? "とりあえずマイリスト" : Group.Name, filteredList);
            App.ViewModelRoot.MainContent.AddUserTab(vm);
        }

        public override void KeyDown(KeyEventArgs e) {

            if(e.Key == Key.F5) {

                Refresh();
            }
        }
    }
}
