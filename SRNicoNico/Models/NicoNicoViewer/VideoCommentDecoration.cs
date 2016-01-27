using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

namespace SRNicoNico.Models.NicoNicoViewer {
    public class VideoCommentDecoration : NotificationObject {

        //コメント位置
        public CommentPosition Position { get; set; }

        //コメントサイズ
        public CommentSize Size { get; set; }

        //コメントカラー
        public uint Color { get; set; }

        //コマンド
        public string RawCommand {

            get {

                return Position.ToString() + " " + Size.ToString() + Color.ToString();
            }
        }
    }

    public enum CommentPosition {

        Ue,
        Naka,
        Shita
    }

    public enum CommentSize {

        Big,
        Medium,
        Small
    }
}
