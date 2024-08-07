using SRNicoNico.Models;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// 設定ページのコメントタブのViewModel
    /// </summary>
    public class SettingsCommentViewModel : TabItemViewModel {

        /// <summary>
        /// かんたんコメントを無効にするかどうか
        /// </summary>
        public bool DisableEasyComment {
            get { return Settings.DisableEasyComment; }
            set {   
                if (Settings.DisableEasyComment == value)
                    return;
                Settings.DisableEasyComment = value;
                RaisePropertyChanged();
            }
        }

        private readonly ISettings Settings;

        public SettingsCommentViewModel(ISettings settings) : base("コメント設定") {

            Settings = settings;
        }
    }
}
