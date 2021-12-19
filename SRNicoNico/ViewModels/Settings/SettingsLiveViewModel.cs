using SRNicoNico.Models;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// 設定ページの生放送タブのViewModel
    /// </summary>
    public class SettingsLiveViewModel : TabItemViewModel {

        /// <summary>
        /// 生放送更新通知間隔
        /// </summary>
        public int RefreshInterval {
            get { return Settings.LiveNotifyRefreshInterval; }
            set { 
                if (Settings.LiveNotifyRefreshInterval == value)
                    return;
                // 0は拒否する
                if (value == 0) {
                    value = 5;
                }
                Settings.LiveNotifyRefreshInterval = value;
                MainContent.LiveNotify!.UpdateInterval();
                RaisePropertyChanged();
            }
        }

        private readonly ISettings Settings;
        private readonly MainContentViewModel MainContent;

        public SettingsLiveViewModel(ISettings settings, MainContentViewModel mainContent) : base("生放送設定") {

            Settings = settings;
            MainContent = mainContent;
        }

    }
}
