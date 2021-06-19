using SRNicoNico.Models;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// 設定ページの動画タブのViewModel
    /// </summary>
    public class SettingsVideoViewModel : TabItemViewModel {

        /// <summary>
        /// 自動再生するかどうか
        /// </summary>
        public bool AutomaticPlay {
            get { return Settings.AutomaticPlay; }
            set {
                if (Settings.AutomaticPlay == value)
                    return;
                Settings.AutomaticPlay = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// シークバーを常に表示するかどうか
        /// </summary>
        public bool AlwaysShowSeekBar {
            get { return Settings.AlwaysShowSeekBar; }
            set {
                if (Settings.AlwaysShowSeekBar == value)
                    return;
                Settings.AlwaysShowSeekBar = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 動画をクリックした時に一時停止するかどうか
        /// </summary>
        public bool ClickOnPause {
            get { return Settings.ClickOnPause; }
            set {
                if (Settings.ClickOnPause == value)
                    return;
                Settings.ClickOnPause = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// ジャンプコマンドを無効にするかどうか
        /// </summary>
        public bool DisableJumpCommand {
            get { return Settings.DisableJumpCommand; }
            set {
                if (Settings.DisableJumpCommand == value)
                    return;
                Settings.DisableJumpCommand = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// レジューム再生を使うかどうか
        /// </summary>
        public bool UseResumePlay {
            get { return Settings.UseResumePlay; }
            set {
                if (Settings.UseResumePlay == value)
                    return;
                Settings.UseResumePlay = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// カーソルキーでのシーク量
        /// </summary>
        public int VideoSeekAmount {
            get { return Settings.VideoSeekAmount; }
            set {
                if (Settings.VideoSeekAmount == value)
                    return;
                Settings.VideoSeekAmount = value;
                RaisePropertyChanged();
            }
        }

        private readonly ISettings Settings;

        public SettingsVideoViewModel(ISettings settings) : base("動画設定") {

            Settings = settings;
        }

    }
}
