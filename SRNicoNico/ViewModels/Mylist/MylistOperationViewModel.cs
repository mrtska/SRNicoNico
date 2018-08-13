using Livet;
using Livet.Messaging.Windows;
using System.Collections.Generic;

namespace SRNicoNico.ViewModels {
    public enum MylistOperation {

        Copy,
        Move,
        Delete
    }

    public class MylistOperationViewModel : ViewModel {

        #region SelectedMylist変更通知プロパティ
        private DispatcherCollection<MylistResultEntryViewModel> _SelectedMylist = new DispatcherCollection<MylistResultEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<MylistResultEntryViewModel> SelectedMylist {
            get { return _SelectedMylist; }
            set { 
                if(_SelectedMylist == value)
                    return;
                _SelectedMylist = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region TargetMylist変更通知プロパティ
        private MylistResultViewModel _TargetMylist;

        public MylistResultViewModel TargetMylist {
            get { return _TargetMylist; }
            set { 
                if(_TargetMylist == value)
                    return;
                _TargetMylist = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public bool IsCanceled { get; set; }

        public MylistOperation Operation { get; private set; }


        public MylistOperationViewModel(MylistViewModel owner, MylistResultViewModel target,  IEnumerable<MylistResultEntryViewModel> collection) {

            foreach(var list in collection) {

                SelectedMylist.Add(list);
            }

            TargetMylist = target;

        }

        public void CopyMylist() {

            IsCanceled = false;
            Operation = MylistOperation.Copy;
            Close();
        }
        public void MoveMylist() {

            IsCanceled = false;
            Operation = MylistOperation.Move;
            Close();
        }
        public void DeleteMylist() {

            IsCanceled = false;
            Operation = MylistOperation.Delete;
            Close();
        }

        public void Cancel() {

            IsCanceled = true;
            Close();
        }

        public void Close() {

            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "NewMylist"));
        }
    }
}
