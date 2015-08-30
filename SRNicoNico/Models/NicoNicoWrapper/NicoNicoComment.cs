using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

namespace SRNicoNico.Models.NicoNicoWrapper {
	public class NicoNicoComment : NotificationObject {


		//スレッドキー取得API
		private const string GetThreadKeyApiUrl = "http://flapi.nicovideo.jp/api/getthreadkey?thread=";

		//ウェイバックキー取得API
		private const string GetWayBackKeyApiUrl = "http://flapi.nicovideo.jp/api/getwaybackkey?thread=";


		//サーバーURL
		private readonly Uri ServerUrl;

		//スレッドID
		private readonly string ThreadId;

		//ユーザーID
		private readonly string UserId;

		//プレミアムか否か
		private readonly bool IsPremium;

		//動画の長さ
		private readonly uint Length;



		public NicoNicoComment(NicoNicoGetFlvData getFlv) {


			ServerUrl = getFlv.CommentServerUrl;

			ThreadId = getFlv.ThreadID;

			UserId = getFlv.UserId;

			IsPremium = getFlv.IsPremium;

			Length = getFlv.Length;
		}


		public List<NicoNicoCommentEntry> GetComment() {

            /**string threadKey = NicoNicoWrapperMain.GetSession().HttpClient.GetStringAsync(GetThreadKeyApiUrl + ThreadId).Result;

			if(IsPremium) {

				string waybackKey = NicoNicoWrapperMain.GetSession().HttpClient.GetStringAsync(GetWayBackKeyApiUrl + ThreadId).Result;


				

			}*/

            List<NicoNicoCommentEntry> list = new List<NicoNicoCommentEntry>();


            //---ユーザーコメント取得---
            //コメント取得APIに渡すGETパラメーター
            string leaves = "thread_leaves?thread=" + ThreadId + "&body=0-" + Length / 60 + 1 + ":100";

			string response = NicoNicoWrapperMain.GetSession().HttpClient.GetStringAsync(ServerUrl + leaves).Result;

            StoreEntry(response, list);
            //------

            //---投稿者コメント取得---
            string thread = "thread?version=20090904&thread=" + ThreadId + "&res_from=-1000&fork=1";
            string authComment = NicoNicoWrapperMain.GetSession().HttpClient.GetStringAsync(ServerUrl + thread).Result;

            StoreEntry(authComment, list);

            //------


            list.Sort();

            return list;
		}


        private void StoreEntry(string response, List<NicoNicoCommentEntry> list) {


            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml2(response);

            var nodes = doc.DocumentNode.SelectNodes("/packet/chat");

            if(nodes == null) {

                return;
            }

            foreach(HtmlNode node in nodes) {

                var attr = node.Attributes;

                //削除されていたら登録しない もったいないしね
                if(attr.Contains("deleted")) {

                    continue;
                }


                NicoNicoCommentEntry entry = new NicoNicoCommentEntry();
                ;
                entry.No = int.Parse(attr["no"].Value);
                entry.Vpos = int.Parse(attr["vpos"].Value);
                entry.Date = long.Parse(attr["date"].Value);
                entry.UserId = attr.Contains("user_id") ? attr["user_id"].Value : "contributor";
                entry.Mail = attr.Contains("mail") ? attr["mail"].Value : "";
                entry.Content = node.InnerText;

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


			return Vpos.CompareTo(obj.Vpos);
		}

    }


}
