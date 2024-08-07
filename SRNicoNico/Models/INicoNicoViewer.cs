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

        /// <summary>
        /// URLのタイプを判定する
        /// </summary>
        /// <param name="url">判定したいURL</param>
        /// <returns>判定結果</returns>
        NicoNicoUrlType DetectUrlType(string url);

        /// <summary>
        /// 指定したURLがNicoNicoViewerで開けるかどうかを確認する
        /// </summary>
        /// <param name="url">確認したいURL</param>
        /// <returns>NicoNicoViewerが開けるURLならTrue</returns>
        bool CanOpenUrl(string url);
    }

    public enum NicoNicoUrlType {
        /// <summary>
        /// 動画
        /// </summary>
        Video,
        /// <summary>
        /// ユーザー
        /// </summary>
        User,
        /// <summary>
        /// マイリスト
        /// </summary>
        Mylist,
        /// <summary>
        /// シリーズ
        /// </summary>
        Series,
        /// <summary>
        /// その他
        /// </summary>
        Other
    }
}
