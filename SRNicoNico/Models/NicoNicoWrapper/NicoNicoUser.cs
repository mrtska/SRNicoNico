using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Livet;
using SRNicoNico.ViewModels;
using HtmlAgilityPack;
using System.Web;
using SRNicoNico.Models.NicoNicoViewer;
using System.Text.RegularExpressions;

namespace SRNicoNico.Models.NicoNicoWrapper {
    using Codeplex.Data;
    using System.Net.Http;
    using NicoNicoUserVideoEntry = NicoNicoSearchResultEntry;

    public class NicoNicoUser : NotificationObject {

        private static readonly Regex GlobalHashRegex = new Regex(@"Globals\.hash.+'(.+?)';");

        //ユーザーリムーブAPI
        private const string UserUnFollowApi = "http://www.nicovideo.jp/api/watchitem/delete";

        //ユーザーフォローAPI
        private const string UserFollowApi = "http://www.nicovideo.jp/api/watchitem/add";

        private NicoNicoNicoRepo NicoRepoInstance;

        private readonly UserViewModel Owner;

        public NicoNicoUser(UserViewModel vm) {

            //NicoRepoInstance = new NicoNicoNicoRepo();
            Owner = vm;
        }


        public async Task<bool> ToggleFollowOwnerAsync(NicoNicoUserEntry entry) {

            try {

                if(entry.IsFollow) {

                    var form = new Dictionary<string, string> {
                        ["id_list[1][]"] = entry.UserId,
                        ["token"] = entry.CsrfToken
                    };
                    var request = new HttpRequestMessage(HttpMethod.Post, UserUnFollowApi) {
                        Content = new FormUrlEncodedContent(form)
                    };

                    var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                    var json = DynamicJson.Parse(a);

                    if(json.status == "ok") {

                        Owner.Status = "フォローを解除しました";
                        return true;
                    } else {

                        Owner.Status = "フォロー解除に失敗しました";
                        return false;
                    }
                } else {

                    var form = new Dictionary<string, string> {
                        ["item_type"] = "1",
                        ["item_id"] = entry.UserId,
                        ["token"] = entry.CsrfToken
                    };
                    var request = new HttpRequestMessage(HttpMethod.Post, UserFollowApi) {
                        Content = new FormUrlEncodedContent(form)
                    };

                    var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                    var json = DynamicJson.Parse(a);
                    if(json.status == "ok") {

                        Owner.Status = "フォローしました";
                        return true;
                    } else {

                        Owner.Status = "フォローに失敗しました";
                        return false;
                    }
                }
            } catch(RequestFailed) {

                return false;
            }
        }
        public async Task<NicoNicoUserEntry> GetUserInfoAsync() {

            try {

                Owner.Status = "ユーザー情報取得中";
                var ret = new NicoNicoUserEntry();

                //ユーザーページのhtmlを取得
                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(Owner.UserPageUrl);

                //htmlをロード
                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                //ユーザープロファイル
                var detail = doc.DocumentNode.SelectSingleNode("//div[@class='userDetail']");
                var profile = detail.SelectSingleNode("div[@class='profile']");
                var account = profile.SelectSingleNode("div[@class='account']");

                ret.UserIconUrl = detail.SelectSingleNode("div[@class='avatar']/img").Attributes["src"].Value;
                ret.UserName = profile.SelectSingleNode("h2").InnerText.Trim();
                ret.IdAndMemberType = account.SelectSingleNode("p[@class='accountNumber']").InnerText.Trim();

                var desc = profile.SelectSingleNode("ul[@class='userDetailComment channel_open_mt0']/li/p/span");

                ret.Description = desc != null ? desc.InnerHtml : "";

                var stats = profile.SelectSingleNode("div[@class='stats channel_open_mb8']/ul");

                ret.FollowedCount = int.Parse(stats.SelectSingleNode("li/span").InnerText, System.Globalization.NumberStyles.AllowThousands);
                ret.StampExp = int.Parse(stats.SelectSingleNode("li/a/span").InnerText, System.Globalization.NumberStyles.AllowThousands);

                ret.CsrfToken = GlobalHashRegex.Match(a).Groups[1].Value;

                ret.UserId = Regex.Match(Owner.UserPageUrl, @"user/(\d+)").Groups[1].Value;
                ret.UserPageUrl = Owner.UserPageUrl;

                var watching = profile.SelectSingleNode("div[@class='watching']");
                ret.IsFollow = watching != null;

                var channel = profile.SelectSingleNode("div[@class='channel_open_box']");
                if(channel != null) {

                    ret.HasChannel = true;
                    ret.ChannelName = channel.SelectSingleNode("p/a").InnerText;
                    ret.ChannelThumbNail = channel.SelectSingleNode("img").Attributes["src"].Value;
                    ret.ChannelUrl = channel.SelectSingleNode("p/a").Attributes["href"].Value;
                }

                //html特殊文字をデコード
                ret.Description = HttpUtility.HtmlDecode(ret.Description);

                //URLをハイパーリンク化する エンコードされてると正しく動かない
                ret.Description = HyperLinkReplacer.Replace(ret.Description);

                //&だけエンコード エンコードしないとUIに&が表示されない
                ret.Description = ret.Description.Replace("&", "&amp;");

                Owner.Status = "";
                return ret;
            } catch(RequestFailed) {

                Owner.Status = "ユーザー情報の取得に失敗しました";
                return null;
            }

        }

        //一度nullを返してきたら二度と呼ばない
        public async Task<Tuple<List<NicoNicoNicoRepoResultEntry>, bool>> GetUserNicoRepoAsync(string userId, string nextPage) {

            var ret = new List<NicoNicoNicoRepoResultEntry>();

            return null;//await NicoRepoInstance.GetNicoRepoAsync();
        }
        public async Task<List<NicoNicoUserMylistEntry>> GetUserMylistAsync() {

            var url = Owner.UserPageUrl + "/mylist";
            Owner.Status = "ユーザーマイリスト取得中";

            var ret = new List<NicoNicoUserMylistEntry>();
            try {

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var content = doc.DocumentNode.SelectSingleNode("//div[@class='content']");

                var outers = content.SelectNodes("div[@class='articleBody']/div[@class='outer']");

                //終了
                if(outers == null) {

                    Owner.Status = "";
                    return null;
                }
                //ニコレポタイムライン走査
                foreach(var node in outers) {

                    var entry = new NicoNicoUserMylistEntry();

                    //h4タグ
                    var h4 = node.SelectSingleNode("div/h4");

                    entry.Url = "http://www.nicovideo.jp/" + h4.SelectSingleNode("a").Attributes["href"].Value;

                    //名前取得
                    entry.Name = HttpUtility.HtmlDecode(h4.SelectSingleNode("a").InnerText.Trim());

                    //説明文取得
                    var desc = node.SelectSingleNode("div/p[@data-nico-mylist-desc-full='true']");
                    entry.Description = desc == null ? "" : desc.InnerText.Trim();

                    entry.Description = HyperLinkReplacer.Replace(entry.Description);

                    //サムネイル取得
                    var thumb1 = node.SelectSingleNode("div/ul/li[1]/img");
                    var thumb2 = node.SelectSingleNode("div/ul/li[2]/img");
                    var thumb3 = node.SelectSingleNode("div/ul/li[3]/img");

                    if(thumb1 != null) {

                        entry.ThumbNail1Available = true;
                        entry.ThumbNail1Url = thumb1.Attributes["src"].Value;
                        entry.ThumbNail1ToolTip = HttpUtility.HtmlDecode(thumb1.Attributes["alt"].Value);
                    } else {
                        goto next;
                    }

                    if(thumb2 != null) {

                        entry.ThumbNail2Available = true;
                        entry.ThumbNail2Url = thumb2.Attributes["src"].Value;
                        entry.ThumbNail2ToolTip = HttpUtility.HtmlDecode(thumb2.Attributes["alt"].Value);
                    } else {
                        goto next;
                    }

                    if(thumb3 != null) {

                        entry.ThumbNail3Available = true;
                        entry.ThumbNail3Url = thumb3.Attributes["src"].Value;
                        entry.ThumbNail3ToolTip = HttpUtility.HtmlDecode(thumb3.Attributes["alt"].Value);
                    }

                    next:
                    ret.Add(entry);
                }

                Owner.Status = "";
                return ret;
            } catch(RequestFailed) {

                Owner.Status = "ユーザーマイリストの取得に失敗しました";
                return null;
            }
        }

        public async Task<int> GetUserVideoCountAsync() {

            try {

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(Owner.UserPageUrl + "/video");
                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var content = doc.DocumentNode.SelectSingleNode("//div[@class='content']");

                var count = content.SelectSingleNode("h3").InnerText;

                var match = Regex.Match(count, @"(\d+)");

                if(match.Success) {

                    return int.Parse(match.Groups[1].Value);
                } else {

                    return 0;
                }
            } catch(RequestFailed) {

                return 0;
            }
        }

        public async Task<List<NicoNicoUserVideoEntry>> GetUserVideoAsync(int page) {

            var url = Owner.UserPageUrl + "/video?page=" + page;
            Owner.Status = "投稿動画を取得中";

            var ret = new List<NicoNicoUserVideoEntry>();

            try {

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var content = doc.DocumentNode.SelectSingleNode("//div[@class='content']");

                var outers = content.SelectNodes("div[@class='articleBody']/div[@class='outer']");

                //終了
                if(outers == null) {

                    Owner.Status = "";
                    return null;
                }

                foreach(var outer in outers) {

                    //フォームだったら飛ばす
                    if(outer.SelectSingleNode("form") != null) {

                        continue;
                    }

                    var entry = new NicoNicoUserVideoEntry();

                    var thumb = outer.SelectSingleNode("div[@class='thumbContainer']");

                    entry.Cmsid = thumb.SelectSingleNode("a").Attributes["href"].Value.Substring(6);
                    entry.ThumbnailUrl = thumb.SelectSingleNode("a/img").Attributes["src"].Value;
                    entry.Length = thumb.SelectSingleNode("span[@class='videoTime']").InnerText.Trim();

                    var section = outer.SelectSingleNode("div[@class='section']");

                    entry.Title = HttpUtility.HtmlDecode(section.SelectSingleNode("h5/a").InnerText.Trim());
                    entry.FirstRetrieve = section.SelectSingleNode("p").InnerText.Trim();

                    var metadata = section.SelectSingleNode("div[@class='dataOuter']/ul");

                    entry.ViewCounter = int.Parse(metadata.SelectSingleNode("li[@class='play']").InnerText.Trim().Split(':')[1].Replace(",", ""));
                    entry.CommentCounter = int.Parse(metadata.SelectSingleNode("li[@class='comment']").InnerText.Trim().Split(':')[1].Replace(",", ""));
                    entry.MylistCounter = int.Parse(metadata.SelectSingleNode("li[@class='mylist']/a").InnerText.Trim().Replace(",", ""));
                    entry.ContentUrl = "http://www.nicovideo.jp/watch/" + entry.Cmsid;

                    NicoNicoUtil.ApplyLocalHistory(entry);
                    ret.Add(entry);
                }

                Owner.Status = "";
                return ret;
            } catch(RequestFailed) {

                Owner.Status = "投稿動画の取得に失敗しました。";
                return null;
            }
        }
    }


    public class NicoNicoUserEntry : NotificationObject {

        //ユーザーID
        public string UserId { get; set; }

        //ユーザーページ
        public string UserPageUrl { get; set; }

        //ユーザーネーム
        public string UserName { get; set; }

        //ユーザー説明文
        public string Description { get; set; }

        //ユーザーアイコン
        public string UserIconUrl { get; set; }

        //IDと会員クラス
        public string IdAndMemberType { get; set; }

        //自分がフォローしているかどうか
        #region IsFollow変更通知プロパティ
        private bool _IsFollow;

        public bool IsFollow {
            get { return _IsFollow; }
            set { 
                if(_IsFollow == value)
                    return;
                _IsFollow = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        //チャンネル持ちユーザーはチャンネルリンクも持っておく
        public bool HasChannel { get; set; }
        public string ChannelThumbNail { get; set; }
        public string ChannelUrl { get; set; }
        public string ChannelName { get; set; }

        //フォロー登録された数
        public int FollowedCount { get; set; }

        //スタンプ経験値
        public int StampExp { get; set; }

        //CSRF防止トークン
        public string CsrfToken { get; set; }
    }

    public class NicoNicoUserMylistEntry {

        //マイリストURL
        public string Url { get; set; }

        //マイリストの名前
        public string Name { get; set; }

        //説明
        public string Description { get; set; }

        //---マイリストサムネイル1---
        public bool ThumbNail1Available { get; set; }
        public string ThumbNail1Url { get; set; }
        public string ThumbNail1ToolTip { get; set; }
        //------

        //---マイリストサムネイル2---
        public bool ThumbNail2Available { get; set; }
        public string ThumbNail2Url { get; set; }
        public string ThumbNail2ToolTip { get; set; }
        //------

        //---マイリストサムネイル3---
        public bool ThumbNail3Available { get; set; }
        public string ThumbNail3Url { get; set; }
        public string ThumbNail3ToolTip { get; set; }
        //------
    }
}
