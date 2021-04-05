using Codeplex.Data;
using FastEnumUtility;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoNicoRepo : NotificationObject {

        private const string NicoRepoApiUrl = "https://public.api.nicovideo.jp/v1/timelines/nicorepo/last-1-month/my/pc/entries.json";
        private const string NicoRepoUserApiUrl = "https://public.api.nicovideo.jp/v1/timelines/nicorepo/last-1-month/users/{0}/pc/entries.json";

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

        private readonly List<NicoRepoEntry> UnFilteredNicoRepoList = new List<NicoRepoEntry>();

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

        private readonly NicoRepoType Type;

        private string Filter = "すべて";

        private string Next = "";

        public NicoNicoNicoRepo(NicoRepoType type) {

            Type = type;
            NicoRepoList = new ObservableSynchronizedCollection<INicoRepo>();
        }

        private readonly string UserId;

        public NicoNicoNicoRepo(string userId) {

            UserId = userId;
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

        public async Task<NicoRepoList> GetNicoRepoAsync(NicoRepoType type, NicoRepoFilter filter, string untilId = null) {

            var query = new GetRequestQuery(NicoRepoApiUrl);
            if (UserId != null) {

                query = new GetRequestQuery(string.Format(NicoRepoUserApiUrl, UserId));
            } else {

                if (type != NicoRepoType.All) {

                    query.AddQuery("list", type.GetLabel()!);
                }
            }

            if (filter != NicoRepoFilter.All) {

                query.AddQuery("object[type]", filter.GetLabel()!);
            }
            if (!string.IsNullOrEmpty(untilId)) {

                query.AddQuery("untilId", untilId);
            }

            var result = await App.ViewModelRoot.CurrentUser.Session.GetResponseAsync(query.TargetUrl).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode) {

                throw new Exception($"{result.StatusCode}");
            }
            var json = DynamicJson.Parse(await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            var ret = new NicoRepoList {
                HasNext = json.meta.hasNext,
                MinId = json.meta.minId() ? json.meta.minId : null,
                MaxId = json.meta.maxId() ? json.meta.maxId : null
            };

            var entries = new List<NicoRepoEntry>();
            foreach (var nicorepo in json.data) {

                if (nicorepo == null) {
                    continue;
                }
                entries.Add(new NicoRepoEntry {
                    Id = nicorepo.id,
                    Title = nicorepo.title,
                    UpdatedAt = DateTimeOffset.Parse(nicorepo.updated),
                    ActorUrl = nicorepo.actor.url,
                    ActorName = nicorepo.actor.name,
                    ActorIconUrl = nicorepo.actor.icon,
                    ObjectType = nicorepo.@object() ? nicorepo.@object.type : null,
                    ObjectUrl = nicorepo.@object() ? nicorepo.@object?.url : null,
                    ObjectName = nicorepo.@object() ? nicorepo.@object?.name : null,
                    ObjectImageUrl = nicorepo.@object() ? nicorepo.@object?.image : null,
                    MuteContext = nicorepo.muteContext() ? new NicoRepoMuteContext {
                        Task = nicorepo.muteContext.task,
                        Id = nicorepo.muteContext.sender.id,
                        IdType = nicorepo.muteContext.sender.idType,
                        Type = nicorepo.muteContext.sender.type,
                        Trigger = nicorepo.muteContext.trigger,
                    } : null
                });
            }
            ret.Entries = entries;

            return ret;
        }

        public async Task<string> GetNicoRepoAsync() {

            if (IsEnd) {

                return "";
            }
            try {

                if (NicoRepoList.LastOrDefault() is NicoNicoNextButtonEntry button) {

                    NicoRepoList.Remove(button);
                }
                var list = await GetNicoRepoAsync(Type, NicoRepoFilter.All, Next);
                
                foreach (var entry in list.Entries) {

                    UnFilteredNicoRepoList.Add(entry);
                    if (FilterEntry(entry)) {
                        NicoRepoList.Add(entry);
                    }
                }

                if (UnFilteredNicoRepoList.Count == 0) {

                    IsEmpty = true;
                    return "";
                }

                Next = UnFilteredNicoRepoList.Last().Id;

                if (list.HasNext) {

                    NicoRepoList.Add(new NicoNicoNextButtonEntry());
                } else {

                    IsEnd = true;
                }

                return "";
            } catch (RequestFailed e) {

                if (e.FailedType == FailedType.Failed) {

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
            if (hasButton) {

                NicoRepoList.Add(new NicoNicoNextButtonEntry());
            }
        }

        private bool FilterEntry(NicoRepoEntry item) {

            switch (Filter) {
                case "すべて":
                    return true;
                case "動画投稿のみ":
                    if (item.MuteContext?.Trigger.EndsWith("video_upload") ?? false) {

                        return true;
                    }
                    return false;
                case "生放送開始のみ":
                    if (item.MuteContext?.Trigger.EndsWith("program_onairs") ?? false) {

                        return true;
                    }
                    return false;
                case "マイリスト登録のみ":
                    if (item.MuteContext?.Trigger.Contains("mylist_add") ?? false) {

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

    public class NicoRepoList {
        /// <summary>
        /// さらに読み込むニコレポがあるかどうか
        /// </summary>
        public bool HasNext { get; set; }

        public string MaxId { get; set; }

        public string MinId { get; set; }

        /// <summary>
        /// ニコレポリスト
        /// </summary>
        public IEnumerable<NicoRepoEntry> Entries { get; set; }
    }
    /// <summary>
    /// ニコレポ情報
    /// </summary>
    public class NicoRepoEntry : INicoRepo {
        /// <summary>
        /// ニコレポのID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// ニコレポのタイトル
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// ニコレポが作られた日時
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }
        /// <summary>
        /// ニコレポが作成された人、チャンネルなどのURL
        /// </summary>
        public string ActorUrl { get; set; }
        /// <summary>
        /// ニコレポが作成された人、チャンネルなどの名前
        /// </summary>
        public string ActorName { get; set; }
        /// <summary>
        /// ニコレポが作成された人、チャンネルなどのサムネイルURL
        /// </summary>
        public string ActorIconUrl { get; set; }
        /// <summary>
        /// ニコレポの内容の種類
        /// videoやprogramなど
        /// </summary>
        public string ObjectType { get; set; }
        /// <summary>
        /// ニコレポの内容のURL
        /// </summary>
        public string ObjectUrl { get; set; }
        /// <summary>
        /// ニコレポの内容の名前
        /// 動画名など
        /// </summary>
        public string ObjectName { get; set; }
        /// <summary>
        /// ニコレポの内容のサムネイルURL
        /// 動画や生放送のサムネイルなど
        /// </summary>
        public string ObjectImageUrl { get; set; }
        /// <summary>
        /// ミュート
        /// </summary>
        public NicoRepoMuteContext MuteContext { get; set; }

        public void Open() {

            NicoNicoOpener.Open(ObjectUrl);
        }
    }
    /// <summary>
    /// ニコレポのミュートに使用する何か
    /// </summary>
    public class NicoRepoMuteContext {
        /// <summary>
        /// nicorepo固定かな？
        /// </summary>
        public string Task { get; set; }
        /// <summary>
        /// Idのタイプ userやchannelなど
        /// </summary>
        public string IdType { get; set; }
        /// <summary>
        /// Id IdTypeがuserの場合はuserのID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// IdTypeと同じ？
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// ニコレポのトリガー名
        /// </summary>
        public string Trigger { get; set; }
    }

    /// <summary>
    /// ニコレポフィルタ
    /// </summary>
    public enum NicoRepoFilter {
        /// <summary>
        /// すべて
        /// </summary>
        All,
        /// <summary>
        /// 動画投稿
        /// </summary>
        [Label("video")]
        VideoUpload,
        /// <summary>
        /// 生放送開始
        /// </summary>
        [Label("program")]
        ProgramOnAir,
        /// <summary>
        /// イラスト投稿
        /// </summary>
        [Label("image")]
        ImageAdd,
        /// <summary>
        /// 漫画投稿
        /// </summary>
        [Label("comicStory")]
        ComicStoryAdd,
        /// <summary>
        /// 記事投稿
        /// </summary>
        [Label("article")]
        ArticleAdd,
        /// <summary>
        /// ゲーム投稿
        /// </summary>
        [Label("game")]
        GameAdd
    }

    /// <summary>
    /// ニコレポ表示対象
    /// </summary>
    public enum NicoRepoType {
        /// <summary>
        /// すべて
        /// </summary>
        All,
        /// <summary>
        /// 自分
        /// </summary>
        [Label("self")]
        Self,
        /// <summary>
        /// ユーザー
        /// </summary>
        [Label("followingUser")]
        User,
        /// <summary>
        ///  チャンネル
        /// </summary>
        [Label("followingChannel")]
        Channel,
        /// <summary>
        /// コミュニティ
        /// </summary>
        [Label("followingCommunity")]
        Community,
        /// <summary>
        /// マイリスト
        /// </summary>
        [Label("followingMylist")]
        Mylist
    }
}
