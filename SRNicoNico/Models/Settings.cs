﻿using System;
using System.Collections.Generic;
using Windows.Storage;

namespace SRNicoNico.Models {
    /// <summary>
    /// アプリの設定を管理する
    /// UWPのAPIを使用する
    /// </summary>
    public sealed class Settings : ISettings {

        private readonly IDictionary<string, object?> RoamingValues;

        /// <summary>
        /// ユーザーセッション
        /// </summary>
        public string? UserSession {
            get {
                return RoamingValues.TryGetValue(nameof(UserSession), out var value) ? value as string : null;
            }
            set {
                RoamingValues[nameof(UserSession)] = value;
            }
        }

        /// <summary>
        /// WebViewで開くデフォルトページ
        /// </summary>
        public string DefaultWebViewPageUrl {
            get {
                return RoamingValues.TryGetValue(nameof(DefaultWebViewPageUrl), out var value) && value != null ? (string)value : "https://www.nicovideo.jp/";
            }
            set {
                RoamingValues[nameof(DefaultWebViewPageUrl)] = value;
            }
        }

        public Settings() {

            RoamingValues = ApplicationData.Current.RoamingSettings.Values;
        }
    }
}