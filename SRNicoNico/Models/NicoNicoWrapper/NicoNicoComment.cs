using System;
using System.Collections.Generic;

using Livet;

using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

using Codeplex.Data;
using System.Net.Http;

using SRNicoNico.ViewModels;
using SRNicoNico.Models.NicoNicoViewer;
using System.Web;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoComment : NotificationObject {


		//スレッドキー取得API
		private const string GetThreadKeyApiUrl = "http://flapi.nicovideo.jp/api/getthreadkey?thread=";

        //ポストキー取得API
        private const string GetPostKeyApiUrl = "http://flapi.nicovideo.jp/api/getpostkey";

		//ウェイバックキー取得API
		private const string GetWayBackKeyApiUrl = "http://flapi.nicovideo.jp/api/getwaybackkey";

        //------
        private NicoNicoGetFlvData GetFlv;

        private VideoViewModel Video;
        //------

        //チケット 何に使うのかは謎
        private string Ticket;

        //コメント数
        private int LastRes;

        public NicoNicoComment(NicoNicoGetFlvData getFlv, VideoViewModel vm) {

            GetFlv = getFlv;
            Video = vm;
		}

        //コメント取得
        public List<NicoNicoCommentEntry> GetComment() {

            /**string threadKey = NicoNicoWrapperMain.GetSession().HttpClient.GetStringAsync(GetThreadKeyApiUrl + ThreadId).Result;

			if(IsPremium) {

				string waybackKey = NicoNicoWrapperMain.GetSession().HttpClient.GetStringAsync(GetWayBackKeyApiUrl + ThreadId).Result;
			}*/
            
            Video.CommentStatus = "取得中";
            var list = new List<NicoNicoCommentEntry>();


            //---ユーザーコメント取得---
            //コメント取得APIに渡すGETパラメーター
            var leaves = new GetRequestQuery(GetFlv.CommentServerUrl + "thread_leaves");
            leaves.AddQuery("thread", GetFlv.ThreadID);
            leaves.AddQuery("body", "0-" + ((GetFlv.Length / 60) + 1) + ":100,1000");
            leaves.AddQuery("user_id", GetFlv.UserId);
            leaves.AddQuery("userkey", GetFlv.UserKey);
            leaves.AddQuery("scores", "1");

            try {
                
                var a = NicoNicoWrapperMain.GetSession().GetAsync(leaves.TargetUrl).Result;

                var doc = new HtmlDocument();
                doc.LoadHtml2(a);

                var resultcode = doc.DocumentNode.SelectSingleNode("/packet/thread").Attributes["resultcode"].Value;


                //thread_leavesが失敗したら（コメント数が少ないと失敗しやすいっぽい？）
                if(resultcode == "11") {

                    Video.Status = "取得失敗（復帰中）";

                    var recv = new GetRequestQuery(GetFlv.CommentServerUrl + "thread");
                    recv.AddQuery("version", "20090904");
                    recv.AddQuery("thread", GetFlv.ThreadID);
                    recv.AddQuery("res_from", "-1000");

                    a = NicoNicoWrapperMain.GetSession().GetAsync(recv.TargetUrl).Result;
                    doc.LoadHtml2(a);
                }

                //公式動画
                if(resultcode == "9") {

                    Video.CommentStatus = "取得失敗（公式動画）復帰中";
                    var threadKey = NicoNicoWrapperMain.Session.GetAsync(GetThreadKeyApiUrl + GetFlv.ThreadID).Result;

                    var rec = new GetRequestQuery(leaves.TargetUrl);
                    rec.AddRawQuery(threadKey);

                    a = NicoNicoWrapperMain.GetSession().GetAsync(rec.TargetUrl).Result;
                    doc.LoadHtml2(a);

                }
                StoreEntry(doc, list);
                //------

                if(list.Count == 0) {

                    Video.CommentStatus = "取得失敗";
                    return null;
                }
                list.Sort();

                Ticket = doc.DocumentNode.SelectSingleNode("packet/thread").Attributes["ticket"].Value;
                LastRes = int.Parse(doc.DocumentNode.SelectSingleNode("packet/thread").Attributes["last_res"].Value);

                Video.CommentStatus = "取得完了";

                return list;

            } catch(RequestTimeout) {

                Video.CommentStatus = "取得失敗（タイムアウト）";
                return null;
            }
		}
        public List<NicoNicoCommentEntry> GetUploaderComment() {

            try {
                
                var list = new List<NicoNicoCommentEntry>();


                Video.CommentStatus = "投稿者コメント取得中";

                //---投稿者コメント取得---
                var thread = new GetRequestQuery(GetFlv.CommentServerUrl + "thread");
                thread.AddQuery("version", "20090904");
                thread.AddQuery("thread", GetFlv.ThreadID);
                thread.AddQuery("res_From", "-1000");
                thread.AddQuery("fork", "1");

                var a = NicoNicoWrapperMain.GetSession().GetAsync(GetFlv.CommentServerUrl + thread.TargetUrl).Result;

                var doc = new HtmlDocument();
                doc.LoadHtml2(a);

                StoreEntry(doc, list);
                //------

                if(list.Count == 0) {

                    Video.CommentStatus = "投稿者コメント取得失敗";
                    return null;
                }
                list.Sort();

                return list;
            } catch(RequestTimeout) {

                Video.CommentStatus = "投稿者コメント取得失敗（タイムアウト）";
                return null;
            }

        }



        public  void Post(string comment, string mail, string vpos) {

            Video.Status = "コメント投稿中";

            try {
                var query = new GetRequestQuery(GetPostKeyApiUrl);
                query.AddQuery("version_sub", "2");
                query.AddQuery("block_no", Math.Floor((decimal)(LastRes + 1) / 100).ToString());
                query.AddQuery("version", "1");
                query.AddQuery("yugi", "");
                query.AddQuery("device", "1");
                query.AddQuery("thread", GetFlv.ThreadID);

                var postkey = NicoNicoWrapperMain.Session.GetAsync(query.TargetUrl).Result;
                postkey = HttpUtility.UrlDecode(postkey);

                var chat = new GetRequestQuery(GetFlv.CommentServerUrl + "chat");
                chat.AddQuery("thread", GetFlv.ThreadID);
                chat.AddQuery("vpos", vpos);
                chat.AddQuery("mail", mail);
                chat.AddQuery("ticket", Ticket);
                chat.AddQuery("user_id", GetFlv.UserId);
                chat.AddRawQuery(postkey);
                if(GetFlv.IsPremium) {

                    chat.AddQuery("premium", "1");
                }
                chat.AddQuery("body", comment);

                var a = NicoNicoWrapperMain.Session.GetAsync(chat.TargetUrl).Result;

                var doc = new HtmlDocument();
                doc.LoadHtml2(a);

                var status = doc.DocumentNode.SelectSingleNode("packet/chat_result").Attributes["status"].Value;
                if(status != "0") {

                    Video.Status = "コメントの投稿に失敗しました";
                } else {

                    Video.Status = "コメントを投稿しました";
                }
                
               
            } catch(RequestTimeout) {

                Video.Status = "コメントの投稿に失敗しました（タイムアウト）";

            }
        }



        private void StoreEntry(HtmlDocument doc, List<NicoNicoCommentEntry> list) {



            var nodes = doc.DocumentNode.SelectNodes("/packet/chat");

            if(nodes == null) {

                return;
            }

            foreach(var node in nodes) {

                var attr = node.Attributes;

                //削除されていたら登録しない もったいないしね
                if(attr.Contains("deleted")) {

                    continue;
                }

                var entry = new NicoNicoCommentEntry();
                
                entry.No = int.Parse(attr["no"].Value);
                entry.Vpos = int.Parse(attr["vpos"].Value);
                entry.Date = long.Parse(attr["date"].Value);
                entry.UserId = attr.Contains("user_id") ? attr["user_id"].Value : "contributor";
                entry.Mail = attr.Contains("mail") ? attr["mail"].Value : "";
                entry.Content = System.Web.HttpUtility.HtmlDecode(node.InnerText);
                
                list.Add(entry);
            }
        }
	}


	public class NicoNicoCommentEntry : IComparable<NicoNicoCommentEntry> {

		//コメントナンバー
		public int No { get; set; }

		//コメント再生位置
		public int Vpos { get; set; }

		//コマンド
		public string Mail { get; set; }

		//コメントを投稿したユーザーID
		public string UserId { get; set; }

		//コメント
		public string Content { get; set; }

		//投稿日時 Unixタイム
		public long Date { get; set; }

        
		//Vposでソートする
		public int CompareTo(NicoNicoCommentEntry obj) {

            if(Vpos == obj.Vpos) {

                return No.CompareTo(obj.No);
            }

			return Vpos.CompareTo(obj.Vpos);
		}

        public string ToJson() {

            return DynamicJson.Serialize(this);
        }

    }


}
