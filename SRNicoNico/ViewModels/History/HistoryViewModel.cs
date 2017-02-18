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
    public class HistoryViewModel : TabItemViewModel {


        private NicoNicoHistory HistoryInstance;




        #region AccountHistoryList変更通知プロパティ
        private DispatcherCollection<HistoryEntryViewModel> _AccountHistoryList = new DispatcherCollection<HistoryEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<HistoryEntryViewModel> AccountHistoryList {
            get { return _AccountHistoryList; }
            set { 
                if(_AccountHistoryList == value)
                    return;
                _AccountHistoryList = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region LocalHistoryList変更通知プロパティ
        private DispatcherCollection<HistoryEntryViewModel> _LocalHistoryList = new DispatcherCollection<HistoryEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<HistoryEntryViewModel> LocalHistoryList {
            get { return _LocalHistoryList; }
            set { 
                if(_LocalHistoryList == value)
                    return;
                _LocalHistoryList = value;
                value.PropertyChanged += (obj, e) => {

                    HistoryInstance.SaveLocalHistory(value.Select(o => o.Item).ToList());
                };
                RaisePropertyChanged();
            }
        }
        #endregion



        public HistoryViewModel() : base("視聴履歴") {

            HistoryInstance = new NicoNicoHistory(this);
            Initialize();
        }

        public async void Initialize() {

            IsActive = true;
            Status = "視聴履歴取得中";

            AccountHistoryList.Clear();
            var account = await HistoryInstance.GetAccountHistoryAsync();

            if(account != null) {

                foreach(var entry in account) {

                    AccountHistoryList.Add(new HistoryEntryViewModel(entry));
                }
            }

            var local = await HistoryInstance.GetLocalHistoryAsync();

            HistoryInstance.MergeHistories(account, local);

            local.Sort();
            local.Reverse();

            var collection = new DispatcherCollection<HistoryEntryViewModel>(DispatcherHelper.UIDispatcher);

            if(local != null) {

                foreach(var entry in local) {

                    collection.Add(new HistoryEntryViewModel(entry));
                }
                LocalHistoryList = collection;
            }
            HistoryInstance.SaveLocalHistory(local);

            IsActive = false;
        }

        public void Refresh() {

            Initialize();
        }


        public override void KeyDown(KeyEventArgs e) {

            if(e.Key == Key.F5) {

                Refresh();
                e.Handled = true;
            }
        }

        public override bool CanShowHelp() {
            return true;
        }
        public override void ShowHelpView(InteractionMessenger Messenger) {

            Messenger.Raise(new TransitionMessage(typeof(Views.HistoryHelpView), this, TransitionMode.NewOrActive));
        }

    }
}
