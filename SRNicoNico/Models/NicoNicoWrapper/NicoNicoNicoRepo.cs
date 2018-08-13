using Codeplex.Data;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoNicoRepo : NotificationObject {

        private static readonly Dictionary<string, string> RankingSpan = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> RankingType = new Dictionary<string, string>();

        static NicoNicoNicoRepo() {

            RankingSpan["hourly"] = "毎時";
            RankingSpan["daily"] = "24時間";
            RankingSpan["weekly"] = "週間";
            RankingSpan["monthly"] = "月間";
            RankingSpan["total"] = "合計";

            RankingType["fav"] = "総合";
            RankingType["view"] = "再生";
            RankingType["res"] = "コメント";
            RankingType["mylist"] = "マイリスト";
        }

        #region IsEmpty変更通知プロパティ
        private bool _IsEmpty;

        public bool IsEmpty {
            get { return _IsEmpty; }
            set {
                if (_IsEmpty == value)
                    return;
                _IsEmpty = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region IsEnd変更通知プロパティ
        private bool _IsEnd;

        public bool IsEnd {
            get { return _IsEnd; }
            set { 
                if (_IsEnd == value)
                    return;
                _IsEnd = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private readonly List<NicoNicoNicoRepoResultEntry> UnFilteredNicoRepoList = new List<NicoNicoNicoRepoResultEntry>();

        #region NicoRepoList変更通知プロパティ
        private ObservableSynchronizedCollection<INicoRepo> _NicoRepoList;

        public ObservableSynchronizedCollection<INicoRepo> NicoRepoList {
            get { return _NicoRepoList; }
            set { 
                if (_NicoRepoList == value)
                    return;
                _NicoRepoList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private readonly string Type;

        private string Filter = "すべて";

        private string Next = "";

        public NicoNicoNicoRepo(string type) {

            Type = type;
            NicoRepoList = new ObservableSynchronizedCollection<INicoRepo>();
        }

        public void SetFilter(string filter) {

            Filter = filter;
            Filtering();
        }

        public void Reset() {

            Next = "";
            UnFilteredNicoRepoList.Clear();
            NicoRepoList.Clear();
            IsEnd = false;
            IsEmpty = false;
        }

        public async Task<string> GetNicoRepoAsync() {

            if (IsEnd) {

                return "";
            }
            try {

                if(NicoRepoList.LastOrDefault() is NicoNicoNextButtonEntry button) {

                    NicoRepoList.Remove(button);
                }

                GetRequestQuery query;
                if (Regex.IsMatch(Type, @"\d+")) {

                    query = new GetRequestQuery("http://www.nicovideo.jp/api/nicorepo/timeline/user/" + Type);
                    query.AddQuery("client_app", "pc_profilerepo");
                } else {

                    query = new GetRequestQuery("http://www.nicovideo.jp/api/nicorepo/timeline/my/" + Type);
                    query.AddQuery("client_app", "pc_myrepo");
                }
                
                if (!string.IsNullOrEmpty(Next)) {

                    query.AddQuery("cursor", Next);
                }
                query.AddQuery("_", UnixTime.ToUnixTime(DateTime.Now) + "000");

                //ニコレポのhtmlを取得
                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(query.TargetUrl);

                dynamic json = DynamicJson.Parse(a);

                void storeSenderUser(NicoNicoNicoRepoResultEntry item, dynamic sender) {

                    item.SenderId = (int)sender.id;
                    item.SenderName = sender.nickname;
                    item.SenderUrl = "http://www.nicovideo.jp/user/" + sender.id;
                    item.SenderThumbnail = sender.icons.tags.defaultValue.urls.s50x50;
                }
                void storeSenderChannel(NicoNicoNicoRepoResultEntry item, dynamic sender) {

                    item.SenderId = (int)sender.id;
                    item.SenderName = sender.name;
                    item.SenderUrl = sender.url;
                    item.SenderThumbnail = sender.thumbnailUrl;
                }
                void storeSenderCommunity(NicoNicoNicoRepoResultEntry item, dynamic sender) {

                    item.SenderId = (int)sender.id;
                    item.SenderName = sender.name;
                    item.SenderUrl = "http://com.nicovideo.jp/community/" + sender.id;
                    item.SenderThumbnail = sender.thumbnailUrl.small;
                }
                void storeCommunity(NicoNicoNicoRepoResultEntry item, dynamic com) {

                    item.CommunityId = com.id;
                    item.CommunityName = com.name;
                    item.CommunityThumbnail = com.thumbnailUrl.small;
                }
                void storeArticle(NicoNicoNicoRepoGenericEntry item, dynamic article) {

                    item.ContentId = article.id.ToString();
                    item.ContentTitle = article.title;
                    item.ContentUrl = article.watchUrls.pcUrl;
                    item.ContentThumbnail = article.thumbnailUrl;
                }
                void storeVideo(NicoNicoNicoRepoVideoEntry item, dynamic video) {

                    item.VideoId = video.id;
                    item.Status = video.status;
                    item.VideoThumbnail = video.thumbnailUrl.normal;
                    item.VideoTitle = video.title;
                    item.VideoWatchPageId = video.videoWatchPageId;
                    item.ContentUrl = "http://www.nicovideo.jp/watch/" + item.VideoWatchPageId;
                }
                void storeLive(NicoNicoNicoRepoLiveEntry item, dynamic program) {

                    item.ProgramId = program.id;
                    item.ProgramBeginAt = DateTime.Parse(program.beginAt);
                    item.IsPayProgram = program.isPayProgram;
                    item.ProgramThumbnail = program.thumbnailUrl;
                    item.ProgramTitle = program.title;
                    item.ContentUrl = "http://live.nicovideo.jp/watch/" + item.ProgramId;
                }

                if(!json.data()) {

                    return "ニコニコ側でエラーが発生しました";
                }

                //ニコレポタイムラインを走査
                foreach (var entry in json.data) {

                    NicoNicoNicoRepoResultEntry item = null;

                    //Console.WriteLine(entry.topic);
                    switch(entry.topic) {
                        case "live.channel.program.onairs":
                        case "live.channel.program.reserve": {

                                var sender = entry.senderChannel;
                                var program = entry.program;

                                item = new NicoNicoNicoRepoLiveEntry();

                                storeSenderChannel(item, sender);
                                storeLive(item as NicoNicoNicoRepoLiveEntry, program);

                                var itemc = item as NicoNicoNicoRepoLiveEntry;
                                if (((string)entry.topic).EndsWith("onairs")) {

                                    item.ComputedTitle = string.Format("チャンネル <a href=\"" + item.SenderUrl + "\">{0}</a> で生放送が開始されました。", item.SenderName);
                                } else {

                                    item.ComputedTitle = string.Format("チャンネル <a href=\"" + item.SenderUrl + "\">{0}</a> で {1:yy年M月d日 h時 m分} に生放送が予約されました。", item.SenderName, DateTime.Parse(program.beginAt));
                                }
                                break;
                            }
                        case "live.user.program.cas.onairs": {

                                var sender = entry.senderNiconicoUser;
                                var program = entry.program;

                                item = new NicoNicoNicoRepoLiveEntry();

                                storeSenderUser(item, sender);
                                storeLive(item as NicoNicoNicoRepoLiveEntry, program);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが生放送が開始しました。", item.SenderName);
                                break;
                            }
                        case "live.user.program.onairs":
                        case "live.user.program.reserve": {

                                var sender = entry.senderNiconicoUser;
                                var program = entry.program;
                                var com = entry.community;

                                item = new NicoNicoNicoRepoLiveEntry();

                                storeSenderUser(item, sender);
                                storeCommunity(item, com);
                                storeLive(item as NicoNicoNicoRepoLiveEntry, program);

                                if (((string)entry.topic).EndsWith("onairs")) {

                                    item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんがコミュニティ <a href=\"" + item.CommunityUrl + "\">{1}</a> で生放送が開始されました。", item.SenderName, item.CommunityName);
                                } else {

                                    item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんがコミュニティ <a href=\"" + item.CommunityUrl + "\">{1}</a> で {2:yy年M月d日 h時 m分} に生放送が予約しました。", item.SenderName, item.CommunityName, DateTime.Parse(program.beginAt));
                                }
                                break;
                            }
                        case "nicoad.user.advertise.video": {

                                var sender = entry.senderNiconicoUser;
                                var video = entry.video;

                                item = new NicoNicoNicoRepoVideoEntry();
                                storeSenderUser(item, sender);
                                storeVideo(item as NicoNicoNicoRepoVideoEntry, video);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが ニコニ広告しました。", item.SenderName);
                                break;
                            }
                        case "nicoad.user.advertised.video.announce": {

                                var sender = entry.senderNiconicoUser;
                                var video = entry.video;

                                item = new NicoNicoNicoRepoVideoEntry();
                                storeSenderUser(item, sender);
                                storeVideo(item as NicoNicoNicoRepoVideoEntry, video);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんの動画が {1} さんにニコニ広告されました。", item.SenderName, entry.nicoad.advertiserName);

                                break;
                            }
                        case "nicoad.user.advertise.program": {

                                var sender = entry.senderNiconicoUser;
                                var program = entry.program;

                                item = new NicoNicoNicoRepoLiveEntry();
                                storeSenderUser(item, sender);
                                storeLive(item as NicoNicoNicoRepoLiveEntry, program);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんがニコニ広告しました。", item.SenderName);
                                break;
                            }
                        case "nicoad.user.advertised.program.announce": {

                                var sender = entry.senderNiconicoUser;
                                var program = entry.program;

                                item = new NicoNicoNicoRepoLiveEntry();
                                storeSenderUser(item, sender);
                                storeLive(item as NicoNicoNicoRepoLiveEntry, program);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんの生放送が {1} さんにニコニ広告されました。", item.SenderName, entry.nicoad.advertiserName);

                                break;
                            }
                        case "nicoad.user.advertise.program.cas": {

                                var sender = entry.senderNiconicoUser;
                                var program = entry.program;

                                item = new NicoNicoNicoRepoLiveEntry();
                                storeSenderUser(item, sender);
                                storeLive(item as NicoNicoNicoRepoLiveEntry, program);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんがニコニ広告しました。", item.SenderName);
                                break;
                            }
                        case "nicoad.user.advertised.program.cas.announce": {

                                var sender = entry.senderNiconicoUser;
                                var program = entry.program;

                                item = new NicoNicoNicoRepoLiveEntry();
                                storeSenderUser(item, sender);
                                storeLive(item as NicoNicoNicoRepoLiveEntry, program);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんのnicocas番組が {1} さんにニコニ広告されました。", item.SenderName, entry.nicoad.advertiserName);

                                break;
                            }
                        case "nicoseiga.user.illust.clip":
                        case "nicoseiga.user.illust.upload": {

                                var sender = entry.senderNiconicoUser;
                                var seiga = entry.illustImage;

                                item = new NicoNicoNicoRepoSeigaEntry() {
                                    SeigaId = seiga.id,
                                    SeigaTitle = seiga.title,
                                    SeigaThumbnail = seiga.thumbnailUrl,
                                    SeigaUrl = seiga.urls.pcUrl,
                                    ContentUrl = seiga.urls.pcUrl
                                };
                                storeSenderUser(item, sender);
                                

                                if (((string)entry.topic).EndsWith("clip")) {

                                    item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが イラストをクリップしました。", item.SenderName);
                                } else {

                                    item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが <strong>イラストを投稿しました。</strong>", item.SenderName);
                                }
                                break;
                            }
                        case "nicoseiga.user.manga.episode.upload":
                        case "nicoseiga.user.manga.content.favorite": {

                                var sender = entry.senderNiconicoUser;
                                var manga = entry.mangaContent;

                                item = new NicoNicoNicoRepoMangaEntry() {
                                    MangaId = manga.id,
                                    MangaTitle = manga.title,
                                    MangaThumbnail = manga.thumbnailUrl,
                                    MangaUrl = manga.urls.pcUrl,
                                    ContentUrl = manga.urls.pcUrl
                                };
                                storeSenderUser(item, sender);

                                if (((string)entry.topic).EndsWith("upload")) {

                                    var episode = entry.mangaEpisode;
                                    var itemc = item as NicoNicoNicoRepoMangaEntry;
                                    itemc.MangaEpisodeId = episode.themeId;
                                    itemc.MangaEpisodeTitle = episode.title;
                                    itemc.MangaEpisodeThumbnail = episode.thumbnailUrl;
                                    itemc.MangaEpisodeUrl = episode.urls.pcUrl;

                                    item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが <strong>マンガ {1} を投稿しました。</strong>", item.SenderName, itemc.MangaTitle);
                                } else {

                                    item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが マンガをお気に入りしました。", item.SenderName);
                                }
                                break;
                            }
                        case "nicovideo.channel.blomaga.upload": {

                                var sender = entry.senderChannel;
                                var content = entry.channelArticle;

                                item = new NicoNicoNicoRepoGenericEntry() {
                                    ContentId = content.id.ToString(),
                                    ContentTitle = content.title,
                                    ContentThumbnail = content.thumbnailUrl,
                                    ContentUrl = content.watchUrls.pcUrl
                                };
                                storeSenderChannel(item, sender);
                                

                                item.ComputedTitle = string.Format("チャンネル <a href=\"" + item.SenderUrl + "\">{0}</a>  に記事が追加されました。", item.SenderName);
                                break;
                            }
                        case "nicovideo.channel.info.add": {

                                var sender = entry.senderChannel;
                                var content = entry.channelNotice;

                                item = new NicoNicoNicoRepoGenericEntry() {
                                    ContentId = sender.id.ToString(),
                                    ContentTitle = content.title,
                                    ContentThumbnail = sender.thumbnailUrl,
                                    ContentUrl = sender.url
                                };
                                storeSenderChannel(item, sender);

                                item.ComputedTitle = string.Format("チャンネル <a href=\"" + item.SenderUrl + "\">{0}</a>  にお知らせが追加されました。", item.SenderName);
                                break;
                            }
                        case "nicovideo.channel.video.upload": {

                                var sender = entry.senderChannel;
                                var video = entry.video;
                                item = new NicoNicoNicoRepoVideoEntry();

                                storeSenderChannel(item, sender);
                                storeVideo(item as NicoNicoNicoRepoVideoEntry, video);

                                item.ComputedTitle = string.Format("チャンネル <a href=\"" + item.SenderUrl + "\">{0}</a>  に動画が追加されました。", item.SenderName);
                                break;
                            }
                        case "nicovideo.community.level.raise": {

                                var sender = entry.senderCommunity;
                                item = new NicoNicoNicoRepoNoContentEntry();

                                storeSenderCommunity(item, sender);
                                item.ContentUrl = item.CommunityUrl;

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> の コミュニティレベルが {1} になりました。 {2} が付与されました。", item.SenderName, entry.actionLog.newHighestLevel, entry.actionLog.communityLevelPrivilege);
                                break;
                            }
                        case "nicovideo.user.app.install": {

                                var sender = entry.senderNiconicoUser;
                                var app = entry.app;
                                item = new NicoNicoNicoRepoGenericEntry() {

                                    ContentTitle = app.title,
                                    ContentId = app.id
                                };
                                var itemc = item as NicoNicoNicoRepoGenericEntry;
                                itemc.ContentThumbnail = "http://appicon.nimg.jp/" + itemc.ContentId.Replace("ap", "") + "/icon96.png";
                                itemc.ContentUrl = "http://app.nicovideo.jp/app/" + itemc.ContentId;

                                storeSenderUser(item, sender);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが アプリを遊びはじめました。", item.SenderName);
                                break;
                            }
                        case "nicovideo.user.blomaga.upload": {

                                var sender = entry.senderNiconicoUser;
                                var article = entry.channelArticle;

                                item = new NicoNicoNicoRepoGenericEntry();
                                storeSenderUser(item, sender);
                                storeArticle(item as NicoNicoNicoRepoGenericEntry, article);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが記事を投稿しました。", item.SenderName);
                                break;
                            }
                        case "nicovideo.user.community.info.add": {

                                var sender = entry.senderNiconicoUser;
                                var com = entry.communityForFollower;
                                var notice = entry.communityNotice;

                                item = new NicoNicoNicoRepoGenericEntry() {

                                    ContentTitle = notice.title
                                };
                                storeSenderUser(item, sender);
                                storeCommunity(item, com);

                                var itemc = item as NicoNicoNicoRepoGenericEntry;
                                itemc.ContentThumbnail = item.CommunityThumbnail;
                                itemc.ContentUrl = item.CommunityUrl;

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a>  コミュニティ <a href=\"" + item.CommunityUrl + "\">{1}</a> にお知らせを追加しました。", item.SenderName, item.CommunityName);
                                break;
                            }
                        case "nicovideo.user.community.video.add": {

                                var sender = entry.senderNiconicoUser;
                                var com = entry.communityForFollower;
                                var video = entry.memberOnlyVideo;

                                item = new NicoNicoNicoRepoVideoEntry();
                                storeSenderUser(item, sender);
                                storeCommunity(item, com);
                                storeVideo(item as NicoNicoNicoRepoVideoEntry, video);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a>  コミュニティ <a href=\"" + item.CommunityUrl + "\">{1}</a> に動画を追加しました。", item.SenderName, item.CommunityName);
                                break;
                            }
                        case "nicovideo.user.community_member_only_video.upload": {

                                var sender = entry.senderNiconicoUser;
                                var video = entry.memberOnlyVideo;

                                item = new NicoNicoNicoRepoVideoEntry();
                                storeSenderUser(item, sender);
                                storeVideo(item as NicoNicoNicoRepoVideoEntry, video);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが <strong>コミュニティ専用動画を投稿しました。</strong>", item.SenderName);
                                break;
                            }
                        case "nicovideo.user.followed_announce": {

                                var sender = entry.followerNiconicoUser;

                                item = new NicoNicoNicoRepoNoContentEntry();
                                storeSenderUser(item, sender);
                                item.ContentUrl = item.SenderUrl;

                                item.ComputedTitle = string.Format("あなたを <a href=\"" + item.SenderUrl + "\">{0}</a> さんがフォローしました。", item.SenderName);
                                break;
                            }
                        case "nicovideo.user.knowledge.upload": {

                                var sender = entry.senderNiconicoUser;
                                var knowledge = entry.knowledge;

                                item = new NicoNicoNicoRepoGenericEntry() {

                                    ContentTitle = knowledge.title,
                                    ContentId = "kn" + knowledge.id,
                                    ContentUrl = "http://niconare.nicovideo.jp/watch/kn" + knowledge.id,
                                    ContentThumbnail = knowledge.thumbnailUrl
                                };
                                storeSenderUser(item, sender);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが ナレッジを投稿しました。", item.SenderName);
                                break;
                            }
                        case "nicovideo.user.mylist.add.blomaga.article": {

                                var sender = entry.senderNiconicoUser;
                                var article = entry.channelArticle;

                                item = new NicoNicoNicoRepoGenericEntry();
                                storeSenderUser(item, sender);
                                storeArticle(item as NicoNicoNicoRepoGenericEntry, article);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが マイリスト <a href=\"http://www.nicovideo.jp/mylist/" + entry.mylist.id + "\">{1}</a> にブロマガを登録しました。", item.SenderName, entry.mylist.name);
                                break;
                            }
                        case "nicovideo.user.mylist.add.book": {

                                var sender = entry.senderNiconicoUser;
                                var book = entry.book;
                                var mylist = entry.mylist;

                                item = new NicoNicoNicoRepoGenericEntry() {
                                    ContentTitle = book.name,
                                    ContentId = "bk" + book.id,
                                    ContentThumbnail = book.thumbnailUrl,
                                    ContentUrl = "http://seiga.nicovideo.jp/watch/bk" + book.id
                                };
                                storeSenderUser(item, sender);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが マイリスト <a href=\"http://www.nicovideo.jp/mylist/" + mylist.id + "\">{1}</a> に書籍を登録しました。", item.SenderName, mylist.name);
                                break;
                            }
                        case "nicovideo.user.mylist.add.manga.episode": {

                                var sender = entry.senderNiconicoUser;
                                var manga = entry.mangaContent;
                                var episode = entry.mangaEpisode;
                                var mylist = entry.mylist;

                                item = new NicoNicoNicoRepoGenericEntry() {
                                    ContentTitle = episode.title,
                                    ContentId = "mg" + episode.themeId,
                                    ContentThumbnail = episode.thumbnailUrl,
                                    ContentUrl = episode.urls.pcUrl
                                };
                                storeSenderUser(item, sender);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが マイリスト <a href=\"http://www.nicovideo.jp/mylist/" + mylist.id + "\">{1}</a> にマンガ <a href=\"" + manga.urls.pcUrl + "\">{2}</a> を登録しました。", item.SenderName, mylist.name, manga.title);
                                break;
                            }
                        case "nicovideo.user.mylist.add.video": {

                                var sender = entry.senderNiconicoUser;
                                var video = entry.video;
                                var mylist = entry.mylist;

                                item = new NicoNicoNicoRepoVideoEntry();
                                storeSenderUser(item, sender);
                                storeVideo(item as NicoNicoNicoRepoVideoEntry, video);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが マイリスト <a href=\"http://www.nicovideo.jp/mylist/" + mylist.id + "\">{1}</a> に動画を登録しました。", item.SenderName, mylist.name);
                                break;
                            }
                        case "nicovideo.user.mylist.followed_announce": {

                                var sender = entry.senderNiconicoUser;
                                var mylist = entry.mylist;

                                item = new NicoNicoNicoRepoGenericEntry() {

                                    ContentTitle = mylist.name,
                                    ContentUrl = "http://www.nicovideo.jp/mylist/" + mylist.id
                                };
                                storeSenderUser(item, sender);

                                item.ComputedTitle = string.Format("あなたの マイリスト <a href=\"http://www.nicovideo.jp/mylist/" + mylist.id + "\">{0}</a> を <a href=\"" + item.SenderUrl + "\">{1}</a> さんがフォローしました。", mylist.name, item.SenderName);
                                break;
                            }
                        case "nicovideo.user.nicogame.update":
                        case "nicovideo.user.nicogame.upload": {

                                var sender = entry.senderNiconicoUser;
                                var game = entry.game;

                                item = new NicoNicoNicoRepoGenericEntry() {

                                    ContentTitle = game.title,
                                    ContentId = game.id,
                                    ContentUrl = "http://game.nicovideo.jp/atsumaru/games/" + game.id,
                                    ContentThumbnail = game.thumbnailUrl
                                };
                                storeSenderUser(item, sender);

                                if (((string)entry.topic).EndsWith("update")) {

                                    item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが<strong>ゲームを更新しました。</strong>", item.SenderName);
                                } else {

                                    item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが<strong>ゲームを投稿しました。</strong>", item.SenderName);
                                }
                                break;
                            }
                        case "nicovideo.user.solid.distribute":
                        case "nicovideo.user.solid.favorite":
                        case "nicovideo.user.solid.update":
                        case "nicovideo.user.solid.upload": {

                                var sender = entry.senderNiconicoUser;
                                var solid = entry.solid;

                                item = new NicoNicoNicoRepoGenericEntry() {

                                    ContentTitle = solid.title,
                                    ContentId = solid.id,
                                    ContentUrl = "http://3d.nicovideo.jp/works/" + solid.id,
                                    ContentThumbnail = solid.thumbnailUrl
                                };
                                storeSenderUser(item, sender);

                                if (((string)entry.topic).EndsWith("distribute")) {

                                    item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが立体の配布データを公開しました。", item.SenderName);
                                } else if (((string)entry.topic).EndsWith("favorite")) {

                                    item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが立体の配布データをお気に入り登録しました。", item.SenderName);
                                } else if (((string)entry.topic).EndsWith("update")) {

                                    item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが立体の配布データを更新しました。", item.SenderName);
                                } else {

                                    item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが立体の配布データを投稿しました。", item.SenderName);
                                }
                                break;
                            }
                        case "nicovideo.user.stamp.obtain": {

                                var sender = entry.senderNiconicoUser;
                                var stamp = entry.stamp;

                                item = new NicoNicoNicoRepoGenericEntry() {

                                    ContentId = stamp.code,
                                    ContentTitle = stamp.name,
                                    ContentThumbnail = "http://nicovideo.cdn.nimg.jp/uni/img/stamp/" + stamp.code + ".gif",
                                    ContentUrl = "http://www.nicovideo.jp/stamp/" + stamp.code
                                };
                                storeSenderUser(item, sender);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんがスタンプを取得しました。", item.SenderName);

                                break;
                            }
                        case "nicovideo.user.temporary_mylist.add.blomaga.article": {

                                var sender = entry.senderNiconicoUser;
                                var article = entry.channelArticle;

                                item = new NicoNicoNicoRepoGenericEntry();
                                storeSenderUser(item, sender);
                                storeArticle(item as NicoNicoNicoRepoGenericEntry, article);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが とりあえずマイリスト にブロマガを登録しました。", item.SenderName);

                                break;
                            }
                        case "nicovideo.user.temporary_mylist.add.book": {

                                var sender = entry.senderNiconicoUser;
                                var book = entry.book;

                                item = new NicoNicoNicoRepoGenericEntry() {

                                    ContentId = "bk" + book.id,
                                    ContentTitle = book.name,
                                    ContentThumbnail = book.thumbnailUrl,
                                    ContentUrl = "http://seiga.nicovideo.jp/watch/bk" + book.id
                                };
                                storeSenderUser(item, sender);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが とりあえずマイリスト に書籍を登録しました。", item.SenderName);

                                break;
                            }
                        case "nicovideo.user.temporary_mylist.add.manga.episode": {

                                var sender = entry.senderNiconicoUser;
                                var manga = entry.mangaContent;
                                var episode = entry.mangaEpisode;

                                item = new NicoNicoNicoRepoGenericEntry() {
                                    ContentTitle = episode.title,
                                    ContentId = "mg" + episode.themeId,
                                    ContentThumbnail = episode.thumbnailUrl,
                                    ContentUrl = episode.urls.pcUrl
                                };
                                storeSenderUser(item, sender);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが とりあえずマイリスト にマンガ <a href=\"" + manga.urls.pcUrl + "\">{1}</a> を登録しました。", item.SenderName, manga.title);
                                break;
                            }
                        case "nicovideo.user.temporary_mylist.add.video": {

                                var sender = entry.senderNiconicoUser;
                                var video = entry.video;

                                item = new NicoNicoNicoRepoVideoEntry();
                                storeSenderUser(item, sender);
                                storeVideo(item as NicoNicoNicoRepoVideoEntry, video);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが とりあえずマイリスト に動画を登録しました。", item.SenderName);

                                break;
                            }
                        case "nicovideo.user.video.advertise": {

                                var sender = entry.senderNiconicoUser;
                                var video = entry.video;

                                item = new NicoNicoNicoRepoVideoEntry();
                                storeSenderUser(item, sender);
                                storeVideo(item as NicoNicoNicoRepoVideoEntry, video);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが ニコニ広告で宣伝しました。", item.SenderName);
                                break;
                            }
                        case "nicovideo.user.video.advertised_announce": {

                                var sender = entry.senderNiconicoUser;
                                var video = entry.video;

                                item = new NicoNicoNicoRepoVideoEntry();
                                storeSenderUser(item, sender);
                                storeVideo(item as NicoNicoNicoRepoVideoEntry, video);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんの動画が {1} さんにニコニ広告で宣伝されました。", item.SenderName, entry.uad.nickname);

                                break;
                            }
                        case "nicovideo.user.video.kiriban.play": {

                                var video = entry.video;

                                item = new NicoNicoNicoRepoVideoEntry();
                                storeVideo(item as NicoNicoNicoRepoVideoEntry, video);

                                var sender = entry.senderNiconicoUser;
                                storeSenderUser(item, sender);
                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんの動画が {1} 再生を達成しました。", item.SenderName, string.Format("{0:N0}", entry.actionLog.kiriban));
                                break;
                            }
                        case "nicovideo.user.video.live.introduce": {

                                var sender = entry.senderNiconicoUser;
                                var video = entry.video;
                                var live = entry.program;

                                item = new NicoNicoNicoRepoVideoEntry();
                                storeSenderUser(item, sender);
                                storeVideo(item as NicoNicoNicoRepoVideoEntry, video);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんの動画が 生放送 {1} で紹介されました。", item.SenderName, live.title);
                                break;
                            }
                        case "nicovideo.user.video.update_highest_rankings": {

                                var sender = entry.senderNiconicoUser;
                                var video = entry.video;
                                var log = entry.actionLog;

                                item = new NicoNicoNicoRepoVideoEntry();
                                storeSenderUser(item, sender);
                                storeVideo(item as NicoNicoNicoRepoVideoEntry, video);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんの動画が {1} {2} {3} ランキングで {4} 位を達成しました。", item.SenderName, log.rankingCategory, RankingSpan[log.rankingSpan], RankingType[log.rankingType], log.newHighestRanking);

                                break;
                            }
                        case "nicovideo.user.video.upload": {

                                var sender = entry.senderNiconicoUser;
                                var video = entry.video;

                                item = new NicoNicoNicoRepoVideoEntry();
                                storeSenderUser(item, sender);
                                storeVideo(item as NicoNicoNicoRepoVideoEntry, video);

                                item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが <strong>動画を投稿しました。</strong>", item.SenderName);

                                break;
                            }
                        default:
                            throw new InvalidOperationException("対応していないニコレポを検出しました。" + entry.topic);
                    }

                    item.Id = entry.id;
                    item.Topic = entry.topic;
                    item.CreatedAt = DateTime.Parse(entry.createdAt);
                    item.Visible = entry.isVisible;
                    item.Muted = entry.isMuted;

                    if(item is NicoNicoNicoRepoVideoEntry ve) {

                        NicoNicoUtil.ApplyLocalHistory(ve);
                    }

                    UnFilteredNicoRepoList.Add(item);
                    if (FilterEntry(item)) {

                        NicoRepoList.Add(item);
                    }
                }

                if(UnFilteredNicoRepoList.Count == 0) {

                    IsEmpty = true;
                    return "";
                }

                Next = UnFilteredNicoRepoList.Last().Id;

                var datac = 0;
                foreach(var data in json.data) {

                    datac++;
                }

                var errorc = 0;
                foreach(var error in json.errors) {

                    errorc++;
                }

                if((datac + errorc) == 25) {

                    NicoRepoList.Add(new NicoNicoNextButtonEntry());
                } else {

                    IsEnd = true;
                }

                return "";
            } catch(RequestFailed e) {

                if(e.FailedType == FailedType.Failed) {

                    return "ニコレポの取得に失敗しました";
                } else {

                    return "ニコレポの取得がタイムアウトになりました";
                }
            }
        }

        private void Filtering() {

            bool hasButton = false;
            if (NicoRepoList.LastOrDefault() is NicoNicoNextButtonEntry button) {

                hasButton = true;
            }
            NicoRepoList.Clear();
            foreach (var entry in UnFilteredNicoRepoList) {

                if (FilterEntry(entry)) {

                    NicoRepoList.Add(entry);
                }
            }
            if(hasButton) {

                NicoRepoList.Add(new NicoNicoNextButtonEntry());
            }
        }

        private bool FilterEntry(NicoNicoNicoRepoResultEntry item) {

            if (item.Muted) {

                return false;
            }

            switch (Filter) {
                case "すべて":
                    return true;
                case "動画投稿のみ":
                    if (item.Topic.EndsWith("video.upload")) {

                        return true;
                    }
                    return false;
                case "生放送開始のみ":
                    if (item.Topic.EndsWith("program.onairs")) {

                        return true;
                    }
                    return false;
                case "マイリスト登録のみ":
                    if (item.Topic.Contains("mylist.add")) {

                        return true;
                    }
                    return false;
                default:
                    return true;
            }
        }
    }

    public interface INicoRepo { }

    public class NicoNicoNextButtonEntry : INicoRepo { }

    public class NicoNicoNicoRepoResultEntry : INicoRepo {

        //ニコレポの固有ID
        public string Id { get; set; }

        //トピック そのニコレポがどんな内容か
        public string Topic { get; set; }

        //そのニコレポがディスパッチされた時間
        public DateTime CreatedAt { get; set; }

        //Visible
        public bool Visible { get; set; }

        //ミュート対象かどうか
        public bool Muted { get; set; }

        public bool Deletable { get; set; }

        //内容のURL
        public string ContentUrl { get; set; }

        public bool IsWatched { get; set; }

        public string SenderName { get; set; }

        public int SenderId { get; set; }

        public string SenderUrl { get; set; }

        public string SenderThumbnail { get; set; }

        //いろいろ弄ったタイトル
        public string ComputedTitle { get; set; }

        public string CommunityName { get; set; }

        public string CommunityId { get; set; }

        public string CommunityThumbnail { get; set; }

        public string CommunityUrl { get {
                return "http://com.nicovideo.jp/community/" + CommunityId;
            }
        }
    }

    public class NicoNicoNicoRepoVideoEntry : NicoNicoNicoRepoResultEntry, IWatchable {

        public string VideoId { get; set; }

        public string Status { get; set; }

        public string VideoThumbnail { get; set; }

        public string VideoTitle { get; set; }

        public string VideoWatchPageId { get; set; }
    }

    public class NicoNicoNicoRepoSeigaEntry : NicoNicoNicoRepoResultEntry {

        public string SeigaUrl { get; set; }

        public string SeigaId { get; set; }

        public string SeigaTitle { get; set; }

        public string SeigaThumbnail { get; set; }
    }

    public class NicoNicoNicoRepoGenericEntry : NicoNicoNicoRepoResultEntry {

        public string ContentId { get; set; }

        public string ContentTitle { get; set; }

        public string ContentThumbnail { get; set; }
    }

    public class NicoNicoNicoRepoNoContentEntry : NicoNicoNicoRepoResultEntry {
    }

    public class NicoNicoNicoRepoMangaEntry : NicoNicoNicoRepoResultEntry {

        public string MangaUrl { get; set; }

        public string MangaId { get; set; }

        public string MangaTitle { get; set; }

        public string MangaThumbnail { get; set; }

        public string MangaEpisodeUrl { get; set; }

        public string MangaEpisodeId { get; set; }

        public string MangaEpisodeThumbnail { get; set; }

        public string MangaEpisodeTitle { get; set; }
    }

    public class NicoNicoNicoRepoLiveEntry : NicoNicoNicoRepoResultEntry {

        //生放送ID
        public string ProgramId { get; set; }

        //生放送開始
        public DateTime ProgramBeginAt { get; set; }

        //有料放送かどうか
        public bool IsPayProgram { get; set; }

        public string ProgramThumbnail { get; set; }

        public string ProgramTitle { get; set; }
    }
}
