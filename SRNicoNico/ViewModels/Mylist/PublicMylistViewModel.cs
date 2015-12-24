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
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class PublicMylistViewModel : TabItemViewModel {

        private NicoNicoPublicMylist PublicMylist;


        #region MylistData変更通知プロパティ
        private NicoNicoPublicMylistEntry _MylistData;

        public NicoNicoPublicMylistEntry MylistData {
            get { return _MylistData; }
            set { 
                if(_MylistData == value)
                    return;
                _MylistData = value;
                RaisePropertyChanged();
            }
        }
        #endregion

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
        private int _SortIndex;

        public int SortIndex {
            get { return _SortIndex; }
            set {
                if(_SortIndex == value)
                    return;
                _SortIndex = value;
                Sort(value);
                RaisePropertyChanged();
            }
        }
        #endregion


        private string MylistId;

        public PublicMylistViewModel(string url) : base("読込中") {

            MylistId = url.Substring(31);
            PublicMylist = new NicoNicoPublicMylist(url);

            App.ViewModelRoot.TabItems.Add(this);
            App.ViewModelRoot.SelectedTab = this;
        }
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

        public void OpenBrowser() {

            System.Diagnostics.Process.Start("http://www.nicovideo.jp/mylist/" + MylistId);
        }

        public void Initialize() {

            Task.Run(() => {

                MylistData = PublicMylist.GetMylist();
                Name = MylistData.MylistName;
                Mylist = new DispatcherCollection<MylistListEntryViewModel>(DispatcherHelper.UIDispatcher);

                foreach(var entry in MylistData.Data) {

                    Mylist.Add(entry);
                }
                Sort(SortIndex);

            });
        }
        //選択したマイリストを開く
        public void Open() {

            if(SelectedItem != null) {

                if(SelectedItem.Entry.Type == 0) {

                    new VideoViewModel("http://www.nicovideo.jp/watch/" + SelectedItem.Entry.Id);
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

        public void Close() {

            App.ViewModelRoot.RemoveTabAndLastSet(this);
        }
        public void Reflesh() {

            Initialize();
        }

        public override void KeyDown(KeyEventArgs e) {

            if(e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.W) {

                Close();
            } else if(e.Key == Key.F5) {

                Reflesh();
            }

        }
    }
}
