using Livet;
using System;
using System.Collections.Generic;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoWatchApi : NotificationObject {

        /// <summary>
        /// WatchAPI Json
        /// </summary>
        public dynamic RootJson { get; set; }

        /// <summary>
        /// 動画ID
        /// </summary>
        public string VideoId { get; set; }

        /// <summary>
        /// 動画タイトル
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 動画説明文 html有り
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 動画の長さ 秒
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// 動画開始位置
        /// </summary>
        public int InitialPlaybackPosition { get; set; }

        /// <summary>
        /// サムネイル
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// 動画投稿時間
        /// </summary>
        public DateTime PostedAt { get; set; }

        /// <summary>
        /// 動画タグ
        /// </summary>
        public List<VideoTag> Tags { get; set; }

        /// <summary>
        /// 再生数
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// コメント数
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// マイリスト数
        /// </summary>
        public int MylistCount { get; set; }

        /// <summary>
        /// 過去最高順位
        /// </summary>
        public int? HighestRank { get; set; }

        /// <summary>
        /// 前日総合順位
        /// </summary>
        public int? YesterdayRank { get; set; }

        public ViewerInfo ViewerInfo { get; set; }

        /// <summary>
        /// 投稿者情報
        /// </summary>
        public UploaderInfo UploaderInfo { get; set; }

        /// <summary>
        /// DMCハートビートが必要か
        /// </summary>
        public bool DmcHeartbeatRequired { get; set; }

        /// <summary>
        ///動画用DMCセッション
        /// </summary>
        public NicoNicoDmc DmcInfo { get; set; }

        /// <summary>
        /// コメントに関する情報
        /// </summary>
        public ThreadInfo ThreadInfo { get; set; }

        /// <summary>
        /// 動画URL
        /// </summary>
        public string VideoUrl { get; set; }

        /// <summary>
        /// ストーリーボード
        /// </summary>
        #region StoryBoard変更通知プロパティ
        private NicoNicoStoryBoard _StoryBoard;

        public NicoNicoStoryBoard StoryBoard {
            get { return _StoryBoard; }
            set { 
                if (_StoryBoard == value)
                    return;
                _StoryBoard = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// CSRF対策トークン
        /// </summary>
        public string CsrfToken { get; set; }

        /// <summary>
        /// プレイリストトークン
        /// 動画情報だけをリフレッシュしたりするのに使う
        /// なぜプレイリストという名前がついているのかは不明
        /// </summary>
        public string PlaylistToken { get; set; }

        public string UserKey { get; set; }

        /// <summary>
        /// 要課金
        /// </summary>
        public bool IsNeedPayment { get; set; }
    }

    public class UploaderInfo {

        /// <summary>
        /// 投稿者ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 投稿者ページ
        /// </summary>
        public string UploaderUrl { get; set; }

        /// <summary>
        /// 投稿者名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 投稿者サムネイル
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// 投稿者がチャンネルかどうか
        /// </summary>
        public bool IsChannel { get; set; }

        /// <summary>
        /// フォローしているかどうか
        /// </summary>
        public bool Followed { get; set; }
    }

    /// <summary>
    /// 見ている人の情報
    /// </summary>
    public class ViewerInfo {

        public string Id { get; set; }

        public string Nickname { get; set; }

        public bool IsPremium { get; set; }

        public string NicosId { get; set; }
    }


    /// <summary>
    /// コメント情報
    /// </summary>
    public class ThreadInfo {

        public string ServerUrl;

        public List<CommentComposite> Composites;

    }
    public class CommentComposite {

        public string Id;   // スレッドID
        public bool Fork; // 投稿者コメント
        public bool IsActive;
        public int PostKeyStatus;   // 0ならコメント可能
        public bool IsDefaultPostTarget;
        public bool IsThreadKeyRequired;    // コメントの取得ににスレッドKeyが必要かどうか
        public bool IsLeafRequired;    // リーフ 葉っぱ
        public string Label;
        public bool IsOwnerThread; // 投稿者コメント
        public bool HasNicoscript; // ニコスクリプトがあるか
    }


    public class VideoTag {

        /// <summary>
        /// タグID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// タグの名前
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 大百科のURL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// カテゴリタグかどうか
        /// </summary>
        public bool IsCategory { get; set; }

        /// <summary>
        /// カテゴリタグ候補？
        /// </summary>
        public bool IsCategoryCandidate { get; set; }
        /// <summary>
        /// タグロックされているかどうか
        /// </summary>
        public bool IsLocked { get; set; }
        
        /// <summary>
        /// ニコニコ大百科に存在するか
        /// </summary>
        public bool IsDictionaryExists { get; set; }

        public void Search() {

            App.ViewModelRoot.Search.SearchType = SearchType.Tag;
            App.ViewModelRoot.Search.Search(Name);
            App.ViewModelRoot.MainContent.SelectedTab = App.ViewModelRoot.Search;
        }

    }
}
