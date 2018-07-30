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
    public class NicoNicoHistory : NotificationObject {

        //視聴履歴ページ
        private const string HistoryUrl = "http://www.nicovideo.jp/my/history";

        //視聴履歴を返すAPI 全部はとってきてくれないのでボツ
        private const string HistoryApiUrl = "http://www.nicovideo.jp/api/videoviewhistory/list";

        private static readonly string LocalHistoryLocation = NicoNicoUtil.OptionDirectory + "localhistory";

        //視聴時間から時間を抜き出す
        private static readonly Regex TimeRegex = new Regex(@"(\d{4})年(\d+)月(\d+)日.(\d+):(\d+)");


        #region AccountHistories変更通知プロパティ
        private ObservableSynchronizedCollection<NicoNicoHistoryEntry> _AccountHistories;

        public ObservableSynchronizedCollection<NicoNicoHistoryEntry> AccountHistories {
            get { return _AccountHistories; }
            set { 
                if (_AccountHistories == value)
                    return;
                _AccountHistories = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region LocalHistries変更通知プロパティ
        private DispatcherCollection<NicoNicoHistoryEntry> _LocalHistries;

        public DispatcherCollection<NicoNicoHistoryEntry> LocalHistries {
            get { return _LocalHistries; }
            set { 
                if (_LocalHistries == value)
                    return;
                _LocalHistries = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public NicoNicoHistory() {

            AccountHistories = new ObservableSynchronizedCollection<NicoNicoHistoryEntry>();
        }

        public async Task<string> GetAccountHistoryAsync() {

            try {

                AccountHistories.Clear();

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(HistoryUrl);

                var doc = new HtmlDocument();
                doc.LoadHtml(a);

                var list = doc.DocumentNode.SelectNodes("//div[@id='historyList']/div");


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

                        AccountHistories.Add(item);
                    }
                }
                return "";
            } catch(RequestFailed) {

                return "アカウントの視聴履歴の取得に失敗しました";
            }
        }

        public async Task<string> GetLocalHistoryAsync() {

            if(File.Exists(LocalHistoryLocation)) {


                try {

                    using(var stream = new StreamReader(LocalHistoryLocation)) {

                        var coll = new DispatcherCollection<NicoNicoHistoryEntry>(DispatcherHelper.UIDispatcher);

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

                            coll.Add(item);
                        }
                        LocalHistries = coll;
                        return "";
                    }
                } catch(Exception) {
                            
                    return "ローカルの視聴履歴の取得に失敗しました。ファイルが壊れている可能性があります";
                }

            } else {    //ファイルが無い 初回起動か何かかな

                await GetAccountHistoryAsync();

                //ファイルがないときはアカウントの視聴履歴をコピーして使う
                LocalHistries = new DispatcherCollection<NicoNicoHistoryEntry>(DispatcherHelper.UIDispatcher);
                foreach(var entry in AccountHistories) {

                    LocalHistries.Add(entry);
                }
                return "";
            }
        }

        public void SaveLocalHistory() {

            try {

                dynamic root = new DynamicJson();

                var array = new List<object>();

                var list = LocalHistries.ToList();
                list.Sort();
                list.Reverse();

                foreach(var entry in list) {

                    array.Add(new { video_id = entry.VideoId, thumbnail_url = entry.ThumbNailUrl, title = entry.Title, length = entry.Length, watchdate = entry.WatchDate, watchcount = entry.WatchCount });
                }
                root.history = array;

                using(var writer = new StreamWriter(new FileStream(LocalHistoryLocation, FileMode.Create))) {

                    writer.WriteLine(Convert.ToBase64String(Encoding.UTF8.GetBytes(root.ToString())));
                    writer.Flush();
                }
            } catch(Exception) {

            }
        }

        //数が増えると重くなりそうなんだよな・・・ O(n)
        public async Task MergeHistoriesAsync() {

            await Task.Run(() => {

                //ローカル視聴履歴にアカウント視聴履歴を足す
                var from = AccountHistories;
                var to = LocalHistries;

                if (from == null || to == null) {

                    return;
                }
                foreach (var fromentry in from) {

                    var hasVideo = false;
                    foreach (var toentry in to) {

                        //同じ動画
                        if (fromentry.VideoId == toentry.VideoId) {

                            if (fromentry.WatchDate != toentry.WatchDate) {

                                toentry.WatchDate = fromentry.WatchDate;
                                toentry.WatchCount = fromentry.WatchCount;
                                hasVideo = true;
                                break;
                            }
                            hasVideo = true;
                            break;
                        }
                    }
                    if (!hasVideo) {

                        to.Add(fromentry);
                    }
                }
                
                SaveLocalHistory();
            });
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

        public void Open() {

            NicoNicoOpener.Open("http://www.nicovideo.jp/watch/" + VideoId);
        }
    }

}
