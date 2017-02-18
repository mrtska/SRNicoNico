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
using System.Windows.Input;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class LiveNotifyViewModel : TabItemViewModel {

        private readonly NicoNicoLiveNotify NotifyInstance;


        #region NowLiveList変更通知プロパティ
        private DispatcherCollection<LiveNotifyEntryViewModel> _NowLiveList = new DispatcherCollection<LiveNotifyEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<LiveNotifyEntryViewModel> NowLiveList {
            get { return _NowLiveList; }
            set {
                if(_NowLiveList == value)
                    return;
                _NowLiveList = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region SelectedLive変更通知プロパティ
        private LiveNotifyEntryViewModel _SelectedLive;

        public LiveNotifyEntryViewModel SelectedLive {
            get { return _SelectedLive; }
            set {
                if(_SelectedLive == value)
                    return;
                _SelectedLive = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private Timer RefreshTimer;


        public LiveNotifyViewModel() : base("生放送通知") {

            NotifyInstance = new NicoNicoLiveNotify(this);
            Initialize();

            RefreshTimer = new Timer(new TimerCallback(o => {

                Initialize();

            }), null, Settings.Instance.RefreshInterval, Settings.Instance.RefreshInterval);
        }

        public  void UpdateTimer() {

            RefreshTimer.Change(0, Settings.Instance.RefreshInterval);
        }


        public async void Initialize() {


            IsActive = true;
            Status = "更新中";
            NowLiveList.Clear();

            var list = await NotifyInstance.GetLiveInformationAsync();

            if(list != null) {

                foreach(var entry in list) {

                    NowLiveList.Add(new LiveNotifyEntryViewModel(entry));
                }

                Badge = list.Count;
            } else {

                Badge = null;
            }

            IsActive = false;
            Status = "";
        }

        public void Refresh() {

            Initialize();
        }

        public override void KeyDown(KeyEventArgs e) {

            if(e.Key == Key.F5) {

                Refresh();
            }
        }

        public override bool CanShowHelp() {
            return false;
        }

        public override void ShowHelpView(InteractionMessenger Messenger) {

            Messenger.Raise(new TransitionMessage(typeof(Views.StartHelpView), this, TransitionMode.NewOrActive));
        }

    }
}
