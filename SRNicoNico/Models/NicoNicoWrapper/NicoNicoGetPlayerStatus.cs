using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

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

        
    }
}
