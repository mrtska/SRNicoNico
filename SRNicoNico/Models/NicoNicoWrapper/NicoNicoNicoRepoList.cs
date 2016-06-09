using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using Codeplex.Data;


namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoNicoRepoList : NotificationObject {

        //WebURL
        private const string NicoRepoWebUrl = @"http://www.nicovideo.jp/my/top";

        //ユーザー定義ニコレポリストを取得するAPI
        private const string NicoRepoListApiUrl = @"http://www.nicovideo.jp/api/nicorepolist?token=";

        //すべてのニコレポリストを取得
        public List<NicoNicoNicoRepoListEntry> GetNicoRepoList() {

            var ret = new List<NicoNicoNicoRepoListEntry>();


            var all = new NicoNicoNicoRepoListEntry("すべて", "all");
            var myself = new NicoNicoNicoRepoListEntry("自分", "myself");
            var user = new NicoNicoNicoRepoListEntry("お気に入りユーザー", "user");
            var chcom = new NicoNicoNicoRepoListEntry("チャンネル&コミュニティ", "chcom");
            var mylist = new NicoNicoNicoRepoListEntry("マイリスト", "mylist");

            ret.Add(all);
            ret.Add(myself);
            ret.Add(user);
            ret.Add(chcom);
            ret.Add(mylist);
            ret = new List<NicoNicoNicoRepoListEntry>(ret.Concat(GetUserDefinitionNicoRepoList()));
            
            return ret;
        }

        //ユーザー定義の二コレポリストを取得
        private List<NicoNicoNicoRepoListEntry> GetUserDefinitionNicoRepoList() {

            try {

                //htmlからCSRFトークンを抜き出す
                var html = NicoNicoWrapperMain.Session.GetAsync(NicoRepoWebUrl).Result;

                //CSRFトークン
                var token = html.Substring(html.IndexOf("Mypage_globals.hash = \"") + 23, 60);

                var response = NicoNicoWrapperMain.Session.GetAsync(NicoRepoListApiUrl + token).Result;
                Console.WriteLine(NicoRepoListApiUrl + token);
                var json = DynamicJson.Parse(response);


                var ret = new List<NicoNicoNicoRepoListEntry>();
                foreach(var entry in json.nicorepolists) {

                    var list = new NicoNicoNicoRepoListEntry(entry.title, entry.id);

                    ret.Add(list);
                }
                return ret;
            } catch(Exception) {

                return new List<NicoNicoNicoRepoListEntry>();
            }
  

        }
    }


    public class NicoNicoNicoRepoListEntry {

        //ニコレポリストの名前
        public string Title { get; set; }

        //ニコレポリストID
        public string Id { get; set; }

        public NicoNicoNicoRepoListEntry(string title, string id) {

            Title = title;
            Id = id;
        }

    }

}
