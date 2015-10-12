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
using System.Threading.Tasks;


namespace SRNicoNico.ViewModels {
    public class NewMylistViewModel : ViewModel {

        #region NewMylistName変更通知プロパティ
        private string _NewMylistName;

        public string NewMylistName {
            get { return _NewMylistName; }
            set {
                if(_NewMylistName == value)
                    return;
                _NewMylistName = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region NewMylistDescription変更通知プロパティ
        private string _NewMylistDescription;

        public string NewMylistDescription {
            get { return _NewMylistDescription; }
            set {
                if(_NewMylistDescription == value)
                    return;
                _NewMylistDescription = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public MylistViewModel Mylist { get; set; }

        public NewMylistViewModel(MylistViewModel vm) {

            Mylist = vm;
            NewMylistName = "新しいマイリスト";

            int index = 1;

            restart:
            foreach(MylistListViewModel list in vm.MylistListCollection) {

                if(list.Group.Name == NewMylistName) {

                    NewMylistName = "新しいマイリスト(" + index++ + ")";
                    goto restart;
                }
            }
        }

        //新しいマイリストを作成
        public void AddMylist() {

            Mylist.Status = "新しいマイリスト " + NewMylistName + " を作成しています";
            Task.Run(() => {

                MylistViewModel.MylistInstance.CreateMylistGroup(NewMylistName, NewMylistDescription);

                Mylist.Reflesh();
                Mylist.Status = "新しいマイリスト " + NewMylistName + " を作成しました";
                Mylist.CloseDialog();

            });

        }

    }
}
