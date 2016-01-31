using System;
using System.Collections.Generic;

using Livet;

using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

using Codeplex.Data;
using System.Net.Http;

using SRNicoNico.ViewModels;
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoComment : NotificationObject {


		//スレッドキー取得API
		private const string GetThreadKeyApiUrl = "http://flapi.nicovideo.jp/api/getthreadkey?thread=";

		//ウェイバックキー取得API
		private const string GetWayBackKeyApiUrl = "http://flapi.nicovideo.jp/api/getwaybackkey?thread=";

        //------
		//サーバーURL
		private string ServerUrl;

		//スレッドID
		private string ThreadId;

		//ユーザーID
		private string UserId;

		//プレミアムか否か
		private bool IsPremium;

		//動画の長さ
		private readonly uint Length;

        private VideoViewModel Video;
        //------

        //チケット 何に使うのかは謎
        private string Ticket;


        public NicoNicoComment(NicoNicoGetFlvData getFlv, VideoViewModel vm) {


			ServerUrl = getFlv.CommentServerUrl.OriginalString;

			ThreadId = getFlv.ThreadID;

			UserId = getFlv.UserId;

			IsPremium = getFlv.IsPremium;

			Length = getFlv.Length;

            Video = vm;
		}

        //コメント取得
        public List<NicoNicoCommentEntry> GetComment() {

            /**string threadKey = NicoNicoWrapperMain.GetSession().HttpClient.GetStringAsync(GetThreadKeyApiUrl + ThreadId).Result;

			if(IsPremium) {

				string waybackKey = NicoNicoWrapperMain.GetSession().HttpClient.GetStringAsync(GetWayBackKeyApiUrl + ThreadId).Result;
			}*/
            
            Video.Status = "ユーザーコメント取得中";
            var list = new List<NicoNicoCommentEntry>();


            //---ユーザーコメント取得---
            //コメント取得APIに渡すGETパラメーター
            string leaves = "thread_leaves?thread=" + ThreadId + "&body=0-" + Length / 60 + 1 + ":100,1000&scores=1";

            try {
                
                var a = NicoNicoWrapperMain.GetSession().GetAsync(ServerUrl + leaves).Result;

                var doc = new HtmlDocument();
                doc.LoadHtml2(a);

                var resultcode = doc.DocumentNode.SelectSingleNode("/packet/thread").Attributes["resultcode"].Value;


                //thread_leavesが失敗したら（コメント数が少ないと失敗しやすいっぽい？）
                if(resultcode == "11") {

                    Video.Status = "ユーザーコメント取得失敗（復帰中）";

                    var recv = "thread?version=20090904&thread=" + ThreadId + "&res_from=-1000";
                    a = NicoNicoWrapperMain.GetSession().GetAsync(ServerUrl + recv).Result;
                    doc.LoadHtml2(a);
                }

                //公式動画
                if(resultcode == "9") {

                    Video.Status = "ユーザーコメント取得失敗（公式動画）復帰中";
                    var threadKey = NicoNicoWrapperMain.GetSession().GetAsync(GetThreadKeyApiUrl + ThreadId).Result;

                    var recv = leaves + "&" + threadKey + "&user_id=" + UserId;
                    a = NicoNicoWrapperMain.GetSession().GetAsync(ServerUrl + recv).Result;
                    doc.LoadHtml2(a);

                }
                StoreEntry(doc, list);
                //------

                if(list.Count == 0) {

                    Video.Status = "コメント取得に失敗しました。";
                    return null;
                }
                list.Sort();

                Ticket = doc.DocumentNode.SelectSingleNode("packet/thread").Attributes["ticket"].Value;

                Video.Status = "コメント取得完了";

                return list;

            } catch(RequestTimeout) {

                Video.Status = "コメント取得に失敗しました。（タイムアウト）";
                return null;
            }
		}
        public List<NicoNicoCommentEntry> GetUploaderComment() {

            try {

                var list = new List<NicoNicoCommentEntry>();


                Video.Status = "投稿者コメント取得中";

                //---投稿者コメント取得---
                var thread = "thread?version=20090904&thread=" + ThreadId + "&res_from=-1000&fork=1";
                var a = NicoNicoWrapperMain.GetSession().GetAsync(ServerUrl + thread).Result;

                var doc = new HtmlDocument();
                doc.LoadHtml2(a);

                StoreEntry(doc, list);
                //------

                if(list.Count == 0) {

                    Video.Status = "投稿者コメント取得に失敗しました。";
                    return null;
                }
                list.Sort();

                return list;
            } catch(RequestTimeout) {

                Video.Status = "投稿者コメント取得に失敗しました。（タイムアウト）";
                return null;
            }

        }



        public  void Post(string comment, string mail, string vpos) {



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
