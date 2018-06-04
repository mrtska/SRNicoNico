using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using HtmlAgilityPack;
using SRNicoNico.ViewModels;
using Codeplex.Data;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;
using System.Text.RegularExpressions;
using System.Collections;

namespace SRNicoNico.Models.NicoNicoWrapper {


    public class NicoNicoComment {

        private const long IdLimit = 10000000000000L;


        private readonly Dictionary<char, char> ConvMap = new Dictionary<char, char>() {
    {'１','1'},{'２','2'},{'３','3'},{'４','4'},{'５','5'},
    {'６','6'},{'７','7'},{'８','8'},{'９','9'},{'０','0'},
    {'Ａ','A'},{'Ｂ','B'},{'Ｃ','C'},{'Ｄ','D'},{'Ｅ','E'},
    {'Ｆ','F'},{'Ｇ','G'},{'Ｈ','H'},{'Ｉ','I'},{'Ｊ','J'},
    {'Ｋ','K'},{'Ｌ','L'},{'Ｍ','M'},{'Ｎ','N'},{'Ｏ','O'},
    {'Ｐ','P'},{'Ｑ','Q'},{'Ｒ','R'},{'Ｓ','S'},{'Ｔ','T'},
    {'Ｕ','U'},{'Ｖ','V'},{'Ｗ','W'},{'Ｘ','X'},{'Ｙ','Y'},
    {'Ｚ','Z'},
    {'ａ','a'},{'ｂ','b'},{'ｃ','c'},{'ｄ','d'},{'ｅ','e'},
    {'ｆ','f'},{'ｇ','g'},{'ｈ','h'},{'ｉ','i'},{'ｊ','j'},
    {'ｋ','k'},{'ｌ','l'},{'ｍ','m'},{'ｎ','n'},{'ｏ','o'},
    {'ｐ','p'},{'ｑ','q'},{'ｒ','r'},{'ｓ','s'},{'ｔ','t'},
    {'ｕ','u'},{'ｖ','v'},{'ｗ','w'},{'ｘ','x'},{'ｙ','y'},
    {'ｚ','z'},
    {'　',' '}, { '＠', '@' }
        };


        //ポストキー取得API
        private const string ThreadKeyApiUrl = "http://flapi.nicovideo.jp/api/getthreadkey";

        //ポストキー取得API
        private const string GetPostKeyApiUrl = "http://flapi.nicovideo.jp/api/getpostkey";

        private VideoViewModel Owner;
        
        private long RequestId = 0;
        private long PacketId = 0;
        private long PacketCount = 0;

        public NicoNicoComment(VideoViewModel vm) {

            Owner = vm;
        }


        public async Task<List<NicoNicoCommentList>> GetAllCommentAsync() {

            var normalList = new NicoNicoCommentList("通常コメント");
            var uploaderList = new NicoNicoCommentList("投稿者コメント");
            var communityList = new NicoNicoCommentList("コミュニティコメント");
            var channelList = new NicoNicoCommentList("チャンネルコメント");
            var officialList = new NicoNicoCommentList("公式コメント");

            try {

                var ret = new List<NicoNicoCommentList>();

                //通常コメントを取得
                //ret.Add(new NicoNicoCommentList() { ListName = "通常コメント", CommentList = await GetNormalCommentAsync() });

                //パケットIDとコメントタイプを紐付ける
                var dic = new Dictionary<long, CommentType>();

                var jsonarray = new List<object>();
                {
                    PacketId = PacketCount;
                    //rs
                    jsonarray.Add(new { ping = new { content = "rs:" + RequestId % IdLimit } });

                    //ps 通常コメント
                    jsonarray.Add(new { ping = new { content = "ps:" + PacketId % IdLimit } });

                    var normalThread = new {

                        thread = Owner.ApiData.Thread.ThreadIds.Default,
                        version = "20090904",
                        language = 0,
                        user_id = int.Parse(Owner.ApiData.Viewer.Id),
                        with_global = 1,
                        scores = 1,
                        nicoru = 0,
                        userkey = Owner.ApiData.Context.UserKey
                    };

                    dic[PacketId] = CommentType.Normal;
                    jsonarray.Add(new { thread = normalThread });

                    //pf
                    jsonarray.Add(new { ping = new { content = "pf:" + PacketId++ % IdLimit } });
                    //------
                    //ps 通常コメント leaves
                    jsonarray.Add(new { ping = new { content = "ps:" + PacketId % IdLimit } });

                    var normalLeaves = new {

                        thread = Owner.ApiData.Thread.ThreadIds.Default,
                        language = 0,
                        user_id = int.Parse(Owner.ApiData.Viewer.Id),
                        content = "0-" + ((Owner.ApiData.Video.Duration / 60) + 1) + ":100," + GetReadCommentCount(Owner.ApiData.Video.Duration).ToString(),
                        scores = 1,
                        nicoru = 0,
                        userkey = Owner.ApiData.Context.UserKey
                    };

                    dic[PacketId] = CommentType.NormalLeaves;
                    jsonarray.Add(new { thread_leaves = normalLeaves });
                    //pf leaves
                    jsonarray.Add(new { ping = new { content = "pf:" + PacketId++ % IdLimit } });
                    //------
                    
                    //投稿者コメント有りフラグ
                    if(Owner.ApiData.Thread.HasOwnerThread == "1") {

                        //ps 投稿者コメント
                        jsonarray.Add(new { ping = new { content = "ps:" + PacketId % IdLimit } });

                        var forkThread = new {

                            thread = Owner.ApiData.Thread.ThreadIds.Default,
                            version = "20061206",
                            fork = 1,
                            user_id = int.Parse(Owner.ApiData.Viewer.Id),
                            res_from = -1000,
                            with_global = 1,
                            scores = 1,
                            nicoru = 0,
                            userkey = Owner.ApiData.Context.UserKey
                        };

                        dic[PacketId] = CommentType.Uploader;
                        jsonarray.Add(new { thread = forkThread });
                        //pf fork
                        jsonarray.Add(new { ping = new { content = "pf:" + PacketId++ % IdLimit } });
                    }

                    if(Owner.ApiData.IsChannelVideo) {

                        //ps チャンネルコメント
                        jsonarray.Add(new { ping = new { content = "ps:" + PacketId % IdLimit } });

                        var query = new GetRequestQuery(ThreadKeyApiUrl);
                        query.AddQuery("thread", Owner.ApiData.Thread.ThreadIds.Community);

                        var threadkey = await App.ViewModelRoot.CurrentUser.Session.GetAsync(query.TargetUrl);
                        var threadKeyDic = HttpUtility.ParseQueryString(threadkey);

                        channelList.ThreadKey = threadKeyDic["threadkey"];
                        channelList.Force184 = threadKeyDic["force_184"];

                        var channelThread = new {

                            thread = Owner.ApiData.Thread.ThreadIds.Community,
                            version = "20090904",
                            language = 0,
                            user_id = int.Parse(Owner.ApiData.Viewer.Id),
                            force_184 = channelList.Force184,
                            with_global = 1,
                            scores = 1,
                            nicoru = 0,
                            threadkey = channelList.ThreadKey
                        };

                        dic[PacketId] = CommentType.Channel;
                        jsonarray.Add(new { thread = channelThread });
                        
                        //pf channel
                        jsonarray.Add(new { ping = new { content = "pf:" + PacketId++ % IdLimit } });

                        jsonarray.Add(new { ping = new { content = "ps:" + PacketId % IdLimit } });

                        var channelLeaves = new {

                            thread = Owner.ApiData.Thread.ThreadIds.Community,
                            language = 0,
                            user_id = int.Parse(Owner.ApiData.Viewer.Id),
                            content = "0-" + ((Owner.ApiData.Video.Duration / 60) + 1) + ":100," + GetReadCommentCount(Owner.ApiData.Video.Duration).ToString(),
                            scores = 1,
                            nicoru = 0,
                            force_184 = threadKeyDic["force_184"],
                            threadkey = threadKeyDic["threadkey"]
                        };

                        dic[PacketId] = CommentType.ChannelLeaves;
                        jsonarray.Add(new { thread_leaves = channelLeaves });


                        //pf channel
                        jsonarray.Add(new { ping = new { content = "pf:" + PacketId++ % IdLimit } });

                    }


                    //rf
                    jsonarray.Add(new { ping = new { content = "rf:" + RequestId++ % IdLimit } });

                    PacketCount += jsonarray.Count;
                }


                var request = new HttpRequestMessage(HttpMethod.Post, Owner.ApiData.Thread.ServerUrl)
                {
                    Content = new StringContent(DynamicJson.Serialize(jsonarray), Encoding.UTF8, "text/plain")
                };
                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                //結果
                {


                    var currentType = CommentType.Normal;
                    foreach(var entry in DynamicJson.Parse(a)) {

                        //コメントタイプを確認
                        if(entry.ping()) {

                            var content = entry.ping.content as string;
                            if(content.Contains("ps")) {

                                currentType = dic[long.Parse(content.Split(':')[1])];
                            }
                            continue;
                        }
                        if(entry.thread()) {

                            var thread = entry.thread;


                            switch(currentType) {
                                case CommentType.NormalLeaves: {

                                        normalList.ThreadId = thread.thread;
                                        normalList.Ticket = thread.ticket() ? thread.ticket : "";
                                        normalList.LastRes = thread.last_res() ? (int) thread.last_res : 0;
                                        break;
                                    }
                                case CommentType.Uploader: {

                                        break;
                                    }
                                case CommentType.Channel: {

                                        break;
                                    }
                                case CommentType.ChannelLeaves: {

                                        channelList.ThreadId = thread.thread;
                                        channelList.Ticket = thread.ticket() ? thread.ticket : "";
                                        channelList.LastRes = thread.last_res() ? (int)thread.last_res : 0;
                                        break;
                                    }
                            }
                            continue;
                        }

                        if(entry.chat()) {

                            var chat = entry.chat;

                            switch(currentType) {
                                case CommentType.NormalLeaves: {

                                        if(chat.deleted()) {

                                            continue;
                                        }

                                        var item = new NicoNicoCommentEntry() {
                                            Number = (int)chat.no,
                                            Vpos = (int)chat.vpos,
                                            PostedAt = (long)chat.date,
                                            Anonymity = chat.anonymity(),
                                            UserId = chat.user_id,
                                            Mail = chat.mail() ? chat.mail : "",
                                            Content = chat.content,
                                            Score = chat.score() ? (int)chat.score : 0
                                        };
                                        item.DisassembleMail();
                                        normalList.Add(item);
                                        break;
                                    }
                                case CommentType.Uploader: {

                                        if(chat.deleted()) {

                                            continue;
                                        }

                                        var item = new NicoNicoCommentEntry() {
                                            Number = (int)chat.no,
                                            Vpos = (int)chat.vpos,
                                            PostedAt = (long)chat.date,
                                            Content = chat.content() ? chat.content: "",
                                            Mail = chat.mail() ? chat.mail : ""
                                        };
                                        item.Mail = new string(item.Mail.Select(n => (ConvMap.ContainsKey(n) ? ConvMap[n] : n)).ToArray());
                                        item.IsUploader = true;

                                        item.DisassembleMail();
                                        uploaderList.Add(item);
                                        break;
                                    }
                                case CommentType.Channel: {

                                        break;
                                    }
                                case CommentType.ChannelLeaves: {

                                        if(chat.deleted()) {

                                            continue;
                                        }

                                        var item = new NicoNicoCommentEntry() {
                                            Number = (int)chat.no,
                                            Vpos = (int)chat.vpos,
                                            PostedAt = (long)chat.date,
                                            Anonymity = chat.anonymity(),
                                            UserId = chat.user_id,
                                            Mail = chat.mail() ? chat.mail : "",
                                            Content = chat.content,
                                            Score = chat.score() ? (int)chat.score : 0
                                        };

                                        item.DisassembleMail();
                                        channelList.Add(item);
                                        
                                        break;
                                    }
                            }
                        }
                    }

                    if(!normalList.IsEmpty()) {

                        normalList.Sort();
                        ret.Add(normalList);
                    }
                    if(!communityList.IsEmpty()) {

                        communityList.Sort();
                        ret.Add(communityList);
                    }
                    if(!channelList.IsEmpty()) {

                        channelList.Sort();
                        ret.Add(channelList);
                    }
                    if(!officialList.IsEmpty()) {

                        officialList.Sort();
                        ret.Add(officialList);
                    }
                    if(!uploaderList.IsEmpty()) {

                        uploaderList.Sort();
                        ret.Add(uploaderList);
                    }
                }
                return ret;
            } catch(RequestFailed) {

                Owner.Status = "コメントの取得に失敗しました";
                return null;
            }
        }
        public async Task<List<NicoNicoCommentList>> RefreshCommentAsync(List<NicoNicoCommentList> targetList) {

            var jsonarray = new List<object>();

            PacketId = PacketCount;
            //rs
            jsonarray.Add(new { ping = new { content = "rs:" + RequestId % IdLimit } });

            foreach (var target in targetList) {

                if(target.ListName == "通常コメント") {

                    //ps
                    jsonarray.Add(new { ping = new { content = "ps:" + PacketId % IdLimit } });

                    var normalThread = new {

                        thread = Owner.ApiData.Thread.ThreadIds.Default,
                        version = "20061206",
                        language = 0,
                        user_id = int.Parse(Owner.ApiData.Viewer.Id),
                        res_from = target.LastRes + 1,
                        with_global = 1,
                        scores = 1,
                        nicoru = 0,
                        userkey = Owner.ApiData.Context.UserKey
                    };
                    jsonarray.Add(new { thread = normalThread });

                    target.PacketId = PacketId;
                    //pf
                    jsonarray.Add(new { ping = new { content = "pf:" + PacketId++ % IdLimit } });
                } else if(target.ListName == "チャンネルコメント") {

                    //ps
                    jsonarray.Add(new { ping = new { content = "ps:" + PacketId % IdLimit } });

                    var channelThread = new {

                        thread = Owner.ApiData.Thread.ThreadIds.Community,
                        version = "20061206",
                        language = 0,
                        user_id = int.Parse(Owner.ApiData.Viewer.Id),
                        res_from = target.LastRes + 1,
                        force_184 = target.Force184,
                        with_global = 1,
                        scores = 0,
                        nicoru = 0,
                        threadkey = target.ThreadKey
                    };

                    jsonarray.Add(new { thread = channelThread });

                    target.PacketId = PacketId;
                    //pf
                    jsonarray.Add(new { ping = new { content = "pf:" + PacketId++ % IdLimit } });
                }
            }

            //rf
            jsonarray.Add(new { ping = new { content = "rf:" + RequestId++ % IdLimit } });

            var request = new HttpRequestMessage(HttpMethod.Post, Owner.ApiData.Thread.ServerUrl) {
                Content = new StringContent(DynamicJson.Serialize(jsonarray), Encoding.UTF8, "text/plain")
            };
            var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

            long currentPacketId = 0;
            foreach (var entry in DynamicJson.Parse(a)) {

                //コメントタイプを確認
                if (entry.ping()) {

                    var content = entry.ping.content as string;
                    if (content.Contains("ps")) {

                        currentPacketId = long.Parse(content.Split(':')[1]);
                    }
                    continue;
                }

                foreach (var target in targetList) {

                    if(target.PacketId == currentPacketId) {

                        if (entry.thread()) {

                            var thread = entry.thread;

                            target.ThreadId = thread.thread;
                            target.Ticket = thread.ticket() ? thread.ticket : "";
                            target.LastRes = thread.last_res() ? (int)thread.last_res : 0;
                            continue;
                        }

                        if (entry.chat()) {

                            var chat = entry.chat;
                            if(target.ListName == "通常コメント") {

                                if (chat.deleted()) {

                                    continue;
                                }
                                var item = new NicoNicoCommentEntry() {
                                    Number = (int)chat.no,
                                    Vpos = (int)chat.vpos,
                                    PostedAt = (long)chat.date,
                                    Anonymity = chat.anonymity(),
                                    UserId = chat.user_id,
                                    Mail = chat.mail() ? chat.mail : "",
                                    Content = chat.content,
                                    Score = chat.score() ? (int)chat.score : 0
                                };

                                item.DisassembleMail();
                                target.Add(item);
                                break;
                            }
                            if(target.ListName == "投稿者コメント") {

                                if (chat.deleted()) {

                                    continue;
                                }

                                var item = new NicoNicoCommentEntry() {
                                    Number = (int)chat.no,
                                    Vpos = (int)chat.vpos,
                                    PostedAt = (long)chat.date,
                                    Content = chat.content,
                                    Mail = chat.mail() ? chat.mail : ""
                                };
                                item.Mail = new string(item.Mail.Select(n => (ConvMap.ContainsKey(n) ? ConvMap[n] : n)).ToArray());
                                item.IsUploader = true;

                                item.DisassembleMail();
                                target.Add(item);
                            }

                            if(target.ListName == "チャンネルコメント") {

                                if (chat.deleted()) {

                                    continue;
                                }

                                var item = new NicoNicoCommentEntry() {
                                    Number = (int)chat.no,
                                    Vpos = (int)chat.vpos,
                                    PostedAt = (long)chat.date,
                                    Anonymity = chat.anonymity(),
                                    UserId = chat.user_id,
                                    Mail = chat.mail() ? chat.mail : "",
                                    Content = chat.content,
                                    Score = chat.score() ? (int)chat.score : 0
                                };

                                item.DisassembleMail();
                                target.Add(item);
                            }
                        }
                    }
                }
            }
            return targetList;
        }

        //動画の長さによって取得するコメントの量を変える
        public int GetReadCommentCount(int length) {

            if(length < 60) {

                return 100;
            }
            if(length < 300) {

                return 250;
            }
            if(length < 600) {

                return 500;
            }
            return 1000;
        }

        public async Task<int> PostAsync(NicoNicoCommentList target, int vpos, string mail, string text) {

            try {

                var query = new GetRequestQuery(GetPostKeyApiUrl);
                query.AddQuery("thread", target.ThreadId);
                query.AddQuery("block_no", Math.Floor((decimal)(target.LastRes + 1) / 100).ToString());
                query.AddQuery("device", "1");
                query.AddQuery("version", "1");
                query.AddQuery("version_sub", "6");
                var postkey = await App.ViewModelRoot.CurrentUser.Session.GetAsync(query.TargetUrl);


                PacketId = PacketCount;

                var jsonarray = new List<object>();

                //rs
                jsonarray.Add(new { ping = new { content = "rs:" + RequestId % IdLimit } });
                //ps
                jsonarray.Add(new { ping = new { content = "ps:" + PacketId % IdLimit } });

                //コメント投稿
                var chat = new {

                    thread = target.ThreadId,
                    vpos = vpos,
                    mail = mail,
                    ticket = target.Ticket,
                    user_id = int.Parse(Owner.ApiData.Viewer.Id),
                    premium = Owner.ApiData.Viewer.IsPremium ? 1 : 0,
                    postkey = postkey,
                    content = text
                };

                jsonarray.Add(new { chat = chat });

                //pf
                jsonarray.Add(new { ping = new { content = "pf:" + PacketId++ % IdLimit } });
                //rf
                jsonarray.Add(new { ping = new { content = "rf:" + RequestId++ % IdLimit } });


                var request = new HttpRequestMessage(HttpMethod.Post, Owner.ApiData.Thread.ServerUrl) {
                    Content = new StringContent(DynamicJson.Serialize(jsonarray), Encoding.UTF8, "text/plain")
                };

                PacketCount += jsonarray.Count;

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                //成功したか確認
                foreach(var entry in DynamicJson.Parse(a)) {

                    if(entry.chat_result()) {

                        if(entry.chat_result.status == 0D) {

                            Owner.Status = "コメントを投稿しました";
                            return (int)entry.chat_result.no;
                        } else {

                            Owner.Status = "コメントの投稿に失敗しました";
                            return -1;
                        }
                    }
                }
                return -1;
            } catch(RequestFailed) {

                Owner.Status = "コメントの投稿に失敗しました";
                return -1;
            }
        }
    }

    public class NicoNicoCommentList {

        //コメントリストの名前 通常コメントとかチャンネルコメントとか投稿者コメントとかいろいろ
        public string ListName { get; private set; }

        //コメントリスト
        public List<NicoNicoCommentEntry> CommentList { get; set; }

        public string ThreadId { get; set; }


        internal long PacketId;

        //コメントチケット？
        public string Ticket { get; set; }

        //最後のコメント
        public int LastRes { get; set; }

        //ThreadKey ある場合
        public string ThreadKey { get; set; }

        //ある場合 0か1
        public string Force184 { get; set; }


        public NicoNicoCommentList(string name) {

            ListName = name;
            CommentList = new List<NicoNicoCommentEntry>();
        }

        public void Add(NicoNicoCommentEntry entry) {

            CommentList.Add(entry);
        }

        public void Sort() {

            CommentList.Sort();
        }

        public bool IsEmpty() {

            return CommentList.Count == 0;
        }
    }

    //コメント情報
    public class NicoNicoCommentEntry : IComparable<NicoNicoCommentEntry> {


        public static readonly Dictionary<string, string> NicoNicoOfficialCommentColor = new Dictionary<string, string>();


        public static readonly Regex TripletColor = new Regex(@"#[\d|A-F]{6}");
        public static readonly Regex DurationRegex = new Regex(@"@(\d+)");

        static NicoNicoCommentEntry() {

            //---全会員共通---
            NicoNicoOfficialCommentColor["white"] = "#FFFFFF";
            NicoNicoOfficialCommentColor["red"] = "#FF0000";
            NicoNicoOfficialCommentColor["pink"] = "#FF8080";
            NicoNicoOfficialCommentColor["orange"] = "#FFC000";
            NicoNicoOfficialCommentColor["yellow"] = "#FFFF00";
            NicoNicoOfficialCommentColor["green"] = "#00FF00";
            NicoNicoOfficialCommentColor["cyan"] = "#00FFFF";
            NicoNicoOfficialCommentColor["blue"] = "#0000FF";
            NicoNicoOfficialCommentColor["purple"] = "#C000FF";
            NicoNicoOfficialCommentColor["black"] = "#000000";
            //------

            //---プレミアム会員のみ---
            NicoNicoOfficialCommentColor["white2"] = "#CCCC99";
            NicoNicoOfficialCommentColor["niconicowhite"] = NicoNicoOfficialCommentColor["white2"];

            NicoNicoOfficialCommentColor["red2"] = "#CC0033";
            NicoNicoOfficialCommentColor["truered"] = NicoNicoOfficialCommentColor["red2"];

            NicoNicoOfficialCommentColor["pink2"] = "#FF33CC";

            NicoNicoOfficialCommentColor["orange2"] = "#FF6600";
            NicoNicoOfficialCommentColor["passionorange"] = NicoNicoOfficialCommentColor["orange2"];

            NicoNicoOfficialCommentColor["yellow2"] = "#999900";
            NicoNicoOfficialCommentColor["madyellow"] = NicoNicoOfficialCommentColor["yellow2"];

            NicoNicoOfficialCommentColor["green2"] = "#00CC66";
            NicoNicoOfficialCommentColor["elementalgreen"] = NicoNicoOfficialCommentColor["green2"];

            NicoNicoOfficialCommentColor["cyan2"] = "#00CCCC";

            NicoNicoOfficialCommentColor["blue2"] = "#3399FF";
            NicoNicoOfficialCommentColor["marineblue"] = NicoNicoOfficialCommentColor["blue2"];

            NicoNicoOfficialCommentColor["purple2"] = "#6633CC";
            NicoNicoOfficialCommentColor["nobleviolet"] = NicoNicoOfficialCommentColor["purple2"];

            NicoNicoOfficialCommentColor["black2"] = "#666666";
        }

        //コメント番号
        public int Number { get; set; }

        //コメント再生位置 デシ秒
        public int Vpos { get; set; }

        //コメント投稿時間 Unixタイム
        public long PostedAt { get; set; }

        //削除されたか
        public bool Deleted { get; set; }

        //匿名コメントかどうか
        public bool Anonymity { get; set; }

        //コメント
        public string Content { get; set; }

        //Mail
        public string Mail { get; set; } = "";

        //ユーザーID
        public string UserId { get; set; }

        //NGスコア
        public int Score { get; set; }

        //NGスコアが閾値を超過してたら
        public bool Rejected { get; set; }

        //コメント位置
        public string Position { get; set; }

        //コメント拡大率
        public double Scale { get; set; }

        //コメント描画終了位置
        public int Vend { get; set; }

        //コメントカラー
        public string CommentColor { get; set; }

        //コメントサイズ
        public string CommentSize { get; set; }

        //透明度
        public double Opacity { get; set; }

        //コメントフォント
        public string Font { get; set; }

        //逆方向にコメントを流すか
        public bool Reverse { get; set; }

        public bool Full { get; set; }

        //投稿者コメントか
        public bool IsUploader { get; set; }

        //ニコスクリプトの場合
        public NicoScriptBase NicoScript { get; set; }

        //描画しているかどうか
        public bool IsRendering { get; set; }

        //投稿した直後か
        public bool JustPosted { get; set; }

        //コメント表示時間 4秒か3秒なんだけど投コメとかで秒数指定してるやつもあるし
        public int Duration { get; set; }

        public string DefaultCommentSize { get; set; }

        int IComparable<NicoNicoCommentEntry>.CompareTo(NicoNicoCommentEntry obj) {

            if(Vpos == obj.Vpos) {

                return Number.CompareTo(obj.Number);
            }

            return Vpos.CompareTo(obj.Vpos);
        }

        public string ToJson() {

            return DynamicJson.Serialize(this);
        }

        //Mailをバラして各パラメータに入れる
        internal void DisassembleMail() {


            if(Mail.Contains("ue")) {

                Position = "ue";
                Duration = 3000;
            } else if(Mail.Contains("shita")) {

                Position = "shita";
                Duration = 3000;
            } else {

                Position = "naka";
                Duration = 4000;
            }

            Vend = Vpos + (Duration / 10);

            CommentColor = "#FFFFFF";

            //色を反映させる
            foreach(var key in NicoNicoOfficialCommentColor.Keys) {

                if(Mail.Contains(key)) {

                    CommentColor = NicoNicoOfficialCommentColor[key];
                    break;
                }
            }

            //#xxxxxxで指定された色を取得する
            var result = TripletColor.Match(Mail);
            if(result.Success) {

                CommentColor = result.Value;
            }

            //コメントサイズ
            if(Mail.Contains("big")) {

                CommentSize = "big";
            } else if(Mail.Contains("small")) {

                CommentSize = "small";
            } else {

                CommentSize = "regular";
            }

            //フォントを設定
            if(Mail.Contains("gothic")) {

                Font = "gothic";
            } else if(Mail.Contains("mincho")) {

                Font = "mincho";
            } else {

                Font = "defont";
            }

            if(Mail.Contains("full")) {

                Full = true;
            }

            //投稿者コメントの秒数指定コメント
            if(IsUploader) {

                //@は半角にしましょうね
                if(Content.StartsWith("＠")) {

                    Content = Content.Replace("＠", "@");
                }

                var dur = DurationRegex.Match(Mail);
                if(dur.Success) {

                    //投コメの時間調整
                    Duration = int.Parse(dur.Groups[1].Value) * 1000;
                    Vend = Vpos + (Duration / 10);
                }

            }

            DefaultCommentSize = Settings.Instance.CommentSize;


            Opacity = Settings.Instance.CommentAlpha / 100.0;

            //改行の数を数えて
            Scale = IsOverflowHeight(Content.ToList().Where(c => c.Equals('\n')).Count() + 1) ? 0.5 : 1.0;
        }

        //高さがオーバーフローしてるか offsetはコメントの行数
        private bool IsOverflowHeight(int offset) {

            if(offset == 1) {

                return false;
            }
            if(Mail.Contains("big")) {

                return offset >= 3;
            } else if(Mail.Contains("small")) {

                return offset >= 7;
            } else {

                return offset >= 5;
            }
        }


    }

    public enum CommentType {

        Normal,
        NormalLeaves,
        Uploader,
        Community,
        Channel,
        ChannelLeaves,
        Official,
        NicoScript
    }

}
