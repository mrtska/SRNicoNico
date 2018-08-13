using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.ViewModels {
    public class SettingsLiveViewModel : TabItemViewModel {

        #region RefreshInterval変更通知プロパティ

        public int RefreshInterval {
            get { return Settings.Instance.RefreshInterval / 60000; }
            set {
                if(Settings.Instance.RefreshInterval == value)
                    return;
                Settings.Instance.RefreshInterval = value * 60000;
                App.ViewModelRoot.LiveNotify.UpdateTimer();
                RaisePropertyChanged();
            }
        }
        #endregion

        public SettingsLiveViewModel() : base("生放送") {

        }
    }
}
