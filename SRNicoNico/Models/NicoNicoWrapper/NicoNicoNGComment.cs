using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using System.Net.Http;
using Livet;

namespace SRNicoNico.Models.NicoNicoWrapper {

    //ニコニコ本家のNG機能バックエンド
    public class NicoNicoNGComment : NotificationObject {

        private const string NGApi = "http://flapi.nicovideo.jp/api/configurengclient";



        public List<NGCommentEntry> GetNGClient() {

            var pair = new Dictionary<string, string>();
            pair["mode"] = "get";

            try {

                var ret = new List<NGCommentEntry>();

                var request = new HttpRequestMessage(HttpMethod.Post, NGApi);

                request.Content = new FormUrlEncodedContent(pair);

                var a = NicoNicoWrapperMain.Session.GetAsync(request).Result;

                var xml = new XmlDocument();
                xml.LoadXml(a);

                if(xml.SelectSingleNode("/response_ngclient").Attributes["status"].Value != "ok") {

                    throw new RequestFailed(FailedType.Failed);
                }

                foreach(XmlNode entry in xml.SelectNodes("/response_ngclient/ngclient")) {

                    var obj = new NGCommentEntry();

                    obj.Type = entry.SelectSingleNode("type").InnerText == "id" ? NGType.Id : NGType.Word;
                    obj.Content = entry.SelectSingleNode("source").InnerText;
                    obj.RegisterTime = entry.SelectSingleNode("register_time").InnerText;

                    ret.Add(obj);
                }

                return ret;
            } catch(RequestTimeout) {

                throw new RequestFailed(FailedType.TimeOut);
            }
        }


        //公式NG機能に登録する
        public void RegisterNGComment(NGType type, string threadId, string content, string token, string count) {

            var pair = new Dictionary<string, string>();
            pair["mode"] = "add";
            pair["language"] = "0";
            pair["thread_id"] = threadId;
            pair["type"] = type == NGType.Word ? "word" : "id";
            pair["token"] = token;
            pair["comments"] = count;
            pair["source"] = content;


            try {

                var request = new HttpRequestMessage(HttpMethod.Post, NGApi);

                request.Content = new FormUrlEncodedContent(pair);

                var a = NicoNicoWrapperMain.Session.GetAsync(request).Result;

                var xml = new XmlDocument();
                xml.LoadXml(a);
                if(xml.SelectSingleNode("/response_ngclient").Attributes["status"].Value != "ok") {

                    throw new RequestFailed(FailedType.Failed);
                }
            } catch(RequestTimeout) {

                throw new RequestFailed(FailedType.TimeOut);
            }
        }

        //公式NG機能から削除する
        public void UnregisterNGComment(NGType type, string content, string token) {

            var pair = new Dictionary<string, string>();
            pair["mode"] = "delete";
            pair["language"] = "0";
            pair["type"] = type == NGType.Word ? "word" : "id";
            pair["token"] = token;
            pair["source"] = content;

            try {

                var request = new HttpRequestMessage(HttpMethod.Post, NGApi);

                request.Content = new FormUrlEncodedContent(pair);

                var a = NicoNicoWrapperMain.Session.GetAsync(request).Result;

                var xml = new XmlDocument();
                xml.LoadXml(a);
                if(xml.SelectSingleNode("/response_ngclient").Attributes["status"].Value != "ok") {

                    throw new RequestFailed(FailedType.Failed);
                }
            } catch(RequestTimeout) {

                throw new RequestFailed(FailedType.TimeOut);
            }
        }





    }

    public class NGCommentEntry {

        //ユーザーIDか文字列か
        public NGType Type { get; set; }

        //NG文字列またはID
        public string Content { get; set; }

        //登録日時
        public string RegisterTime { get; set; }



    }

    public enum NGType {

        Id,
        Word
    }


}
