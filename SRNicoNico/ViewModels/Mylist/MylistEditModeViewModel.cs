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

using SRNicoNico.Models;
using System.Threading.Tasks;

namespace SRNicoNico.ViewModels {

    public class MylistEditModeViewModel : ViewModel {

        #region AllSelect変更通知プロパティ
        private bool _AllSelect;

        public bool AllSelect {
            get { return _AllSelect; }
            set {
                if(_AllSelect == value)
                    return;
                _AllSelect = value;


                foreach(MylistListEntryViewModel entry in List.Mylist) {

                    entry.IsChecked = value;
                }
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Process変更通知プロパティ
        private string _Process;

        public string Process {
            get { return _Process; }
            set { 
                if(_Process == value)
                    return;
                _Process = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region IsAnyoneChecked変更通知プロパティ

        public bool IsAnyoneChecked {
            get {
                foreach(MylistListEntryViewModel entry in List.Mylist) {

                    if(entry.IsChecked) {

                        return true;
                    }
                }
                return false;
            }
            set { 
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Status変更通知プロパティ
        private string _Status;

        public string Status {
            get { return _Status; }
            set { 
                if(_Status == value)
                    return;
                _Status = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region SelectedMylist変更通知プロパティ
        private DispatcherCollection<MylistListEntryViewModel> _SelectedMylist = new DispatcherCollection<MylistListEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<MylistListEntryViewModel> SelectedMylist {
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
        private DispatcherCollection<MylistListViewModel> _TargetMylist = new DispatcherCollection<MylistListViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<MylistListViewModel> TargetMylist {
            get { return _TargetMylist; }
            set {
                if(_TargetMylist == value)
                    return;
                _TargetMylist = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public MylistListViewModel List { get; set; }

        public MylistEditModeViewModel(MylistListViewModel vm) {

            List = vm;
        }

        //ダイアログ表示 Processは処理内容
        public void ShowDialog(string process) {

            Process = process;

            //リストをクリア
            SelectedMylist.Clear();

            //選択されているマイリストを追加
            foreach(MylistListEntryViewModel entry in List.Mylist) {

                if(entry.IsChecked) {

                    SelectedMylist.Add(entry);
                }
            }

            //ターゲットになるマイリストのリストをコンボボックスに追加
            foreach(MylistListViewModel list in List.Owner.MylistListCollection) {

                if(List != list) {

                    TargetMylist.Add(list);
                }
            }
            //ダイアログ表示
            App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.Contents.Mylist.EditConfirmDialog), this, TransitionMode.Modal));
        }

        //Processを処理
        public void DoProcess(MylistListViewModel list) {

            Status = "マイリストを" + Process + "しています";
            Task.Run(() => {

                //各種バックエンド呼び出し
                switch(Process) {
                    case "削除":
                        MylistViewModel.MylistInstance.DeleteMylist(SelectedMylist);
                        break;
                    case "移動":
                        MylistViewModel.MylistInstance.MoveMylist(SelectedMylist, list);
                        break;
                    case "コピー":
                        MylistViewModel.MylistInstance.CopyMylist(SelectedMylist, list);
                        break;
                }

                //削除なら削除したマイリストのみ更新 その他はソースとディスティネーションを更新
                if(Process == "削除") {

                    List.Refresh();
                } else {

                    List.Refresh();
                    list.Refresh();
                }

                Status = "マイリストを" + Process + "しました";
                List.CloseDialog();
            });
        }

        //マイリスト情報を更新
        public void UpdateMylist() {

            //とりあえずマイリストならエディットモードを終了するだけ
            if(List.Group.Id == "0") {

                List.EditMode = false;
                return;
            }

            //リストの名前と説明に変化が無かったらエディットモードを終了して終わり
            if(List.Group.BeforeName == List.Group.Name && List.Group.BeforeDescription == List.Group.Description) {

                List.EditMode = false;
                return;
            }

            //変化があったら変更APIを叩く
            Task.Run(() => {

                Status = "マイリスト更新中";
                MylistViewModel.MylistGroupInstance.UpdateMylistGroup(List.Group);
                List.Name = List.Group.Name;
                Status = "マイリストを更新しました";
                List.EditMode = false;
            });
        }
    }
}
