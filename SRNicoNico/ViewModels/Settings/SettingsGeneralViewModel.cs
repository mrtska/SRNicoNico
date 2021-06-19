using System.Linq;
using System.Windows.Media;
using SRNicoNico.Models;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// 設定ページの一般タブのViewModel
    /// </summary>
    public class SettingsGeneralViewModel : TabItemViewModel {

        /// <summary>
        /// アクセントカラー
        /// </summary>
        public string AccentColor {
            get { return Settings.AccentColor; }
            set { 
                if (Settings.AccentColor == value)
                    return;
                Settings.AccentColor = value;
                RaisePropertyChanged();
                Settings.ChangeAccent();
            }
        }

        /// <summary>
        /// フォント名
        /// </summary>
        public FontFamily FontFamily {
            get { return Fonts.SystemFontFamilies.FirstOrDefault(f => f.Source == Settings.FontFamily) ?? new FontFamily("Segoe UI"); }
            set { 
                if (Settings.FontFamily == value.Source)
                    return;
                Settings.FontFamily = value.Source;
                RaisePropertyChanged();
                Settings.ChangeFontFamily();
            }
        }

        /// <summary>
        /// WebViewが開くデフォルトページ
        /// </summary>
        public string DefaultWebViewPageUrl {
            get { return Settings.DefaultWebViewPageUrl; }
            set { 
                if (Settings.DefaultWebViewPageUrl == value)
                    return;
                Settings.DefaultWebViewPageUrl = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 終了時に確認ダイアログを出すかどうか
        /// </summary>
        public bool ShowExitConfirmDialog {
            get { return Settings.ShowExitConfirmDialog; }
            set { 
                if (Settings.ShowExitConfirmDialog == value)
                    return;
                Settings.ShowExitConfirmDialog = value;
                RaisePropertyChanged();
            }
        }

        private readonly ISettings Settings;

        public SettingsGeneralViewModel(ISettings settings) : base("一般設定") {

            Settings = settings;
        }

        /// <summary>
        /// アクセントを変更する
        /// </summary>
        /// <param name="color">変更後の色</param>
        public void ChangeAccent(string color) {

            AccentColor = color;
        }
    }
}
