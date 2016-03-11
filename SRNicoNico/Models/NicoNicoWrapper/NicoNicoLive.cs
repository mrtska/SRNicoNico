using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Text.RegularExpressions;

using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using Codeplex.Data;

using Livet;

using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoLive : NotificationObject {

        private const string GetPlayerStatusApiUrl = "http://live.nicovideo.jp/api/getplayerstatus";

        private const string GetThreadsApiUrl = "http://live.nicovideo.jp/api/getthreads";

        private const string HeartBeatApiUrl = "http://live.nicovideo.jp/api/heartbeat";

        private const string WatchingReservationApiUrl = "http://live.nicovideo.jp/api/watchingreservation";

        private const string SetEncodeHtml = @"<head>
                                                                        <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">
                                                                        <style tyle=""text/css"">
                                                                            html { background: #222222; color: white; font-family: Yu Gothic UI, メイリオ, Arial; scrollbar-face-color: #666666; scrollbar-track-color: #3F3F46; scrollbar-arrow-color: #C8C8C8; scrollbar-highlight-color: #3F3F46; scrollbar-3dlight-color: #3F3F46; scrollbar-shadow-color: #3F3F46; }  
                                                                        </style>
                                                                   </head>
                                                                   <body onContextMenu=""return false;"">";
        private string LiveUrl;

        public NicoNicoLive(string url) {

            LiveUrl = url;
        }

        public NicoNicoLiveContent GetPage() {

            try {

                var a = NicoNicoWrapperMain.Session.GetAsync(LiveUrl).Result;

                var doc = new HtmlDocument();
                doc.LoadHtml2(a);

                var content = new NicoNicoLiveContent();

                var hide = doc.DocumentNode.SelectSingleNode("//div[@class='hide']");

                //ゲートじゃない
                if(hide != null) {

                    var json = DynamicJson.Parse(Regex.Match(a, "Nicolive_JS_Conf.Watch = ({.*})").Groups[1].Value);

                    string xml = HttpUtility.UrlDecode(json.GPS.value_by_gps);

                    var xmldoc = new HtmlDocument();
                    xmldoc.LoadHtml2(xml);

                    var stream = xmldoc.DocumentNode.SelectSingleNode("//stream");

                    var status = new NicoNicoGetPlayerStatus();
                    
                    status.Id = stream.SelectSingleNode("id").InnerText;
                    status.Title = json.Player.videoTitle;
                    status.Description = stream.SelectSingleNode("child::description").InnerText;
                    status.Description = SetEncodeHtml + HttpUtility.HtmlDecode(status.Description) + "</body>";
                    status.ProviderType = stream.SelectSingleNode("provider_type").InnerText;
                    status.DefaultCommunity = stream.SelectSingleNode("default_community").InnerText;

                    status.IsOwner = stream.SelectSingleNode("is_owner").InnerText == "1";
                    status.OwnerId = stream.SelectSingleNode("owner_id").InnerText;
                    status.OwnerName = stream.SelectSingleNode("owner_name").InnerText;
                    status.WatchCount = stream.SelectSingleNode("watch_count").InnerText;
                    status.CommentCount = stream.SelectSingleNode("comment_count").InnerText;
                    status.BaseTime = stream.SelectSingleNode("base_time").InnerText;
                    status.StartTime = stream.SelectSingleNode("start_time").InnerText;
                    status.EndTime = stream.SelectSingleNode("end_time").InnerText;

                    status.Archive = stream.SelectSingleNode("archive").InnerText == "1";
                    if(status.Archive) {

                        content.Type = LivePageType.TimeShift;
                        status.QueSheet = new List<QueSheet>();

                        foreach(var que in stream.SelectNodes("quesheet/que")) {

                            var sheet = new QueSheet();

                            sheet.Vpos = que.Attributes["vpos"].Value;
                            sheet.Mail = que.Attributes["mail"].Value;
                            sheet.Content = que.InnerText;

                            status.QueSheet.Add(sheet);
                        }
                    } else {

                        content.Type = LivePageType.Live;
                        status.ContentsList = new List<Contents>();

                        foreach(var que in stream.SelectNodes("contents_list/contents")) {

                            var contents = new Contents();

                            contents.Id = que.Attributes["id"].Value;
                            contents.DisableAudio = que.Attributes["disableAudio"].Value == "1";
                            contents.DisableVideo = que.Attributes["disableVideo"].Value == "1";
                            contents.StartTime = que.Attributes["start_time"].Value;

                            contents.Content = que.InnerText;

                            status.ContentsList.Add(contents);
                        }
                        ;
                    }
                    status.ThumbNailUrl = stream.SelectSingleNode("picture_url").InnerText;

                    status.RoomLabel = xmldoc.DocumentNode.SelectSingleNode("//user/room_label").InnerText;
                    status.SeetNumber = xmldoc.DocumentNode.SelectSingleNode("//user/room_seetno").InnerText;

                    status.RtmpUrl = xmldoc.DocumentNode.SelectSingleNode("//rtmp/url").InnerText;
                    status.Ticket = xmldoc.DocumentNode.SelectSingleNode("//rtmp/ticket").InnerText;

                    status.MesseageServerUrl = xmldoc.DocumentNode.SelectSingleNode("//ms/addr").InnerText;
                    status.MesseageServerPort = xmldoc.DocumentNode.SelectSingleNode("//ms/port").InnerText;
                    status.ThreadId = xmldoc.DocumentNode.SelectSingleNode("//ms/thread").InnerText;


                    content.GetPlayerStatus = status;

                    content.Title = status.Title;
                    content.Description = status.Description;
                    
                    return content;
                }


                var ichiba = doc.DocumentNode.SelectSingleNode("//div[@class='ichiba']").InnerHtml;
                

                //タイムシフト試聴ゲート
                if(ichiba != null) {

                    var lefcom = doc.DocumentNode.SelectSingleNode("//div[@class='lefcom']");

                    if(lefcom != null) {

                        content.Type = LivePageType.TimeShiftGate;
                        content.EndTime = lefcom.InnerHtml.Replace("<br>", "").Trim();
                    } else {
                        
                        content.Type = LivePageType.Gate;
                    }

                    content.Id = GetValue("id", ichiba);
                    content.Title = GetValue("title", ichiba);
                    content.Description = SetEncodeHtml + doc.DocumentNode.SelectSingleNode("//div[@class='text_area']").InnerHtml.Trim() + "</body>";
                    content.Description = content.Description.Replace("target=\"_blank\"", "");

                    content.ThumbNailUrl = doc.DocumentNode.SelectSingleNode("//meta[@itemprop='thumbnail']").Attributes["content"].Value;

                    var hmf = doc.DocumentNode.SelectSingleNode("//div[@class='hmf']");
                    content.StartTime = hmf.SelectSingleNode("child::div[@class='kaijo']").InnerHtml.Trim();

                    content.VisitorsAndComments = hmf.SelectSingleNode("child::div[@id='comment_area" + content.Id + "']").InnerHtml.Trim();
                    content.VisitorsAndComments = content.VisitorsAndComments.Split(new string[] { "<br>" }, StringSplitOptions.None)[1].Trim();
                    return content;
                }
                
                
                ;

                return content;
            } catch(RequestTimeout) {

                return null;
            }
        }


        public void ConfirmReservation(string id, ConfirmReservation conf) {

            if(conf == null) {

                return;
            }
            //lvを消す
            id = id.Replace("lv", "");

            try {

                var request = new GetRequestQuery(WatchingReservationApiUrl);
                request.AddQuery("mode", "confirm_watch_my");
                request.AddQuery("vid", id);

                var a = NicoNicoWrapperMain.Session.GetAsync(request.TargetUrl).Result;

                var doc = new HtmlDocument();
                doc.LoadHtml2(a);

                conf.Id = id;
                conf.Expires = doc.DocumentNode.SelectSingleNode("//div[@class='info']/div[1]/p").InnerText;
                conf.Title = doc.DocumentNode.SelectSingleNode("//div[@class='info']/div/strong").InnerText;

                conf.Token = Regex.Match(a, "confirmToWatch[^,]+,[ ]'(.*)'").Groups[1].Value;
            }catch(RequestTimeout) {

                conf.Expires = null;
                conf.Title = null;
                conf.Token = null;
                return;
            }


        }

        public void MakeReservation(ConfirmReservation conf) {

            try {

                var param = new Dictionary<string, string>();
                param["accept"] = "true";
                param["mode"] = "use";
                param["vid"] = conf.Id;
                param["token"] = conf.Token;

                var request = new HttpRequestMessage(HttpMethod.Post, WatchingReservationApiUrl);

                request.Content = new FormUrlEncodedContent(param);

                var a = NicoNicoWrapperMain.Session.GetAsync(request).Result;

                

            } catch(RequestTimeout) {


            }
        }

        public  void HeartBeat() {


        }

        //正規表現でhtmlからいろいろと抜き出す
        private string GetValue(string field, string input) {

            return Regex.Match(input, field + ":[^']+'([^']+)'").Groups[1].Value;

        }

    }


    public class NicoNicoLiveContent : NotificationObject {

        //そのページのタイプ
        public LivePageType Type { get; set; }

        //生放送ID
        public string Id { get; set; }

        //生放送タイトル
        public string Title { get; set; }

        //生放送説明文
        public string Description { get; set; }

        //サムネイル
        public string ThumbNailUrl { get; set; }

        //開演時間とか開場時間とか日時とかまとめたやつ
        public string StartTime { get; set; }

        //終わった時間 (タイムシフト試聴時のみ)
        public string EndTime { get; set; }

        //来場者数とコメント数
        public string VisitorsAndComments { get; set; }
        
        //その放送が見れるか (タイムシフト予約できるか)
        public bool CanWatch { get; set; }
        
        //登録タグ
        public IList<string> TagEntries { get; set; }

        //ゲートではnull
        public NicoNicoGetPlayerStatus GetPlayerStatus { get; set; }
    }

    public enum LivePageType {

        Live,               //生放送中
        Gate,              //生放送が始まる前のゲート
        TimeShift,       //タイムシフト
        TimeShiftGate //タイムシフトを見る時にでるゲート

    }

    public class ConfirmReservation : NotificationObject {

        //生放送ID lvを除いたもの
        public string Id { get; set; }

        //視聴期限
        public string Expires { get; set; }

        //タイトル
        public string Title { get; set; }

        //タイムシフト予約するのに必要なトークン
        public string Token { get; set; }

        #region IsActive変更通知プロパティ
        private bool _IsActive;

        public bool IsActive {
            get { return _IsActive; }
            set { 
                if(_IsActive == value)
                    return;
                _IsActive = value;
                RaisePropertyChanged();
            }
        }
        #endregion


    }

}
