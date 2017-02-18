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
using SRNicoNico.Models.NicoNicoViewer;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class PublicMylistViewModel : TabItemViewModel {

        internal readonly NicoNicoPublicMylist PublicMylistInstance;

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
        private DispatcherCollection<PublicMylistEntryViewModel> _MylistList = new DispatcherCollection<PublicMylistEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<PublicMylistEntryViewModel> MylistList {
            get { return _MylistList; }
            set {
                if(_MylistList == value)
                    return;
                _MylistList = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region MylistInfo変更通知プロパティ
        private NicoNicoPublicMylistGroupEntry _MylistInfo;

        public NicoNicoPublicMylistGroupEntry MylistInfo {
            get { return _MylistInfo; }
            set { 
                if(_MylistInfo == value)
                    return;
                _MylistInfo = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public PublicMylistViewModel(string url) : base("公開マイリスト") {

            PublicMylistInstance = new NicoNicoPublicMylist(this, url);
        }

        

        public async void Initialize() {

            IsActive = true;
            MylistList.Clear();

            var mylist = await PublicMylistInstance.GetMylistAsync();

            MylistInfo = mylist;

            Name += "\n" + mylist.Name;

            if(mylist.Data != null) {

                foreach(var entry in mylist.Data) {

                    MylistList.Add(new PublicMylistEntryViewModel(this, entry));
                }
            }
            if(MylistList.Count == 0) {

                IsEmpty = true;
            }

            IsActive = false;
            Sort(SortIndex);
        }
        public void Refresh() {


            Initialize();
        }

        public void MakePlayList() {

            var filteredList = MylistList.Where(e => e.Item is NicoNicoMylistVideoEntry).Select(e => e.Item);

            if(filteredList.Count() == 0) {

                Status = "連続再生できるマイリストがありません";
                return;
            }

            var vm = new PlayListViewModel(MylistInfo.Name, filteredList);
            App.ViewModelRoot.MainContent.AddUserTab(vm);
        }

        public void Close() {

            App.ViewModelRoot.MainContent.RemoveUserTab(this);
        }

        public override void KeyDown(KeyEventArgs e) {

            if(e.KeyboardDevice.Modifiers == ModifierKeys.Control) {

                if(e.Key == Key.W) {

                    Close();
                    return;
                }
                if(e.Key == Key.F5) {

                    Refresh();
                    return;
                }
            }
        }
        public void Sort(int index) {

            IOrderedEnumerable<PublicMylistEntryViewModel> sorted = null;

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
    }
}
