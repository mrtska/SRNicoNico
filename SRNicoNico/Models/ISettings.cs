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
    }
}
