using System;
using System.Collections.Generic;
using System.Windows;
using Windows.Storage;

namespace SRNicoNico.Models {
    /// <summary>
    /// アプリの設定を管理する
    /// UWPのAPIを使用する
    /// </summary>
    public sealed class Settings : ISettings {

        private readonly IDictionary<string, object?> RoamingValues;

        /// <inheritdoc />
        public string? UserSession {
            get {
                return RoamingValues.TryGetValue(nameof(UserSession), out var value) ? value as string : null;
            }
            set {
                RoamingValues[nameof(UserSession)] = value;
            }
        }

        /// <inheritdoc />
        public string DefaultWebViewPageUrl {
            get {
                return RoamingValues.TryGetValue(nameof(DefaultWebViewPageUrl), out var value) && value != null ? (string)value : "https://www.nicovideo.jp/";
            }
            set {
                RoamingValues[nameof(DefaultWebViewPageUrl)] = value;
            }
        }

        /// <inheritdoc />
        public string AccentColor {
            get {
                return RoamingValues.TryGetValue(nameof(AccentColor), out var value) && value != null ? (string)value : "Orange";
            }
            set {
                RoamingValues[nameof(AccentColor)] = value;
            }
        }

        /// <inheritdoc />
        public string FontFamily {
            get {
                return RoamingValues.TryGetValue(nameof(FontFamily), out var value) && value != null ? (string)value : "Segoe UI";
            }
            set {
                RoamingValues[nameof(FontFamily)] = value;
            }
        }

        /// <inheritdoc />
        public bool ShowExitConfirmDialog {
            get {
                return RoamingValues.TryGetValue(nameof(ShowExitConfirmDialog), out var value) && value != null ? (bool)value : true;
            }
            set {
                RoamingValues[nameof(ShowExitConfirmDialog)] = value;
            }
        }

        /// <inheritdoc />
        public bool DisableABRepeat {
            get {
                return RoamingValues.TryGetValue(nameof(DisableABRepeat), out var value) && value != null ? (bool)value : false;
            }
            set {
                RoamingValues[nameof(DisableABRepeat)] = value;
            }
        }

        /// <inheritdoc />
        public bool AutomaticPlay {
            get {
                return RoamingValues.TryGetValue(nameof(AutomaticPlay), out var value) && value != null ? (bool)value : true;
            }
            set {
                RoamingValues[nameof(AutomaticPlay)] = value;
            }
        }

        /// <inheritdoc />
        public bool AlwaysShowSeekBar {
            get {
                return RoamingValues.TryGetValue(nameof(AlwaysShowSeekBar), out var value) && value != null ? (bool)value : false;
            }
            set {
                RoamingValues[nameof(AlwaysShowSeekBar)] = value;
            }
        }

        /// <inheritdoc />
        public bool ClickOnPause {
            get {
                return RoamingValues.TryGetValue(nameof(ClickOnPause), out var value) && value != null ? (bool)value : true;
            }
            set {
                RoamingValues[nameof(ClickOnPause)] = value;
            }
        }

        /// <inheritdoc />
        public bool DisableJumpCommand {
            get {
                return RoamingValues.TryGetValue(nameof(DisableJumpCommand), out var value) && value != null ? (bool)value : false;
            }
            set {
                RoamingValues[nameof(DisableJumpCommand)] = value;
            }
        }

        /// <inheritdoc />
        public bool UseResumePlay {
            get {
                return RoamingValues.TryGetValue(nameof(UseResumePlay), out var value) && value != null ? (bool)value : false;
            }
            set {
                RoamingValues[nameof(UseResumePlay)] = value;
            }
        }

        /// <inheritdoc />
        public int VideoSeekAmount {
            get {
                return RoamingValues.TryGetValue(nameof(VideoSeekAmount), out var value) && value != null ? (int)value : 5;
            }
            set {
                RoamingValues[nameof(VideoSeekAmount)] = value;
            }
        }

        public Settings() {

            RoamingValues = ApplicationData.Current.RoamingSettings.Values;
        }


        /// <inheritdoc />
        public void ChangeAccent() {

            var resourcePath = "pack://application:,,,/Themes/Accent/Orange.xaml";
            if (AccentColor == "Blue") {
                resourcePath = "pack://application:,,,/Themes/Accent/Blue.xaml";
            } else if (AccentColor == "Purple") {
                resourcePath = "pack://application:,,,/Themes/Accent/Purple.xaml";
            }

            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(resourcePath, UriKind.RelativeOrAbsolute) });
        }

        /// <inheritdoc />
        public void ChangeFontFamily() {

            Application.Current.Resources["NicoNicoViewerFontFamily"] = new System.Windows.Media.FontFamily(FontFamily);
        }
    }
}
