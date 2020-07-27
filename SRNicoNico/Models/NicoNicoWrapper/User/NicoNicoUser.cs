using HtmlAgilityPack;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Codeplex.Data;
using System.Net.Http;

namespace SRNicoNico.Models.NicoNicoWrapper {

    public class NicoNicoUser : NotificationObject {

        private static readonly Regex GlobalHashRegex = new Regex(@"Globals\.hash.+'(.+?)';");

        //ユーザーリムーブAPI
        private const string UserUnFollowApi = "https://www.nicovideo.jp/api/watchitem/delete";

        //ユーザーフォローAPI
        private const string UserFollowApi = "https://www.nicovideo.jp/api/watchitem/add";

        #region UserInfo変更通知プロパティ
        private NicoNicoUserEntry _UserInfo;

        public NicoNicoUserEntry UserInfo {
            get { return _UserInfo; }
            set {
                if (_UserInfo == value)
                    return;
                _UserInfo = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private readonly string UserPageUrl;

        public NicoNicoUser(string url) {

            UserPageUrl = url;
        }

        public async Task<string> ToggleFollowOwnerAsync() {

            try {

                if(UserInfo.IsFollow) {

                    var form = new Dictionary<string, string> {
                        ["id_list[1][]"] = UserInfo.UserId,
                        ["token"] = UserInfo.CsrfToken
                    };
                    var request = new HttpRequestMessage(HttpMethod.Post, UserUnFollowApi) {
                        Content = new FormUrlEncodedContent(form)
                    };

                    var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                    var json = DynamicJson.Parse(a);

                    if(json.status == "ok") {

                        UserInfo.IsFollow = false;
                        return "フォローを解除しました";
                    } else {

                        return "フォロー解除に失敗しました";
                    }
                } else {

                    var form = new Dictionary<string, string> {
                        ["item_type"] = "1",
                        ["item_id"] = UserInfo.UserId,
                        ["token"] = UserInfo.CsrfToken
                    };
                    var request = new HttpRequestMessage(HttpMethod.Post, UserFollowApi) {
                        Content = new FormUrlEncodedContent(form)
                    };

                    var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                    var json = DynamicJson.Parse(a);
                    if(json.status == "ok") {

                        UserInfo.IsFollow = true;
                        return "フォローしました";
                    } else {

                        return "フォローに失敗しました";
                    }
                }
            } catch(RequestFailed) {

                return "処理中にエラーが発生しました";
            }
        }
        public async Task<string> GetUserInfoAsync() {

            try {


                //ユーザーページのhtmlを取得
                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(UserPageUrl);

                //htmlをロード
                var doc = new HtmlDocument();
                doc.LoadHtml(a);
                var ret = new NicoNicoUserEntry();

                //ユーザープロファイル
                var div = doc.DocumentNode.SelectSingleNode("//div[@id='js-initial-userpage-data']");
                var data = DynamicJson.Parse(HttpUtility.HtmlDecode(div.Attributes["data-initial-data"].Value));

                var details = data.userDetails.userDetails;
                var user = details.user;

                ret.UserIconUrl = user.icons.large;
                ret.UserName = user.nickname;
                ret.IdAndMemberType = $"{user.id} {user.registeredVersion} {(user.isPremium ? "プレミアム会員" : "一般会員")}";
                ret.Description = user.description;
                ret.FollowedCount = (int) user.followerCount;
                ret.Level = (int) user.userLevel.currentLevel;
                ret.UserId = user.id.ToString();
                ret.UserPageUrl = UserPageUrl;

                ret.IsFollow = details.followStatus.isFollowing;

                //html特殊文字をデコード
                //ret.Description = HttpUtility.HtmlDecode(ret.Description);

                //URLをハイパーリンク化する エンコードされてると正しく動かない
                ret.Description = HyperLinkReplacer.Replace(ret.Description);

                //&だけエンコード エンコードしないとUIに&が表示されない
                ret.Description = ret.Description.Replace("&", "&amp;");

                UserInfo = ret;
                return "";
            } catch(RequestFailed) {

                return "ユーザー情報の取得に失敗しました";
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

        // ユーザーレベル
        public int Level { get; set; }

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
}
