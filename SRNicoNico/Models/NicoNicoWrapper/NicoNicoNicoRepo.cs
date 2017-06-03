using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SRNicoNico.ViewModels;
using SRNicoNico.Models.NicoNicoViewer;
using HtmlAgilityPack;
using System.Web;
using System.Text.RegularExpressions;
using Codeplex.Data;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoNicoRepo {

        private NicoRepoViewModel Owner;

        public NicoNicoNicoRepo(NicoRepoViewModel owner) {

            Owner = owner;
        }

        public async Task<NicoNicoNicoRepoResult> GetNicoRepoAsync(string type, string nextPage) {

            try {

                var query = new GetRequestQuery("http://www.nicovideo.jp/api/nicorepo/timeline/my/" + type);
                query.AddQuery("client_app", "pc_myrepo");
                query.AddQuery("_", UnixTime.ToUnixTime(DateTime.Now));
                if(nextPage != null) {

                    query.AddQuery("cursor", nextPage);
                }

                //ニコレポのhtmlを取得
                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(query.TargetUrl);

                dynamic json = DynamicJson.Parse(a);

                var ret = new NicoNicoNicoRepoResult();
                ret.Items = new List<NicoNicoNicoRepoResultEntry>();

                //ニコレポタイムラインを走査
                foreach(var entry in json.data) {

                    NicoNicoNicoRepoResultEntry item = null;

                    Console.WriteLine(entry.topic);

                    switch(entry.topic) {
                        case "live.channel.program.onairs":
                        case "live.channel.program.reserve": {

                                var sender = entry.senderChannel;
                                var program = entry.program;

                                item = new NicoNicoNicoRepoLiveEntry() {
                                    SenderId = (int)sender.id,
                                    SenderName = sender.name,
                                    SenderUrl = sender.url,
                                    SenderThumbnail = sender.thumbnailUrl,
                                    ProgramId = program.id,
                                    ProgramBeginAt = DateTime.Parse(program.beginAt),
                                    IsPayProgram = program.isPayProgram,
                                    ProgramThumbnail = program.thumbnailUrl,
                                    ProgramTitle = program.title
                                };

                                if (((string)entry.topic).EndsWith("onairs")) {

                                    item.ComputedTitle = string.Format("チャンネル {0} で生放送が開始されました。", item.SenderName);
                                } else {

                                    item.ComputedTitle = string.Format("チャンネル {0} で {1:yy年M月d日 h時 m分} に生放送が予約されました。", item.SenderName, DateTime.Parse(program.beginAt));
                                }
                                break;
                            }
                        case "live.user.program.onairs":
                        case "live.user.program.reserve": {

                                var sender = entry.senderNiconicoUser;
                                var program = entry.program;
                                var com = entry.community;

                                item = new NicoNicoNicoRepoLiveEntry() {
                                    SenderId = (int)sender.id,
                                    SenderName = sender.nickname,
                                    SenderUrl = "http://www.nicovideo.jp/user/" + sender.id,
                                    SenderThumbnail = sender.icons.tags.defaultValue.urls.s50x50,
                                    ProgramId = program.id,
                                    ProgramBeginAt = DateTime.Parse(program.beginAt),
                                    IsPayProgram = program.isPayProgram,
                                    ProgramThumbnail = program.thumbnailUrl,
                                    ProgramTitle = program.title,
                                    CommunityId = com.id,
                                    CommunityName = com.name,
                                    CommunityThumbnail = com.thumbnailUrl.small
                                };

                                if (((string)entry.topic).EndsWith("onairs")) {

                                    item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんがコミュニティ {1} で生放送が開始されました。", item.SenderName, item.CommunityName);
                                } else {

                                    item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんがコミュニティ {1} で {2:yy年M月d日 h時 m分} に生放送が予約しました。", item.SenderName, item.CommunityName, DateTime.Parse(program.beginAt));
                                }
                                break;
                            }
                        case "nicoseiga.user.illust.clip":
                        case "nicoseiga.user.illust.upload": {

                                var sender = entry.senderNiconicoUser;
                                var seiga = entry.illustImage;

                                item = new NicoNicoNicoRepoSeigaEntry() {
                                    SenderId = (int)sender.id,
                                    SenderName = sender.nickname,
                                    SenderUrl = "http://www.nicovideo.jp/user/" + sender.id,
                                    SenderThumbnail = sender.icons.tags.defaultValue.urls.s50x50,
                                    SeigaId = seiga.id,
                                    SeigaTitle = seiga.title,
                                    SeigaThumbnail = seiga.thumbnailUrl,
                                    SeigaUrl = seiga.urls.pcUrl
                                };

                                if (((string)entry.topic).EndsWith("clip")) {

                                    item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが イラストをクリップしました。", item.SenderName);
                                } else {

                                    item.ComputedTitle = string.Format("<a href=\"" + item.SenderUrl + "\">{0}</a> さんが <strong>イラストを投稿しました。</strong>", item.SenderName);
                                }
                                break;
                            }
                        default:
                            item = new NicoNicoNicoRepoResultEntry();
                            break;

                    }

                    item.Id = entry.id;
                    item.Topic = entry.topic;
                    item.CreatedAt = DateTime.Parse(entry.createdAt);
                    item.Visible = entry.isVisible;
                    item.Muted = entry.isMuted;

                    ret.Items.Add(item);
                }

                return ret;
            } catch(RequestFailed e) {

                if(e.FailedType == FailedType.Failed) {

                    Owner.Status = "ニコレポの取得に失敗しました";
                } else {

                    Owner.Status = "ニコレポの取得がタイムアウトになりました";
                }
                return null;
            }
        }
    }

    public class NicoNicoNicoRepoResult {

        //エントリリスト
        public List<NicoNicoNicoRepoResultEntry> Items { get; set; }

        //リストの終端まで来たかどうか
        public bool IsEnd { get; set; }

    }

    public class NicoNicoNicoRepoResultEntry : IWatchable {

        //ニコレポの固有ID なんでこんなもん振ったんだろうね
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
    }

    public class NicoNicoNicoRepoVideoEntry : NicoNicoNicoRepoResultEntry {

        public string VideoId { get; set; }

        //なんの解像度か全く分からんぞい
        public int VideoWidth { get; set; }
        public int VideoHeight { get; set; }

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
