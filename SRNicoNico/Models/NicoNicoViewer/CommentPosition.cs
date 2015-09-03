using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using System.Windows;

namespace SRNicoNico.Models.NicoNicoViewer {

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
