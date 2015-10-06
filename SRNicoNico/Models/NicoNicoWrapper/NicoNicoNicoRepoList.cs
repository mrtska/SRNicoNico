using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using Codeplex.Data;


namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoNicoRepoList : NotificationObject {


        private const string NicoRepoWebUrl = @"http://www.nicovideo.jp/my/top";

        private const string NicoRepoListApiUrl = @"http://www.nicovideo.jp/api/nicorepolist?token=";


        //すべてのニコレポリストを取得
        public List<NicoNicoNicoRepoListEntry> GetNicoRepoList() {

            List<NicoNicoNicoRepoListEntry> ret = new List<NicoNicoNicoRepoListEntry>();


            NicoNicoNicoRepoListEntry all = new NicoNicoNicoRepoListEntry("すべて", "all");
            NicoNicoNicoRepoListEntry myself = new NicoNicoNicoRepoListEntry("自分", "myself");
            NicoNicoNicoRepoListEntry user = new NicoNicoNicoRepoListEntry("お気に入りユーザー", "user");
            NicoNicoNicoRepoListEntry chcom = new NicoNicoNicoRepoListEntry("チャンネル&コミュニティ", "chcom");
            NicoNicoNicoRepoListEntry mylist = new NicoNicoNicoRepoListEntry("マイリスト", "mylist");

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

            //htmlからCSRFトークンを抜き出す
            string html = NicoNicoWrapperMain.GetSession().GetAsync(NicoRepoWebUrl).Result;

            //CSRFトークン
            string token = html.Substring(html.IndexOf("Mypage_globals.hash = \"") + 23, 60);

            string response = NicoNicoWrapperMain.GetSession().GetAsync(NicoRepoListApiUrl + token).Result;
            Console.WriteLine(NicoRepoListApiUrl + token);
            var json = DynamicJson.Parse(response);


            List<NicoNicoNicoRepoListEntry> ret = new List<NicoNicoNicoRepoListEntry>();

            foreach(var entry in json.nicorepolists) {

                NicoNicoNicoRepoListEntry list = new NicoNicoNicoRepoListEntry(entry.title, entry.id);

                ret.Add(list);
            }
            return ret;

        }
    }


    public class NicoNicoNicoRepoListEntry {


        public string Title { get; set; }
        public string Id { get; set; }

        public NicoNicoNicoRepoListEntry(string title, string id) {

            Title = title;
            Id = id;
        }

    }

}
