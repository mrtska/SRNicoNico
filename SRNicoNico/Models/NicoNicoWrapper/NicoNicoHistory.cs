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
using Codeplex.Data;
using System.IO;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoHistory {

        //視聴履歴ページ
        private const string HistoryUrl = "http://www.nicovideo.jp/my/history";

        //視聴履歴を返すAPI 全部はとってきてくれないのでボツ
        private const string HistoryApiUrl = "http://www.nicovideo.jp/api/videoviewhistory/list";

        private readonly string LocalHistoryLocation = NicoNicoUtil.OptionDirectory + "localhistory";

        //視聴時間から時間を抜き出す ひどい実装だなぉぃ
        private readonly Regex TimeRegex = new Regex(@"(\d{4})年(\d+)月(\d+)日.(\d+):(\d+)");

        private HistoryViewModel Owner;

        public NicoNicoHistory(HistoryViewModel owner) {

            Owner = owner;
        }

        public async Task<List<NicoNicoHistoryEntry>> GetAccountHistoryAsync() {

            try {

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(HistoryUrl);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var list = doc.DocumentNode.SelectNodes("//div[@id='historyList']/div");

                var ret = new List<NicoNicoHistoryEntry>();

                if(list != null) {

                    foreach(var entry in list) {

                        var item = new NicoNicoHistoryEntry();

                        item.VideoId = entry.SelectSingleNode("div/h5/a").Attributes["href"].Value.Substring(6);
                        item.ThumbNailUrl = entry.SelectSingleNode("div/a/img").Attributes["data-original"].Value;
                        item.Title = entry.SelectSingleNode("div/h5/a").InnerText.Trim();

                        //削除された動画だとぬるぽになる
                        var time = entry.SelectSingleNode("div/span[@class='videoTime']");

                        if(time != null) {

                            item.Length = time.InnerText.Trim();
                        }

                        var date = TimeRegex.Match(entry.SelectSingleNode("div[@class='section']/p").ChildNodes["#text"].InnerText.Trim());
                        

                        //これはひどい
                        item.WatchDate = UnixTime.ToUnixTime(new DateTime(int.Parse(date.Groups[1].Value), int.Parse(date.Groups[2].Value), int.Parse(date.Groups[3].Value), int.Parse(date.Groups[4].Value), int.Parse(date.Groups[5].Value), 0));
                        item.WatchCount = entry.SelectSingleNode("div[@class='section']/p/span").InnerText;

                        ret.Add(item);
                    }
                }
                Owner.Status = "";
                return ret;
            } catch(RequestFailed) {

                Owner.Status = "アカウントの視聴履歴の取得に失敗しました";
                return null;
            }
        }

        public async Task<List<NicoNicoHistoryEntry>> GetLocalHistoryAsync() {


            if(File.Exists(LocalHistoryLocation)) {

                try {

                    using(var stream = new StreamReader(LocalHistoryLocation)) {

                        var ret = new List<NicoNicoHistoryEntry>();
                        var a = Encoding.UTF8.GetString(Convert.FromBase64String(await stream.ReadToEndAsync()));

                        dynamic json = DynamicJson.Parse(a);
                            
                        foreach(var entry in json.history) {

                            var item = new NicoNicoHistoryEntry();

                            item.VideoId = entry.video_id;
                            item.ThumbNailUrl = entry.thumbnail_url;
                            item.Title = entry.title;
                            item.Length = entry.length;
                            item.WatchDate = (long) entry.watchdate;
                            item.WatchCount = entry.watchcount;

                            ret.Add(item);
                        }

                        return ret;
                    }
                } catch(Exception) {
                            
                    Owner.Status = "ローカルの視聴履歴の取得に失敗しました。ファイルが壊れている可能性があります";
                    return null;
                }

            } else {    //ファイルが無い 初回起動か何かかな

                //ファイルがないときはアカウントの視聴履歴をコピーして使おうかね
                return await GetAccountHistoryAsync(); ;
            }
        }


        public void SaveLocalHistory(List<NicoNicoHistoryEntry> list) {

            try {

                dynamic root = new DynamicJson();

                var array = new List<object>();

                foreach(var entry in list) {

                    array.Add(new { video_id = entry.VideoId, thumbnail_url = entry.ThumbNailUrl, title = entry.Title, length = entry.Length, watchdate = entry.WatchDate, watchcount = entry.WatchCount });
                }

                root.history = array;

                using(var writer = new StreamWriter(new FileStream(LocalHistoryLocation, FileMode.OpenOrCreate))) {

                    writer.BaseStream.Position = 0;
                    writer.WriteLine(Convert.ToBase64String(Encoding.UTF8.GetBytes(root.ToString())));
                    writer.Flush();
                }
            } catch(Exception) {


            }
        }

        //数が増えると重くなりそうなんだよな・・・
        //まあそんなの私が気にすることないか
        public void MergeHistories(List<NicoNicoHistoryEntry> from, List<NicoNicoHistoryEntry> to) {

            if(from == null || to == null) {

                return;
            }


            foreach(var fromentry in from) {


                var hasVideo = false;
                foreach(var toentry in to) {

                    //同じ動画
                    if(fromentry.VideoId == toentry.VideoId) {
                        
                        if(fromentry.WatchDate != toentry.WatchDate) {

                            toentry.WatchDate = fromentry.WatchDate;
                            toentry.WatchCount = fromentry.WatchCount;
                            hasVideo = true;
                            break;
                        }
                        hasVideo = true;
                        break;
                    }
                }

                if(!hasVideo) {

                    to.Add(fromentry);
                }
            }
        }
    }

    public class NicoNicoHistoryEntry : IComparable<NicoNicoHistoryEntry> {

        //動画ID
        public string VideoId { get; set; }

        //動画サムネイル
        public string ThumbNailUrl { get; set; }

        //動画タイトル
        public string Title { get; set; }

        //動画の長さ
        public string Length { get; set; }

        //視聴日時 UnixTime
        public long WatchDate { get; set; }

        //視聴回数
        public string WatchCount { get; set; }

        public int CompareTo(NicoNicoHistoryEntry other) {

            return WatchDate.CompareTo(other.WatchDate);
        }
    }

}
