using Livet.Messaging;
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.ViewModels {
    public class OverViewViewModel : TabItemViewModel {

        private TabItemViewModel Owner;

        public OverViewViewModel(TabItemViewModel vm) : base("このソフトウェアについて") {

            Owner = vm;
        }

        public async void CheckUpdate() {

            if(await UpdateChecker.IsUpdateAvailable()) {

                App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.UpdateFoundView), new UpdaterViewModel(), TransitionMode.Modal));
            } else {

                Owner.Status = "アップデートはありません。";
            }
        }
    }
}
