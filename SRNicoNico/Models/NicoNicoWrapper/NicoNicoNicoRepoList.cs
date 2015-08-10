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



        public ObservableSynchronizedCollection<NicoRepoListViewModel> GetNicoRepoList() {

            ObservableSynchronizedCollection<NicoRepoListViewModel> ret = new ObservableSynchronizedCollection<NicoRepoListViewModel>();


            NicoRepoListViewModel all = new NicoRepoListViewModel() { Id = "all", Title = "すべて" };
            NicoRepoListViewModel myself = new NicoRepoListViewModel() { Id = "myself", Title = "自分" };
            NicoRepoListViewModel user = new NicoRepoListViewModel() { Id = "user", Title = "お気に入りユーザー" };
            NicoRepoListViewModel chcom = new NicoRepoListViewModel() { Id = "chcom", Title = "チャンネル＆コミュニティ" };
            NicoRepoListViewModel mylist = new NicoRepoListViewModel() { Id = "mylist", Title = "マイリスト" };

            ret.Add(all);
            ret.Add(myself);
            ret.Add(user);
            ret.Add(chcom);
            ret.Add(mylist);
            ret = new ObservableSynchronizedCollection<NicoRepoListViewModel>(ret.Concat(GetUserDefinitionsNicoRepoList()));
            
            return ret;

        }

        //ユーザー定義の二コレポリストを取得
        private ObservableSynchronizedCollection<NicoRepoListViewModel> GetUserDefinitionsNicoRepoList() {

            //htmlからCSRFトークンを抜き出す
            string html = NicoNicoWrapperMain.GetSession().HttpClient.GetStringAsync(NicoRepoWebUrl).Result;

            //CSRFトークン
            string token = html.Substring(html.IndexOf("Mypage_globals.hash = \"") + 23, 60);

            string response = NicoNicoWrapperMain.GetSession().HttpClient.GetStringAsync(NicoRepoListApiUrl + token).Result;

            var json = DynamicJson.Parse(response);


            ObservableSynchronizedCollection<NicoRepoListViewModel> ret = new ObservableSynchronizedCollection<NicoRepoListViewModel>();

            foreach(var entry in json.nicorepolists) {

                NicoRepoListViewModel list = new NicoRepoListViewModel();
                list.Title = entry.title;
                list.Id = entry.id;

                ret.Add(list);
            }
            return ret;

        }




    }
}
