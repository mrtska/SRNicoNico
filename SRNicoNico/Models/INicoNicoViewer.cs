namespace SRNicoNico.Models {
    public interface INicoNicoViewer {
        /// <summary>
        /// NicoNicoViewerのバージョン
        /// </summary>
        double CurrentVersion { get; }

        /// <summary>
        /// 指定したURLに適したViewModelを作成してUIを表示する
        /// 最適なUIが無かった場合はWebViewで開かれる
        /// </summary>
        /// <param name="url">開きたいURL</param>
        void OpenUrl(string url);

    }
}
