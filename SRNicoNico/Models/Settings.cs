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
