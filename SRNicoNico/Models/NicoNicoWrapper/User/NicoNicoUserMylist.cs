using HtmlAgilityPack;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SRNicoNico.Models.NicoNicoWrapper {

    public class NicoNicoUserMylist : NotificationObject {

        private static readonly Regex GlobalHashRegex = new Regex(@"Globals\.hash.+'(.+?)';");

        #region Closed変更通知プロパティ
        private bool _Closed;

        public bool Closed {
            get { return _Closed; }
            set {
                if (_Closed == value)
                    return;
                _Closed = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region MylistList変更通知プロパティ
        private ObservableSynchronizedCollection<NicoNicoUserMylistEntry> _MylistList;

        public ObservableSynchronizedCollection<NicoNicoUserMylistEntry> MylistList {
            get { return _MylistList; }
            set { 
                if (_MylistList == value)
                    return;
                _MylistList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private readonly string UserPageUrl;

        public NicoNicoUserMylist(string url) {

            UserPageUrl = url;
            MylistList = new ObservableSynchronizedCollection<NicoNicoUserMylistEntry>();
        }

        public async Task<string> GetUserMylistAsync() {

            var url = UserPageUrl + "/mylist";

            try {

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var content = doc.DocumentNode.SelectSingleNode("//div[@class='content']");

                var outers = content.SelectNodes("div[@class='articleBody']/div[@class='outer']");

                //終了
                if (outers == null) {

                    Closed = true;
                    return "";
                }
                //ニコレポタイムライン走査
                foreach (var node in outers) {

                    var entry = new NicoNicoUserMylistEntry();

                    //h4タグ
                    var h4 = node.SelectSingleNode("div/h4");

                    entry.ContentUrl = "https://www.nicovideo.jp/" + h4.SelectSingleNode("a").Attributes["href"].Value;

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

                    if (thumb1 != null) {

                        entry.ThumbNail1Available = true;
                        entry.ThumbNail1Url = thumb1.Attributes["src"].Value;
                        entry.ThumbNail1ToolTip = HttpUtility.HtmlDecode(thumb1.Attributes["alt"].Value);
                    } else {
                        goto next;
                    }

                    if (thumb2 != null) {

                        entry.ThumbNail2Available = true;
                        entry.ThumbNail2Url = thumb2.Attributes["src"].Value;
                        entry.ThumbNail2ToolTip = HttpUtility.HtmlDecode(thumb2.Attributes["alt"].Value);
                    } else {
                        goto next;
                    }

                    if (thumb3 != null) {

                        entry.ThumbNail3Available = true;
                        entry.ThumbNail3Url = thumb3.Attributes["src"].Value;
                        entry.ThumbNail3ToolTip = HttpUtility.HtmlDecode(thumb3.Attributes["alt"].Value);
                    }

                    next:
                    MylistList.Add(entry);
                }

                return "";
            } catch (RequestFailed) {

                return "ユーザーマイリストの取得に失敗しました";
            }
        }
    }

    public class NicoNicoUserMylistEntry {

        //マイリストURL
        public string ContentUrl { get; set; }

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
