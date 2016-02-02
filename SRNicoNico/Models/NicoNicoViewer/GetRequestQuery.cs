using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoViewer {
    public class GetRequestQuery {

        //元のURL
        public string BaseUrl { get; private set; }

        //コイツに書き込んでいく
        public string TargetUrl { get; set; }


        public GetRequestQuery(string baseUrl) {

            BaseUrl = baseUrl;
            TargetUrl = baseUrl;
        }

        //クエリを追加
        public void AddQuery(string key, string value) {

            var query = key + "=" + value;

            if(!TargetUrl.Contains("?")) {

                TargetUrl += "?" + query;
            } else {

                TargetUrl += "&" + query;
            }
        }

        //key=valueを直接追加する場合はこれを呼ぶ
        public void AddRawQuery(string query) {

            if(!TargetUrl.Contains("?")) {

                TargetUrl += "?" + query;
            } else {

                TargetUrl += "&" + query;
            }
        }

    }
}
