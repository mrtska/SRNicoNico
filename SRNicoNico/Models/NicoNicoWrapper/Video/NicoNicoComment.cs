using Codeplex.Data;
using SRNicoNico.Models.NicoNicoViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoComment {

    //resultcode MEMO
    //"THREAD_FOUND": 0
    //"THREAD_NOT_FOUND": 1
    //"THREAD_INVALID": 2
    //"THREAD_VERSION": 3
    //"THREAD_INVALID_WAYBACKKEY": 4
    //"THREAD_TOO_OLD_WAYBACKKEY": 5
    //"THREAD_INVALID_ADMINKEY": 6
    //"THREAD_TOO_OLD_ADMINKEY": 7
    //"THREAD_INVALID_THREADKEY": 8
    //"THREAD_TOO_OLD_THREADKEY": 9
    //"THREAD_ADMIN_CONFLICT": 10
    //"THREAD_LEAF_NOT_ACTIVATED": 11
    //"THREAD_LEAF_LOADING": 12
    //"THREAD_INVALID_LANGUAGE": 13
    //"THREAD_INVALID_USERKEY": 14
    //"THREAD_TOO_OLD_USERKEY": 15
    //"THREAD_CONFLICT_USERKEY_AND_OTHERKEY": 16
    //"THREAD_USER_CONFLICT": 17

    //"POST_SUCCESS": 0
    //"POST_FAILURE": 1
    //"POST_INVALID_THREAD": 2
    //"POST_INVALID_TICKET": 3
    //"POST_INVALID_POSTKEY": 4
    //"POST_LOCKED": 5
    //"POST_READONLY": 6
    //"POST_NOT_IMPLEMENTED": 7
    //"POST_INVALID_184": 8

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

        // スレッドキー取得API
        private const string ThreadKeyApiUrl = "https://flapi.nicovideo.jp/api/getthreadkey";

        // ポストキー取得API
        private const string PostKeyApiUrl = "https://flapi.nicovideo.jp/api/getpostkey";

        private long RequestId = 0;
        private long PacketId = 0;

        private readonly NicoNicoWatchApi ApiData;

        public NicoNicoComment(NicoNicoWatchApi apiData) {

            ApiData = apiData;
        }

        public async Task<List<NicoNicoCommentList>> GetAllCommentAsync() {

            //パケットIDとコメントタイプを紐付ける
            var TypeResolver = new Dictionary<long, NicoNicoCommentList>();

            var array = new List<object>();
            array.Add(new { ping = new { content = "rs:" + RequestId % IdLimit } });
            foreach (var composite in ApiData.ThreadInfo.Composites) {

                //アクティブじゃないコメントスレッドは無視
                if (!composite.IsActive || composite.Label != "default") {

                    continue;
                }
                array.Add(new { ping = new { content = "ps:" + PacketId % IdLimit } });

                object thread = null;

                if(composite.Fork) {

                    thread = new {

                        fork = 1,
                        language = 0,
                        nicoru = 0,
                        scores = 1,
                        thread = composite.Id,
                        user_id = ApiData.ViewerInfo.Id,
                        userkey = ApiData.UserKey,
                        version = "20090904",
                        with_global = 1,
                        res_from = -1000
                    };
                } else {

                    thread = new {

                        fork = 0,
                        language = 0,
                        nicoru = 0,
                        scores = 1,
                        thread = composite.Id,
                        user_id = ApiData.ViewerInfo.Id,
                        userkey = ApiData.UserKey,
                        version = "20090904",
                        with_global = 1
                    };
                }

                var threadkey = "";
                var force184 = "";
                if (composite.IsThreadKeyRequired) {

                    var query = new GetRequestQuery(ThreadKeyApiUrl);
                    query.AddQuery("thread", composite.Id);
                    var dic = HttpUtility.ParseQueryString(await App.ViewModelRoot.CurrentUser.Session.GetAsync(query.TargetUrl));
                    threadkey = dic["threadkey"];
                    force184 = dic["force_184"];

                    thread = new {

                        fork = composite.Fork ? 1 : 0,
                        language = 0,
                        nicoru = 0,
                        scores = 1,
                        thread = composite.Id,
                        user_id = ApiData.ViewerInfo.Id,
                        threadkey,
                        version = "20090904",
                        with_global = 1,
                        force_184 = force184
                    };
                    if (ApiData.UploaderInfo != null && ApiData.UploaderInfo.IsChannel) {

                        var list = new NicoNicoCommentList(CommentType.Channel, composite) {
                            ThreadKey = threadkey,
                            Force184 = force184
                        };
                        TypeResolver[PacketId] = list;
                    } else {

                        var list = new NicoNicoCommentList(CommentType.Community, composite) {
                            ThreadKey = threadkey,
                            Force184 = force184
                        };
                        TypeResolver[PacketId] = list;
                    }
                } else {

                    if(composite.IsOwnerThread) {

                        TypeResolver[PacketId] = new NicoNicoCommentList(CommentType.Uploader, composite);
                    } else if(composite.Label == "nicos") {

                        TypeResolver[PacketId] = new NicoNicoCommentList(CommentType.NicoScript, composite);
                    } else {

                        TypeResolver[PacketId] = new NicoNicoCommentList(CommentType.Normal, composite);
                    }
                }

                array.Add(new { thread });

                array.Add(new { ping = new { content = "pf:" + PacketId % IdLimit } });
                PacketId++;

                // 葉っぱ
                if (composite.IsLeafRequired || composite.Label == "nicos") {

                    array.Add(new { ping = new { content = "ps:" + PacketId % IdLimit } });

                    object leaves = new {

                        content = $"0-{((ApiData.Duration / 60) + 1)}:100,{GetReadCommentCount(ApiData.Duration)}",
                        language = 0,
                        nicoru = 0,
                        scores = 1,
                        thread = composite.Id,
                        user_id = ApiData.ViewerInfo.Id,
                        userkey = ApiData.UserKey
                    };
                    if (composite.IsThreadKeyRequired) {
                        leaves = new {

                            content = $"0-{((ApiData.Duration / 60) + 1)}:100,{GetReadCommentCount(ApiData.Duration)}",
                            language = 0,
                            nicoru = 0,
                            scores = 1,
                            thread = composite.Id,
                            user_id = ApiData.ViewerInfo.Id,
                            threadkey,
                            force_184 = force184
                        };
                        if (ApiData.UploaderInfo != null && ApiData.UploaderInfo.IsChannel) {

                            TypeResolver[PacketId] = TypeResolver.Single(e => e.Value.CommentType == CommentType.Channel).Value;
                        } else {

                            TypeResolver[PacketId] = TypeResolver.Single(e => e.Value.CommentType == CommentType.Community).Value;
                        }
                    } else {

                        if (composite.IsOwnerThread) {

                            TypeResolver[PacketId] = TypeResolver.Single(e => e.Value.CommentType == CommentType.Uploader).Value;
                        } else if (composite.Label == "nicos") {

                            TypeResolver[PacketId] = TypeResolver.Single(e => e.Value.CommentType == CommentType.NicoScript).Value;
                        } else {

                            TypeResolver[PacketId] = TypeResolver.Single(e => e.Value.CommentType == CommentType.Normal).Value;
                        }
                    }
                    array.Add(new { thread_leaves = leaves });

                    array.Add(new { ping = new { content = "pf:" + PacketId % IdLimit } });
                    PacketId++;
                }
            }
            array.Add(new { ping = new { content = "rf:" + RequestId % IdLimit } });
            RequestId++;

            var str = DynamicJson.Serialize(array);
            try {

                var request = new HttpRequestMessage(HttpMethod.Post, ApiData.ThreadInfo.ServerUrl) {
                    Content = new StringContent(str, Encoding.UTF8, "text/plain")
                };
                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);
                NicoNicoCommentList currentList = null;
                foreach (var entry in DynamicJson.Parse(a)) {

                    //コメントタイプを確認
                    if (entry.ping()) {

                        var content = entry.ping.content as string;
                        if (content.Contains("ps")) {

                            currentList = TypeResolver[long.Parse(content.Split(':')[1])];
                        }
                        continue;
                    }
                    if (entry.thread()) {

                        var thread = entry.thread;

                        switch (currentList.CommentType) {
                            case CommentType.Normal: {

                                    currentList.Ticket = thread.ticket() ? thread.ticket : "";
                                    currentList.LastRes = thread.last_res() ? (int)thread.last_res : 0;
                                    break;
                                }
                            case CommentType.Uploader: {

                                    break;
                                }
                            case CommentType.Channel: {

                                    currentList.Ticket = thread.ticket() ? thread.ticket : "";
                                    currentList.LastRes = thread.last_res() ? (int)thread.last_res : 0;
                                    break;
                                }
                        }
                        continue;
                    }

                    if (entry.chat()) {

                        var chat = entry.chat;
                        if (chat.deleted()) {

                            continue;
                        }
                        var item = new NicoNicoCommentEntry() {
                            Number = (int)chat.no,
                            Vpos = (int)chat.vpos,
                            PostedAt = (long)chat.date,
                            Content = chat.content() ? chat.content : "",
                            Mail = chat.mail() ? chat.mail : ""
                        };

                        switch (currentList.CommentType) {
                            case CommentType.Normal:

                                item.Anonymity = chat.anonymity();
                                item.UserId = chat.user_id;
                                item.Score = chat.score() ? (int)chat.score : 0;
                                break;
                            case CommentType.Uploader:

                                item.Mail = new string(item.Mail.Select(n => (ConvMap.ContainsKey(n) ? ConvMap[n] : n)).ToArray());
                                item.IsUploader = true;
                                break;
                            case CommentType.Community:
                            case CommentType.Channel:

                                item.Anonymity = chat.anonymity();
                                item.UserId = chat.user_id;
                                item.Score = chat.score() ? (int)chat.score : 0;
                                break;
                        }
                        item.DisassembleMail();
                        currentList.Add(item);
                    }
                }
                return TypeResolver.ToList().Select(e => e.Value).Distinct().ToList();
            } catch (RequestFailed) {

                return null;
            }
        }
        public async Task<NicoNicoCommentList> RefreshCommentAsync(NicoNicoCommentList list) {

            var array = new List<object>();
            array.Add(new { ping = new { content = "rs:" + RequestId % IdLimit } });
            array.Add(new { ping = new { content = "ps:" + PacketId % IdLimit } });
            var composite = list.Composite;

            object thread = new {

                res_from = list.LastRes + 1,
                fork = composite.Fork ? 1 : 0,
                language = 0,
                nicoru = 0,
                scores = 0,
                thread = composite.Id,
                user_id = ApiData.ViewerInfo.Id,
                userkey = ApiData.UserKey,
                version = "20090904",
                with_global = 1
            };
            var threadkey = "";
            var force184 = "";
            if (composite.IsThreadKeyRequired) {

                var query = new GetRequestQuery(ThreadKeyApiUrl);
                query.AddQuery("thread", composite.Id);
                var dic = HttpUtility.ParseQueryString(await App.ViewModelRoot.CurrentUser.Session.GetAsync(query.TargetUrl));
                threadkey = dic["threadkey"];
                force184 = dic["force_184"];

                thread = new {

                    res_from = list.LastRes + 1,
                    fork = composite.Fork ? 1 : 0,
                    language = 0,
                    nicoru = 0,
                    scores = 1,
                    thread = composite.Id,
                    user_id = ApiData.ViewerInfo.Id,
                    threadkey,
                    version = "20090904",
                    with_global = 1,
                    force_184 = force184
                };
            }
            array.Add(new { thread });

            array.Add(new { ping = new { content = "pf:" + PacketId % IdLimit } });
            PacketId++;
            array.Add(new { ping = new { content = "rf:" + RequestId % IdLimit } });
            RequestId++;

            try {
                var str = DynamicJson.Serialize(array);

                var request = new HttpRequestMessage(HttpMethod.Post, ApiData.ThreadInfo.ServerUrl) {
                    Content = new StringContent(str, Encoding.UTF8, "text/plain")
                };
                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);
                foreach (var entry in DynamicJson.Parse(a)) {

                    if (entry.thread()) {

                        var t = entry.thread;
                        list.Ticket = t.ticket() ? t.ticket : "";
                        list.LastRes = t.last_res() ? (int)t.last_res : 0;
                        continue;
                    }

                    if (entry.chat()) {

                        var chat = entry.chat;
                        if (chat.deleted()) {

                            continue;
                        }
                        var item = new NicoNicoCommentEntry() {
                            Number = (int)chat.no,
                            Vpos = (int)chat.vpos,
                            PostedAt = (long)chat.date,
                            Content = chat.content() ? chat.content : "",
                            Mail = chat.mail() ? chat.mail : ""
                        };

                        switch (list.CommentType) {
                            case CommentType.Normal:

                                item.Anonymity = chat.anonymity();
                                item.UserId = chat.user_id;
                                item.Score = chat.score() ? (int)chat.score : 0;
                                break;
                            case CommentType.Uploader:

                                item.Mail = new string(item.Mail.Select(n => (ConvMap.ContainsKey(n) ? ConvMap[n] : n)).ToArray());
                                item.IsUploader = true;
                                break;
                            case CommentType.Community:
                            case CommentType.Channel:

                                item.Anonymity = chat.anonymity();
                                item.UserId = chat.user_id;
                                item.Score = chat.score() ? (int)chat.score : 0;
                                break;
                        }
                        item.DisassembleMail();
                        list.Add(item);
                    }
                }


                return list;
            } catch(RequestFailed) {

                return null;
            }
        }



        /// <summary>
        /// コメントを投稿
        /// </summary>
        /// <param name="vpos"></param>
        /// <param name="mail"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<int> PostAsync(NicoNicoCommentList list, int vpos, string mail, string content) {

            try {

                var composite = list.Composite;
                var query = new GetRequestQuery(PostKeyApiUrl);
                query.AddQuery("thread", composite.Id);
                query.AddQuery("block_no", Math.Floor((decimal)(list.LastRes + 1) / 100).ToString());
                query.AddQuery("device", "1");
                query.AddQuery("version", "1");
                query.AddQuery("version_sub", "6");
                var postkey = await App.ViewModelRoot.CurrentUser.Session.GetAsync(query.TargetUrl);
                postkey = postkey.Split('=')[1];
                var array = new List<object>();
                array.Add(new { ping = new { content = "rs:" + RequestId % IdLimit } });
                array.Add(new { ping = new { content = "ps:" + PacketId % IdLimit } });

                var chat = new {

                    content,
                    mail,
                    postkey,
                    premium = ApiData.ViewerInfo.IsPremium ? 1 : 0,
                    thread = composite.Id,
                    ticket = list.Ticket,
                    user_id = ApiData.ViewerInfo.Id,
                    vpos
                };
                array.Add(new { chat });

                array.Add(new { ping = new { content = "pf:" + PacketId % IdLimit } });
                PacketId++;
                array.Add(new { ping = new { content = "rf:" + RequestId % IdLimit } });
                RequestId++;

                var str = DynamicJson.Serialize(array);
                var request = new HttpRequestMessage(HttpMethod.Post, ApiData.ThreadInfo.ServerUrl) {
                    Content = new StringContent(str, Encoding.UTF8, "text/plain")
                };
                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                foreach (var entry in DynamicJson.Parse(a)) {

                    if (entry.chat_result()) {

                        if (entry.chat_result.status == 0D) {

                            return (int)entry.chat_result.no;
                        } else {

                            return -1;
                        }
                    }
                }
                return 0;
            } catch (RequestFailed) {

                return -1;
            }
        }

        //動画の長さによって取得するコメントの量を変える
        public int GetReadCommentCount(int length) {

            if (length < 60) {

                return 100;
            }
            if (length < 300) {

                return 250;
            }
            if (length < 600) {

                return 500;
            }
            return 1000;
        }

    }
    public enum CommentType {

        Normal,
        Uploader,
        Community,
        Channel,
        ExtCommunity,
        NicoScript
    }
}
