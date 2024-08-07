using System;
using System.Net;
using System.Net.Http;

namespace SRNicoNico.Models {
    /// <summary>
    /// APIアクセス時に正常なレスポンスが帰ってこなかった時にスローされる例外
    /// 仕様変更や鯖落ちなど
    /// </summary>
    public class StatusErrorException : HttpRequestException {
        /// <summary>
        /// エラーになったリクエストのステータスコード
        /// 503などが入る想定
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        public StatusErrorException(HttpStatusCode statusCode) : base($"データ取得中にエラーが発生しました ステータス: {statusCode}") {

            StatusCode = statusCode;
        }
    }
}
