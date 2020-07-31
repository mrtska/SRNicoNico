using Codeplex.Data;
using HtmlAgilityPack;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.ViewModels;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoCommunity {

        private readonly string CommunityUrl;

        private readonly TabItemViewModel Owner;

        public NicoNicoCommunity(TabItemViewModel vm, string url) {

            Owner = vm;
            CommunityUrl = new Uri(url).AbsoluteUri;
        }
        public async Task<NicoNicoCommunityEntry> GetCommunityAsync() {

            try {

                Owner.Status = "コミュニティ情報取得中";

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(CommunityUrl);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var ret = new NicoNicoCommunityEntry();

                var header = doc.DocumentNode.SelectSingleNode("//header[@class='area-communityHeader']");

                if(header == null) {

                    return null;
                }

                ret.Id = Regex.Match(new Uri(CommunityUrl).AbsolutePath, @"(\d+)").Groups[1].Value;

                ret.CommunityUrl = CommunityUrl;
                ret.ThumbnailUrl = header.SelectSingleNode("div/div/div/a/img").Attributes["src"].Value;

                //コミュニティタイプ
                if(header.SelectSingleNode("div/div/div/div/img").Attributes["src"].Value.Contains("open")) {

                    ret.Status = NicoNicoCommunityStatus.Open;
                } else {

                    ret.Status = NicoNicoCommunityStatus.Close;
                }

                var data = header.SelectSingleNode("div/div/div[@class='communityData']");

                ret.OwnerUrl = data.SelectSingleNode("table/tr[1]/td/a").Attributes["href"].Value;
                ret.OwnerName = "<a href=\"" + ret.OwnerUrl + "\">" + data.SelectSingleNode("table/tr[1]/td/a").InnerText.Trim() + "</a>";
                ret.CommmunityName = HttpUtility.HtmlDecode(data.SelectSingleNode("h2/a").InnerText.Trim());
                ret.OpeningDate = data.SelectSingleNode("table/tr[2]/td").InnerText.Trim();

                var tags = data.SelectNodes("table/tr[3]/td/ul/li/a");

                ret.CommunityTags = new List<string>();

                if(tags != null) {

                    foreach(var tag in tags) {

                        ret.CommunityTags.Add(tag.InnerText.Trim());
                    }
                }


                var scale = header.SelectSingleNode("div/div/div/dl[@class='communityScale']");

                ret.CommunityLevel = int.Parse(scale.SelectSingleNode("dd[1]").InnerText.Trim());
                ret.CommunityMember = int.Parse(scale.SelectSingleNode("dd[2]/text()").InnerText.Trim());
                ret.CommunityMaxMember = int.Parse(Regex.Match(scale.SelectSingleNode("dd[2]/span[@class='max']").InnerText, @"\d+").Value);

                //お知らせ取得
                var noticeList = doc.DocumentNode.SelectNodes("//ul[@class='noticeList']/li");

                ret.CommunityNews = new List<NicoNicoCommunityNews>();
                if(noticeList != null) {

                    foreach(var notice in noticeList) {

                        var news = new NicoNicoCommunityNews();

                        news.Title = notice.SelectSingleNode("div/div/h3").InnerText.Trim();
                        news.Description = HyperLinkReplacer.Replace(notice.SelectSingleNode("p").InnerHtml);
                        news.Date = notice.SelectSingleNode("div/div/div/span").InnerText.Trim();

                        ret.CommunityNews.Add(news);
                    }
                }

                //プロフィール
                ret.CommunityProfile = HyperLinkReplacer.Replace(doc.DocumentNode.SelectSingleNode("//span[@id='profile_text_content']").InnerHtml);

                Owner.Status = "";
                return ret;
            } catch(RequestFailed) {

                Owner.Status = "コミュニティの取得に失敗しました";
                return null;
            }
        }

        public async Task<int> GetCommunityVideoCountAsync() {

            try {
                Owner.Status = "コミュニティ動画数を取得中";

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(CommunityUrl.Replace("community", "video"));

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var count = doc.DocumentNode.SelectSingleNode("//div[@class='pageCount']");

                Owner.Status = "";
                if(count != null) {

                    return int.Parse(count.SelectSingleNode("strong[1]").InnerText.Trim(), System.Globalization.NumberStyles.AllowThousands);
                } else {

                    return 0;
                }
            } catch(RequestFailed) {

                Owner.Status = "コミュニティ動画数の取得に失敗しました";
                return -1;
            }

        }

        public async Task<List<NicoNicoCommunityVideoEntry>> GetCommunityVideoAsync(string id, int page) {

            try {

                Owner.Status = "コミュニティ動画取得中";

                var query = new GetRequestQuery($"https://com.nicovideo.jp/api/v1/communities/{id}/contents/videos.json");
                query.AddQuery("limit", "20");
                query.AddQuery("offset", (page - 1) * 20);
                query.AddQuery("sort", "c");
                query.AddQuery("direction", "d");
                query.AddQuery("_", DateTimeOffset.Now.ToUnixTimeMilliseconds());

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(query.TargetUrl);


                var json = DynamicJson.Parse(a);

                if (json.meta.status != 200) {

                    return null;
                }
                var ret = new List<NicoNicoCommunityVideoEntry>();

                foreach (var video in json.data.contents) {

                    var entry = new NicoNicoCommunityVideoEntry();

                    var detail = video.SelectSingleNode("div[@class='videoContent']/div[@class='videoDetail']");
                    var status = video.SelectSingleNode("div[@class='videoStatus']");

                    entry.Title = detail.SelectSingleNode("span[@class='videoTitle']").InnerText.Trim();
                    entry.ViewCounter = int.Parse(detail.SelectSingleNode("ul/li[1]/span[2]").InnerText.Trim(), System.Globalization.NumberStyles.AllowThousands);
                    entry.CommentCounter = int.Parse(detail.SelectSingleNode("ul/li[2]/span[2]/strong").InnerText.Trim(), System.Globalization.NumberStyles.AllowThousands);

                    var mylist = detail.SelectSingleNode("ul/li[3]/a/span");

                    if(mylist != null) {

                        entry.MylistCounter = int.Parse(mylist.InnerText.Trim(), System.Globalization.NumberStyles.AllowThousands);
                    }


                    entry.ThumbnailUrl = video.SelectSingleNode("div[@class='videoContent']/div[@class='videoThumbnail']/a/img").Attributes["src"].Value;
                    if (entry.ThumbnailUrl.StartsWith("//")) {

                        entry.ThumbnailUrl = "https:" + entry.ThumbnailUrl;
                    }


                    entry.ContentUrl = video.SelectSingleNode("div[@class='videoContent']/div[@class='videoThumbnail']/a").Attributes["href"].Value;
                    entry.Cmsid = Regex.Match(entry.ContentUrl, @"\d+$").Value;

                    var length = video.SelectSingleNode("div[@class='videoContent']/div[@class='videoThumbnail']/span");

                    if(length != null) {

                        entry.Length = length.InnerText.Trim();
                    }

                    entry.FirstRetrieve = status.SelectSingleNode("span[@class='videoRegisterdDate']").InnerText;
                    entry.VideoStatus = status.SelectSingleNode("span[@class='videoPermission']").InnerHtml.Trim();

                    NicoNicoUtil.ApplyLocalHistory(entry);
                    ret.Add(entry);
                }

                Owner.Status = "";
                return ret;
            } catch(RequestFailed) {

                Owner.Status = "コミュニティ動画の取得に失敗しました";
                return null;
            }
        }

        public async Task<int> GetCommunityFollowerCountAsync() {


            try {

                Owner.Status = "コミュニティフォロワー数を取得中";

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(CommunityUrl.Replace("community", "member"));

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var count = doc.DocumentNode.SelectSingleNode("//div[@class='pageCount']");

                Owner.Status = "";
                if(count != null) {

                    return int.Parse(count.SelectSingleNode("strong[1]").InnerText.Trim(), System.Globalization.NumberStyles.AllowThousands);
                } else {

                    return 0;
                }
            } catch(RequestFailed) {

                Owner.Status = "";
                return -1;
            }
        }
        public async Task<List<NicoNicoCommunityMember>> GetCommunityFolowerAsync(int page) {

            try {

                Owner.Status = "コミュニティフォロワー取得中";

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(CommunityUrl.Replace("community", "member") + "?page=" + page);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var ret = new List<NicoNicoCommunityMember>();

                var followers = doc.DocumentNode.SelectNodes("//li[@class='memberItem']");

                if(followers != null) {

                    foreach(var follower in followers) {

                        var entry = new NicoNicoCommunityMember();

                        entry.Name = follower.SelectSingleNode("span/a").InnerText.Trim();
                        entry.ThumbNailUrl = follower.SelectSingleNode("div[@class='memberThumbnail']/a/img").Attributes["src"].Value;
                        entry.UserUrl = follower.SelectSingleNode("span/a").Attributes["href"].Value;

                        ret.Add(entry);
                    }
                }

                Owner.Status = "";
                return ret;
            } catch(RequestFailed) {

                Owner.Status = "コミュニティフォロワーの取得に失敗しました";
                return null;
            }
        }
    }

    public class NicoNicoCommunityEntry {

        public string Id { get; set; }

        //コミュニティURL
        public string CommunityUrl { get; set; }

        //コミュニティサムネイルURL
        public string ThumbnailUrl { get; set; }

        //オーナー名
        public string OwnerName { get; set; }

        //オーナーURL
        public string OwnerUrl { get; set; }

        //コミュニティの名前
        public string CommmunityName { get; set; }

        //コミュニティレベル
        public int CommunityLevel { get; set; }

        //コミュニティメンバー数
        public int CommunityMember { get; set; }

        //コミュニティメンバー最大人数
        public int CommunityMaxMember { get; set; }

        //コミュニティプロフィール
        public string CommunityProfile { get; set; }

        //コミュニティ開設日
        public string OpeningDate { get; set; }

        //お知らせ
        public List<NicoNicoCommunityNews> CommunityNews { get; set; }

        //コミュニティ登録タグ
        public List<string> CommunityTags { get; set; }

        public NicoNicoCommunityStatus Status { get; set; }
    }

    public class NicoNicoCommunityNews {

        //お知らせタイトル
        public string Title { get; set; }

        //内容
        public string Description { get; set; }

        //日付
        public string Date { get; set; }
    }

    public enum NicoNicoCommunityStatus {

        Open,
        Close
    }

    public class NicoNicoCommunityVideoEntry : NicoNicoSearchResultEntry {

        //コメント不可とかそういうやつ
        public string VideoStatus { get; set; }
    }

    public class NicoNicoCommunityMember {

        //名前
        public string Name { set; get; }

        public string ThumbNailUrl { get; set; }

        public string UserUrl { get; set; }
    }

}
