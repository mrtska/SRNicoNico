namespace SRNicoNico.Models {
    /// <summary>
    /// 設定項目
    /// </summary>
    public interface ISettings {

        /// <summary>
        /// セッション文字列
        /// </summary>
        string? UserSession { get; set; }

        /// <summary>
        /// WebViewで開くデフォルトページ
        /// </summary>
        string DefaultWebViewPageUrl { get; set; }

        /// <summary>
        /// アクセントカラー
        /// </summary>
        string AccentColor { get; set; }

        /// <summary>
        /// フォントファミリ
        /// </summary>
        string FontFamily { get; set; }

        /// <summary>
        /// 終了時に確認ダイアログを出すかどうか
        /// </summary>
        bool ShowExitConfirmDialog { get; set; }

        /// <summary>
        /// 現在の設定でアクセントを変更する
        /// </summary>
        void ChangeAccent();

        /// <summary>
        /// 現在の設定でフォントを変更する
        /// </summary>
        void ChangeFontFamily();
    }
}
