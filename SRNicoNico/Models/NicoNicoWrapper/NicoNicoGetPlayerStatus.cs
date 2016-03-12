using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using Codeplex.Data;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoGetPlayerStatus : NotificationObject {

        //生放送ID
        public string Id { get; set; }

        //生放送タイトル
        public string Title { get; set; }

        //生放送説明文（最初の部分のみ）
        public string Description { get; set; }

        //communityとかofficialとかchannelとか
        public string ProviderType { get; set; }

        //コミュニティ放送ならコミュニティID チャンネル放送ならチャンネルID 公式はブランク
        public string DefaultCommunity { get; set; }

        //自分が生放送主か否か
        public bool IsOwner { get; set; }

        //放送主のID 公式放送の時はよく分からないIDが返される
        public string OwnerId { get; set; }

        //コミュニティ放送のみ
        public string OwnerName { get; set; }

        //放送作成時間 放送開始時間 放送終了時間
        public string BaseTime { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        //APIを叩いた時の視聴回数
        public string WatchCount { get; set; }

        //APIを叩いた時のコメント数
        public string CommentCount { get; set; }

        //タイムシフト試聴か否か
        public bool Archive { get; set; }

        //生放送サムネイル チャンネル/コミュニティのサムネイルとは限らないかも？
        public string ThumbNailUrl { get; set; }

        //座席位置 アリーナとかそういうやつ
        public string RoomLabel { get; set; }

        //座席番号
        public string SeetNumber { get; set; }

        //生放送URL
        public string RtmpUrl { get; set; }

        //生放送チケット
        public string Ticket { get; set; }

        //コメントサーバーURL
        public string MesseageServerUrl { get; set; }
        public string MesseageServerPort { get; set; }
        public string CommentServerUrl {
            get {
                return MesseageServerUrl + ":" + MesseageServerPort;
            }
        }
        //生放送スレッドID
        public string ThreadId { get; set; }
        
        //オンエア時のみ
        public IList<Contents> ContentsList { get; set; }

        //タイムシフト時のみ
        public IList<QueSheet> QueSheet { get; set; }

        public string ToJson() {
            return DynamicJson.Serialize(this);
        }
    }

    public class Contents {

        //コンテンツID main か sub
        public string Id { get; set; }

        public bool DisableVideo { get; set; }

        public bool DisableAudio { get; set; }

        //開始時刻 UnixTime
        public string StartTime { get; set; }

        public string Content { get; set; }
    }

    //タイムシフトのタイムテーブル的な
    public class QueSheet {

        //開始位置
        public string Vpos { get; set; }

        //属性
        public string Mail { get; set; }

        //コマンド
        public string Content { get; set; }

        //そのコマンドを叩き終わったかどうか
        public bool Done { get; set; }
    }
}
