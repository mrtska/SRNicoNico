using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SRNicoNico.ViewModels;
using SRNicoNico.Models.NicoNicoViewer;
using HtmlAgilityPack;
using System.Web;
using System.Text.RegularExpressions;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoNicoRepo {

        private NicoRepoViewModel Owner;

        public NicoNicoNicoRepo(NicoRepoViewModel owner) {

            Owner = owner;
        }

        public async Task<NicoNicoNicoRepoResult> GetNicoRepoAsync(string type, string nextPage) {

            try {

                //ニコレポのhtmlを取得
                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync("http://www.nicovideo.jp/my/top/" + type + "?innerPage=1&mode=next_page" + ((nextPage == null) ? "" : "&last_timeline=" + nextPage));


                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var timeline = doc.DocumentNode.SelectNodes("//div[@class='timeline']/div");
                
                //無いとき 
                if(timeline == null) {

                    return null;
                }

                var ret = new NicoNicoNicoRepoResult();
                ret.Items = new List<NicoNicoNicoRepoResultEntry>();

                //ニコレポタイムラインを走査
                foreach(var entry in timeline) {

                    var item = new NicoNicoNicoRepoResultEntry();
                    var author = entry.SelectSingleNode("div[contains(@class, 'log-author ')]");

                    if(author != null) {

                        item.NicoRepoThumbNail = author.SelectSingleNode("a/img").Attributes["data-original"].Value;
                        item.AuthorUrl = author.SelectSingleNode("a").Attributes["href"].Value;
                    }

                    var content = entry.SelectSingleNode("div[@class='log-content']");

                    if(content != null) {


                        var body = content.SelectSingleNode("div[@class='log-body']");
                        if(body != null) {

                            item.Title = body.InnerHtml;
                        }

                        var detail = content.SelectSingleNode("div[contains(@class, 'log-details')]");

                        if(detail != null) {

                            item.Time = detail.SelectSingleNode("div/div/span/time")?.InnerText.Trim() ?? "";

                            item.ContentThumbNail = detail.SelectSingleNode("div[@class='log-target-thumbnail']/a/img")?.Attributes["data-original"].Value ?? "";

                            var target = detail.SelectSingleNode("div[@class='log-target-info']/a");
                            if(target != null) {

                                item.HasContent = true;
                                item.ContentTitle = HttpUtility.HtmlDecode(target.InnerText.Trim());
                                item.ContentUrl = target.Attributes["href"].Value;

                                NicoNicoUtil.ApplyLocalHistory(item);
                            }

                        }

                    }

                    if(item.ContentUrl == null) {

                        item.ContentUrl = item.AuthorUrl;
                    }

                    ret.Items.Add(item);
                }

                var next = doc.DocumentNode.SelectSingleNode("//a[@class='next-page-link']");

                if(next != null) {

                    ret.NextPage = Regex.Match(next.Attributes["href"].Value, @"\d+$").Value;
                } else {

                    ret.IsEnd = true;
                    ret.NextPage = null;
                }
                return ret;
            } catch(RequestFailed e) {

                if(e.FailedType == FailedType.Failed) {

                    Owner.Status = "ニコレポの取得に失敗しました";
                } else {

                    Owner.Status = "ニコレポの取得がタイムアウトになりました";
                }

                return null;
            }


        }





    }

    public class NicoNicoNicoRepoResult {

        //エントリリスト
        public List<NicoNicoNicoRepoResultEntry> Items { get; set; }

        //リストの終端まで来たかどうか
        public bool IsEnd { get; set; }

        //次のページのトークン IsEndがtrueならnull
        public string NextPage { get; set; }


    }

    public class NicoNicoNicoRepoResultEntry : IWatchable {

        //ニコレポのタイトル
        public string Title { get; set; }
        

        //ニコレポのサムネ
        public string NicoRepoThumbNail { get; set; }

        //ニコレポが発生した時間
        public string Time { get; set; }

        //そのニコレポを
        public string AuthorUrl { get; set; }

        //内容があるかどうか
        public bool HasContent { get; set; }

        //内容のURL
        public string ContentUrl { get; set; }

        //内容
        public string ContentTitle { get; set; }

        //内容のサムネ
        public string ContentThumbNail { get; set; }

        public bool IsWatched { get; set; }
    }
}
