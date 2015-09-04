using System;

using Livet;

namespace SRNicoNico.Models.NicoNicoViewer {

    [Obsolete]
    public class CommentPosition : NotificationObject {


        public EnumCommentPosition EnumPos { get; set; }

        public double XPos { get; set; }

        public double YPos { get; set; }


    }

    public enum EnumCommentPosition {

        Ue,
        Shita,
        Naka
    }
}
