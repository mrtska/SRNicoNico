using GongSolutions.Wpf.DragDrop;
using Livet;
using Livet.Messaging;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class MylistViewModel : TabItemViewModel, IDropTarget {

        #region MylistList変更通知プロパティ
        private DispatcherCollection<TabItemViewModel> _MylistList = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<TabItemViewModel> MylistList {
            get { return _MylistList; }
            set {
                if(_MylistList == value)
                    return;
                _MylistList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SelectedList変更通知プロパティ
        private TabItemViewModel _SelectedList;

        public TabItemViewModel SelectedList {
            get { return _SelectedList; }
            set {
                if(_SelectedList == value)
                    return;
                _SelectedList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private NicoNicoMylist MylistInstance;

        public MylistViewModel() : base("マイリスト") {

            MylistInstance = new NicoNicoMylist(this);
        }

        public async void Initialize() {

            IsActive = true;
            Status = "マイリストグループを取得中";

            MylistList.Clear();

            //MylistList.Add(new MylistResultViewModel(this, MylistInstance));

            var groups = await MylistInstance.Group.GetMylistGroupAsync();

            if(groups == null) {

                return;
            }

            foreach(var group in groups) {

                MylistList.Add(new MylistResultViewModel(this, group, MylistInstance));
            }
            Status = "";
            IsActive = false;
        }

        public void Refresh() {

            Initialize();
        }

        //新しいマイリストを作る
        public async void AddMylist() {

            var vm = new NewMylistViewModel(this, MylistInstance);

            //Modalはウィンドウが閉じるまで処理がブロックされる
            App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.NewMylistView), vm, TransitionMode.Modal));

            if(!vm.IsCanceled) {

                Status = "マイリスト (" + vm.NewMylistName + ") を作成しています";

                var token = await MylistInstance.GetMylistTokenAsync();
                if(token == null || token.Length == 0) {

                    return;
                }
                if(await MylistInstance.Group.CreateMylistAsync(vm.NewMylistName, vm.NewMylistDescription, token)) {

                    Status = "マイリスト (" + vm.NewMylistName + ") を作成しました";
                    Refresh();
                }
            }
        }

        public override void KeyDown(KeyEventArgs e) {

            if(e.KeyboardDevice.Modifiers == ModifierKeys.Control) {

                if(e.Key == Key.F5) {

                    Refresh();
                    return;
                }
            }
            SelectedList?.KeyDown(e);
        }

        public void DragOver(IDropInfo dropInfo) {

            if (dropInfo.TargetItem is MylistResultViewModel vm) {

                //とりあえずマイリストにコピー/移動はさせない
                if (vm.IsDefList) {

                    Status = "とりあえずマイリストに移動/コピーは出来ません";
                    return;
                }

                var count = 0;
                if (dropInfo.Data is ICollection data) {
                    count = data.Count;
                } else {

                    count = 1;
                }

                Status = count + " アイテムを " + vm.Name + " に移動/コピー";
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public async void Drop(IDropInfo dropInfo) {

            Status = "";

            if (dropInfo.TargetItem is MylistResultViewModel vm) {
                var selectedList = new List<MylistResultEntryViewModel>();

                if (dropInfo.Data is ICollection data) {
                    foreach (var list in data) {

                        selectedList.Add((MylistResultEntryViewModel)list);
                    }
                } else {

                    selectedList.Add((MylistResultEntryViewModel)dropInfo.Data);
                }

                var operation = new MylistOperationViewModel(this, vm, selectedList);

                App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.MylistCopyOrMoveView), operation, TransitionMode.Modal));

                if (operation.IsCanceled) {

                    return;
                }

                var token = await MylistInstance.GetMylistTokenAsync();

                //マイリストコピー処理
                if (operation.Operation == MylistOperation.Copy) {

                    if (await MylistInstance.Item.CopyMylistAsync(selectedList, vm.Group, token)) {

                        foreach (var entry in selectedList) {

                            //同じIDのマイリストがあったらコピーしない
                            if (vm.MylistList.Where(e => e.Item.ItemId == entry.Item.ItemId).Count() == 0) {

                                //ターゲット側マイリストに追加
                                vm.MylistList.Add(entry);
                            }
                        }
                        vm.Sort(vm.SortIndex);
                    }
                    return;
                }

                //マイリスト移動処理
                if (operation.Operation == MylistOperation.Move) {

                    if (await MylistInstance.Item.MoveMylistAsync(selectedList, vm.Group, token)) {

                        foreach (var entry in selectedList) {

                            //同じIDのマイリストがあったら移動しない
                            if (vm.MylistList.Where(e => e.Item.ItemId == entry.Item.ItemId).Count() == 0) {

                                //ターゲット側マイリストに追加
                                vm.MylistList.Add(entry);
                            }
                            //元のリストから削除
                            entry.Owner.MylistList.Remove(entry);
                        }
                        vm.Sort(vm.SortIndex);
                    }
                    return;
                }
            }
        }

        public override bool CanShowHelp() {
            return true;
        }

        public override void ShowHelpView(InteractionMessenger Messenger) {

            Messenger.Raise(new TransitionMessage(typeof(Views.MylistHelpView), this, TransitionMode.NewOrActive));
        }
    }
}
