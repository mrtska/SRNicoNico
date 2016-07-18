using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text.RegularExpressions;

using System.Net.Http;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;

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

                    obj.Type = entry.SelectSingleNode("type").InnerText == "id" ? NGType.UserId : NGType.Word;
                    obj.Content = entry.SelectSingleNode("source").InnerText;

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


        //コメントをフィルタリングする アウトならtrue、セーフならfalse
        public static bool Filter(NicoNicoCommentEntry entry) {

            foreach(NGCommentEntry ng in Settings.Instance.NGList) {

                if(!ng.IsEnabled) {

                    continue;
                }

                switch(ng.Type) {
                    case NGType.RegEx:

                        if(Regex.Match(entry.Content, ng.Content).Success) {

                            return true;
                        }
                        break;
                    case NGType.UserId:

                        if(entry.UserId == ng.Content) {

                            return true;
                        }
                        break;
                    case NGType.Word:

                        if(entry.Content == ng.Content) {

                            return true;
                        }
                        break;
                    case NGType.WordContains:

                        if(entry.Content.Contains(ng.Content)) {

                            return true;
                        }
                        break;
                }

            }

            return false;

        }


    }

    public class NGCommentEntry : NotificationObject {

        //有効かどうか
        #region IsEnabled変更通知プロパティ
        private bool _IsEnabled;

        public bool IsEnabled {
            get { return _IsEnabled; }
            set { 
                if(_IsEnabled == value)
                    return;
                _IsEnabled = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        //ユーザーIDか文字列か
        #region Type変更通知プロパティ
        private NGType _Type;

        public NGType Type {
            get { return _Type; }
            set { 
                if(_Type == value)
                    return;
                _Type = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        //NG文字列またはID
        #region Content変更通知プロパティ
        private string _Content;

        public string Content {
            get { return _Content; }
            set { 
                if(_Content == value)
                    return;
                _Content = value;
                RaisePropertyChanged();
            }
        }
        #endregion
    }

    public enum NGType {

        UserId,
        Word,
        WordContains,
        RegEx

    }
    


}
