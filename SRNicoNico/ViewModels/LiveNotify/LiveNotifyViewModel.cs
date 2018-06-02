using Livet.Messaging;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Threading;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class LiveNotifyViewModel : TabItemViewModel {

        public NicoNicoLiveNotify Model { get; set; }

        private Timer RefreshTimer;


        public LiveNotifyViewModel() : base("生放送通知") {

            Model = new NicoNicoLiveNotify();
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
            Status = await Model.GetLiveInformationAsync();

            if(Model.NowLiveList.Count != 0) {

                Badge = Model.NowLiveList.Count;
            } else {

                Badge = null;
            }
            IsActive = false;
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
