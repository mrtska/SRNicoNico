using Livet.Messaging;

using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class HistoryViewModel : TabItemViewModel {

        public NicoNicoHistory Model { get; set; }

        public HistoryViewModel() : base("視聴履歴") {

            Model = new NicoNicoHistory();
            Initialize();
        }

        public async void Initialize() {

            IsActive = true;
            Status = "視聴履歴取得中";

            Status = await Model.GetAccountHistoryAsync();

            Status = await Model.GetLocalHistoryAsync();

            await Model.MergeHistoriesAsync();

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
