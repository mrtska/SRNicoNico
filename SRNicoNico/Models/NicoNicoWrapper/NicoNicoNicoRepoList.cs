using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using Codeplex.Data;

using SRNicoNico.ViewModels;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoNicoRepoList : NotificationObject {


        private const string NicoRepoWebUrl = @"http://www.nicovideo.jp/my/top";

        private const string NicoRepoListApiUrl = @"http://www.nicovideo.jp/api/nicorepolist?token=";


        //すべてのニコレポリストを取得
        public List<NicoRepoListViewModel> GetNicoRepoList() {

            List<NicoRepoListViewModel> ret = new List<NicoRepoListViewModel>();


            NicoRepoListViewModel all = new NicoRepoListViewModel("すべて") { Id = "all" };
            NicoRepoListViewModel myself = new NicoRepoListViewModel("自分") { Id = "myself" };
            NicoRepoListViewModel user = new NicoRepoListViewModel("お気に入りユーザー") { Id = "user" };
            NicoRepoListViewModel chcom = new NicoRepoListViewModel("チャンネル&コミュニティ") { Id = "chcom" };
            NicoRepoListViewModel mylist = new NicoRepoListViewModel("マイリスト") { Id = "mylist" };

            ret.Add(all);
            ret.Add(myself);
            ret.Add(user);
            ret.Add(chcom);
            ret.Add(mylist);
            ret = new List<NicoRepoListViewModel>(ret.Concat(GetUserDefinitionsNicoRepoList()));
            
            return ret;
        }

        //ユーザー定義の二コレポリストを取得
        private List<NicoRepoListViewModel> GetUserDefinitionsNicoRepoList() {

            //htmlからCSRFトークンを抜き出す
            string html = NicoNicoWrapperMain.GetSession().HttpClient.GetStringAsync(NicoRepoWebUrl).Result;

            //CSRFトークン
            string token = html.Substring(html.IndexOf("Mypage_globals.hash = \"") + 23, 60);

            string response = NicoNicoWrapperMain.GetSession().HttpClient.GetStringAsync(NicoRepoListApiUrl + token).Result;

            var json = DynamicJson.Parse(response);


            List<NicoRepoListViewModel> ret = new List<NicoRepoListViewModel>();

            foreach(var entry in json.nicorepolists) {

                NicoRepoListViewModel list = new NicoRepoListViewModel(entry.title);
                list.Id = entry.id;

                ret.Add(list);
            }
            return ret;

        }




    }
}
