using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoLiveApi : NotificationObject {

        /// <summary>
        /// 生放送ID
        /// </summary>
        public string LiveId { get; set; }

        /// <summary>
        /// 生放送タイトル
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 生放送説明文
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 生放送タグ
        /// </summary>
        public List<VideoTag> Tags { get; set; }

        /// <summary>
        /// ニコ生制御用WebSocket
        /// </summary>
        public string WebSocketUrl { get; set; }

        /// <summary>
        /// APIベース
        /// </summary>
        public string ApiBaseUrl { get; set; }

        /// <summary>
        /// BroadcastId dmcで使う
        /// </summary>
        public string BroadcastId { get; set; }

        /// <summary>
        /// HLS Url
        /// </summary>        
        public string HlsUrl { get; set; }


        #region Comment変更通知プロパティ
        private CommentRoom _Comment;

        public CommentRoom Comment {
            get { return _Comment; }
            set { 
                if (_Comment == value)
                    return;
                _Comment = value;
                RaisePropertyChanged();
            }
        }
        #endregion


    }

    public class CommentRoom : NotificationObject {

        /// <summary>
        /// 立ち見 アリーナ等
        /// </summary>
        public string RoomName { get; set; }

        /// <summary>
        /// スレッドID
        /// </summary>
        public string ThreadId { get; set; }

        /// <summary>
        /// コメント取得用WebSocket
        /// </summary>
        public string CommentSocketUrl { get; set; }

    }

}
