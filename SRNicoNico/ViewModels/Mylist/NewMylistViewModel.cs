using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
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


        public bool IsCanceled { get; set; }

        private NicoNicoMylist MylistInstance;



        public NewMylistViewModel(MylistViewModel owner, NicoNicoMylist inst) {

            MylistInstance = inst;
            NewMylistName = "新しいマイリスト";

            int index = 1;

            //新しいマイリストを作る時にデフォルトで入っている名前を決める
            //既に(1)とかがあったら(2)にするみたいな処理
            restart:
            foreach(var list in owner.MylistList) {

                if(list.Name == NewMylistName) {

                    NewMylistName = "新しいマイリスト(" + index++ + ")";
                    goto restart;
                }
            }
        }

        public void MakeMylist() {

            IsCanceled = false;
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
