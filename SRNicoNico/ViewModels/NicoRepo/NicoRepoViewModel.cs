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
    public class NicoRepoViewModel : TabItemViewModel {


        #region NicoRepoList変更通知プロパティ
        private DispatcherCollection<TabItemViewModel> _NicoRepoList = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<TabItemViewModel> NicoRepoList {
            get { return _NicoRepoList; }
            set {
                if(_NicoRepoList == value)
                    return;
                _NicoRepoList = value;
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

        private NicoNicoNicoRepo NicoRepoInstance;

        public NicoRepoViewModel() : base("ニコレポ") {

            NicoRepoInstance = new NicoNicoNicoRepo(this);
        }

        public void Initialize() {

            IsActive = true;
            Status = "ニコレポリストを取得中";
            NicoRepoList.Clear();
            NicoRepoList.Add(new NicoRepoResultViewModel("すべて", "all", NicoRepoInstance));
            NicoRepoList.Add(new NicoRepoResultViewModel("自分", "self", NicoRepoInstance));
            NicoRepoList.Add(new NicoRepoResultViewModel("フォロー中のユーザー", "followingUser", NicoRepoInstance));
            NicoRepoList.Add(new NicoRepoResultViewModel("チャンネル", "followingChannel", NicoRepoInstance));
            NicoRepoList.Add(new NicoRepoResultViewModel("コミュニティ", "followingCommunity", NicoRepoInstance));
            NicoRepoList.Add(new NicoRepoResultViewModel("マイリスト", "followingMylist", NicoRepoInstance));

            Status = "";
            IsActive = false;
        }

        public void Refresh() {

            Initialize();
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

        public override bool CanShowHelp() {
            return true;
        }
        public override void ShowHelpView(InteractionMessenger Messenger) {

            Messenger.Raise(new TransitionMessage(typeof(Views.NicoRepoHelpView), this, TransitionMode.NewOrActive));
        }
    }
}
