using Livet.Messaging;

namespace SRNicoNico.ViewModels {
    public class StartViewModel : TabItemViewModel {

        public double CurrentVersion {
            get { return App.ViewModelRoot.CurrentVersion; }
        }

        public StartViewModel() : base("スタート") {
        }

        public override bool CanShowHelp() {
            return true;
        }
        public override void ShowHelpView(InteractionMessenger Messenger) {

            Messenger.Raise(new TransitionMessage(typeof(Views.StartHelpView), this, TransitionMode.NewOrActive));
        }
    }
}
