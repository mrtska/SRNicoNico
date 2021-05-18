using System;

namespace SRNicoNico.Models {
    /// <summary>
    /// GETクエリのURLをビルドする便利クラス
    /// </summary>
    public class GetRequestQueryBuilder {

        private readonly string BaseUrl;
        private string TargetUrl;

        public GetRequestQueryBuilder(string baseUrl) {
 
            BaseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            TargetUrl = BaseUrl;
        }

        /// <summary>
        /// 生のクエリを追加する
        /// 既に=で分割されている文字列
        /// </summary>
        /// <param name="rawQuery">=を含むkeyとvalueのペア</param>
        /// <returns>this</returns>
        public GetRequestQueryBuilder AddRawQuery(string rawQuery) {

            if (!TargetUrl.Contains("?")) {

                TargetUrl += $"?{rawQuery}";
            } else {

                TargetUrl += $"&{rawQuery}";
            }
            return this;
        }

        /// <summary>
        /// クエリを追加する
        /// </summary>
        /// <param name="key">クエリのキー</param>
        /// <param name="value">クエリの値</param>
        /// <returns>this</returns>
        public GetRequestQueryBuilder AddQuery(string key, string value) {

            var raw = $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}";

            if (!TargetUrl.Contains("?")) {

                TargetUrl += $"?{raw}";
            } else {

                TargetUrl += $"&{raw}";
            }
            return this;
        }

        public GetRequestQueryBuilder AddQuery(string key, int value) {

            return AddQuery(key, value.ToString());
        }
        public GetRequestQueryBuilder AddQuery(string key, long value) {

            return AddQuery(key, value.ToString());
        }
        public GetRequestQueryBuilder AddQuery(string key, float value) {

            return AddQuery(key, value.ToString());
        }
        public GetRequestQueryBuilder AddQuery(string key, double value) {

            return AddQuery(key, value.ToString());
        }
        public GetRequestQueryBuilder AddQuery(string key, bool value) {

            return AddQuery(key, value.ToString().ToLower());
        }

        /// <summary>
        /// クエリをビルドしてURLを取得する
        /// </summary>
        /// <returns>クエリが追加されたURL</returns>
        public string Build() {

            return TargetUrl;
        }
    }
}
