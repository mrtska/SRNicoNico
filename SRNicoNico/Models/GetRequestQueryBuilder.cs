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
        /// クエリを追加する
        /// </summary>
        /// <param name="key">クエリのキー</param>
        /// <param name="value">クエリの値</param>
        /// <returns>this</returns>
        public GetRequestQueryBuilder AddQuery(string key, string value) {

            var raw = $"{key}={value}";

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

        /// <summary>
        /// クエリをビルドしてURLを取得する
        /// </summary>
        /// <returns>クエリが追加されたURL</returns>
        public string Build() {

            return TargetUrl;
        }
    }
}
