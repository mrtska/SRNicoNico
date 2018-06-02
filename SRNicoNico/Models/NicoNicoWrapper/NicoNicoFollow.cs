using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using HtmlAgilityPack;
using SRNicoNico.ViewModels;
using System.Text.RegularExpressions;
using System.Web;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoFollow {

        private FollowViewModel Owner;

        public NicoNicoFollow(FollowViewModel owner) {

            Owner = owner;
        }

        //自分がフォローしているユーザーの数を取得する
        public async Task<int> GetFollowedUserCountAsync() {

            try {

                //取得するURL
                var url = "http://www.nicovideo.jp/my/fav/user";

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var count = doc.DocumentNode.SelectSingleNode("//div[@id='favUser']/h3/span");

                if(count == null) {

                    return 0;
                }
                return int.Parse(Regex.Match(count.InnerHtml, @"\((\d+)").Groups[1].Value);
            } catch(RequestFailed) {

                Owner.Status = "フォローしているユーザーの数の取得に失敗しました";
                return -1;
            }
        }

        public async Task<List<NicoNicoFollowUser>> GetFollowedUserAsync(int page) {

            try {

                //取得するURL
                var url = "http://www.nicovideo.jp/my/fav/user?page=" + page;

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var ret = new List<NicoNicoFollowUser>();

                foreach(var outer in doc.DocumentNode.SelectNodes("//div[@class='articleBody']/div[@class='outer']")) {

                    var user = new NicoNicoFollowUser();

                    user.Name = outer.SelectSingleNode("div/h5/a").InnerText.Trim();
                    user.UserPageUrl = "http://www.nicovideo.jp" + outer.SelectSingleNode("div/h5/a").Attributes["href"].Value;

                    user.ThumbNailUrl = outer.SelectSingleNode("div/a/img").Attributes["src"].Value;

                    //説明文がないユーザーはなしにする
                    var p = outer.SelectSingleNode("div/p");
                    user.Description = p == null ? "" : p.InnerText.Trim();

                    ret.Add(user);
                }

                return ret;
            } catch(RequestFailed f) {

                if(f.FailedType == FailedType.Failed) {

                    Owner.Status = "フォローしているユーザーの取得に失敗しました";
                } else {

                    Owner.Status = "フォローしているユーザーの取得がタイムアウトになりました";
                }
                return null;
            }
        }

        //自分がフォローしているマイリストの数を取得する
        public async Task<int> GetFollowedMylistCountAsync() {

            try {

                //取得するURL
                var url = "http://www.nicovideo.jp/my/fav/mylist";

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var count = doc.DocumentNode.SelectSingleNode("//div[@id='favMylist']/h3/span");

                if(count == null) {

                    return 0;
                }
                return int.Parse(Regex.Match(count.InnerHtml, @"\((\d+)").Groups[1].Value);
            } catch(RequestFailed) {

                Owner.Status = "フォローしているmマイリストの数の取得に失敗しました";
                return -1;
            }
        }

        public async Task<List<NicoNicoFollowMylist>> GetFollowedMylistAsync(int page) {

            try {

                //取得するURL
                var url = "http://www.nicovideo.jp/my/fav/mylist?page=" + page;

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var ret = new List<NicoNicoFollowMylist>();

                foreach(var outer in doc.DocumentNode.SelectNodes("//div[@class='articleBody']/div[@class='outer']")) {

                    var mylist = new NicoNicoFollowMylist();

                    mylist.Title = HttpUtility.HtmlDecode(outer.SelectSingleNode("h5/a").InnerText.Trim());
                    mylist.MylistPageUrl = "http://www.nicovideo.jp" + outer.SelectSingleNode("h5/a").Attributes["href"].Value;

                    //URLがJSになってる時は削除されている
                    if(mylist.MylistPageUrl.Contains("javascript:")) {

                        mylist.Deleted = true;
                    }

                    //説明文がないマイリストはなしにする
                    var p = outer.SelectSingleNode("p[@class='mylistDescription']");
                    mylist.Description = p != null ? HttpUtility.HtmlDecode(p.InnerText.Trim()) : "";

                    var inner = outer.SelectSingleNode("div[@class='inner']");
                    if(inner != null) {

                        mylist.HasVideoLink = true;
                        mylist.ThumbNailUrl = inner.SelectSingleNode("div/a/img").Attributes["src"].Value;
                        mylist.VideoTitle = HttpUtility.HtmlDecode(inner.SelectSingleNode("div/p/a").InnerText.Trim());
                        mylist.VideoUrl =  inner.SelectSingleNode("div/p/a").Attributes["href"].Value;
                        mylist.PostedAt = inner.SelectSingleNode("div/p/span").InnerText.Trim();
                    }
                    ret.Add(mylist);
                }

                return ret;
            } catch(RequestFailed f) {

                if(f.FailedType == FailedType.Failed) {

                    Owner.Status = "フォローしているマイリストの取得に失敗しました";
                } else {

                    Owner.Status = "フォローしているマイリストの取得がタイムアウトになりました";
                }
                return null;
            }

        }

        //自分がフォローしているチャンネルの数を取得する
        public async Task<int> GetFollowedChannelCountAsync() {

            try {

                //取得するURL
                var url = "http://www.nicovideo.jp/my/channel";

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var count = doc.DocumentNode.SelectSingleNode("//div[@id='favChannel']/h3/span");

                if(count == null) {

                    return 0;
                }

                return int.Parse(Regex.Match(count.InnerHtml, @"\((\d+)").Groups[1].Value);
            } catch(RequestFailed) {

                Owner.Status = "フォローしているチャンネルの数の取得に失敗しました";
                return -1;
            }

        }


        public async Task<List<NicoNicoFollowChannel>> GetFollowedChannelAsync(int page) {

            try {

                //取得するURL
                var url = "http://www.nicovideo.jp/my/channel?page=" + page;

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var ret = new List<NicoNicoFollowChannel>();

                foreach(var outer in doc.DocumentNode.SelectNodes("//div[@class='articleBody']/div[@class='outer']")) {

                    var channel = new NicoNicoFollowChannel();

                    channel.Title = HttpUtility.HtmlDecode(outer.SelectSingleNode("div/h5/a").InnerText.Trim());
                    channel.ChannelUrl = outer.SelectSingleNode("div/h5/a").Attributes["href"].Value;

                    channel.ThumbNailUrl = outer.SelectSingleNode("div/a/img").Attributes["src"].Value;

                    //説明文がないチャンネルはなしにする
                    var p = outer.SelectSingleNode("div/p");
                    channel.Description = p.Attributes["class"] == null ? p.InnerText.Trim() : "";

                    ret.Add(channel);
                }

                return ret;
            } catch(RequestFailed f) {

                if(f.FailedType == FailedType.Failed) {

                    Owner.Status = "フォローしているチャンネルの取得に失敗しました";
                } else {

                    Owner.Status = "フォローしているチャンネルの取得がタイムアウトになりました";
                }
                return null;
            }

        }


        //自分がフォローしているコミュニティの数を取得する
        public async Task<int> GetFollowedCommunityCountAsync() {

            try {

                //取得するURL
                var url = "http://www.nicovideo.jp/my/community";

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var count = doc.DocumentNode.SelectSingleNode("//div[@id='favCommunity']/h3/span");

                if(count == null) {

                    return 0;
                }
                return int.Parse(Regex.Match(count.InnerHtml, @"\((\d+)").Groups[1].Value);
            } catch(RequestFailed) {

                Owner.Status = "フォローしているコミュニティの数の取得に失敗しました";
                return -1;
            }

        }


        public async Task<List<NicoNicoFollowCommunity>> GetFollowedCommunityAsync(int page) {

            try {

                //取得するURL
                var url = "http://www.nicovideo.jp/my/community?page=" + page;

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var ret = new List<NicoNicoFollowCommunity>();

                foreach(var outer in doc.DocumentNode.SelectNodes("//div[@class='articleBody']/div[@class='outer']")) {

                    var channel = new NicoNicoFollowCommunity();

                    channel.Title = HttpUtility.HtmlDecode(outer.SelectSingleNode("div/h5/a").InnerText.Trim());
                    channel.CommunityUrl = outer.SelectSingleNode("div/h5/a").Attributes["href"].Value;

                    channel.ThumbNailUrl = outer.SelectSingleNode("div/a/img").Attributes["src"].Value;

                    var data = outer.SelectSingleNode("div/ul");

                    channel.Data = data.SelectSingleNode("li[1]").InnerText.Trim()+ " " + data.SelectSingleNode("li[2]").InnerText.Trim();

                    //説明文がないチャンネルはなしにする
                    var p = outer.SelectSingleNode("div/p");
                    channel.Description = p.Attributes["class"] == null ? HttpUtility.HtmlDecode(p.InnerText.Trim()) : "";

                    ret.Add(channel);
                }

                return ret;
            } catch(RequestFailed f) {

                if(f.FailedType == FailedType.Failed) {

                    Owner.Status = "フォローしているコミュニティの取得に失敗しました";
                } else {

                    Owner.Status = "フォローしているコミュニティの取得がタイムアウトになりました";
                }
                return null;
            }

        }



    }

    //フォローしているユーザー
    public class NicoNicoFollowUser {

        //ユーザーページURL
        public string UserPageUrl { get; set; }

        //お気に入りユーザーの名前
        public string Name { get; set; }

        //簡易説明文
        public string Description { get; set; }

        //サムネイルURL
        public string ThumbNailUrl { get; set; }
    }

    //フォローしているマイリスト
    public class NicoNicoFollowMylist {

        //マイリストタイトル
        public string Title { get; set; }

        //マイリストページURL
        public string MylistPageUrl { get; set; }

        //簡易説明文
        public string Description { get; set; }

        //代表動画があるかどうか
        public bool HasVideoLink { get; set; }

        //代表動画のサムネ
        public string ThumbNailUrl { get; set; }

        //代表動画URL
        public string VideoUrl { get; set; }

        //代表動画タイトル
        public string VideoTitle { get; set; }

        //代表動画投稿日時
        public string PostedAt { get; set; }

        //マイリストが削除されているか
        public bool Deleted { get; set; }

    }

    //フォローしているチャンネル
    public class NicoNicoFollowChannel {

        //チャンネル名
        public string Title { get; set; }

        //チャンネルURL
        public string ChannelUrl { get; set; }

        //チャンネルサムネ
        public string ThumbNailUrl { get; set; }

        //動画：\d+
        public string Description { get; set; }
    }

    //フォローしているコミュニティ
    public class NicoNicoFollowCommunity {

        //コミュニティ名
        public string Title { get; set; }

        //コミュニティURL
        public string CommunityUrl { get; set; }

        //コミュニティサムネ
        public string ThumbNailUrl { get; set; }

        //コミュニティデータ
        public string Data { get; set; }

        public string Description { get; set; }
    }

}
