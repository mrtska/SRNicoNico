using System;
using System.Windows.Controls;

using Livet;
using System.Windows.Media.Animation;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Models.NicoNicoViewer {

    [Obsolete]
    public class CommentEntry : NotificationObject {


        public TextBlock Text { get; set; }

        public CommentPosition Pos { get; set; }

        public NicoNicoCommentEntry Raw { get; set; }

        public Storyboard Story { get; set; }

        public DoubleAnimation Anime { get; set; }

        public CommentDecoration Decoration { get; set; }

        public CommentEntry(NicoNicoCommentEntry entry) {

            Raw = entry;
            Pos = new CommentPosition();
            Decoration = new CommentDecoration();
        }


        public override string ToString() {
            return "Enum:" + Pos.EnumPos + " entryVpos:" + Raw.Vpos + " entryVend:" + Raw.Vend + " xpos:" + Pos.XPos + " ypos:" + Pos.YPos + "content:" + Text.Text;
        }
    }
}
