using System;

using Livet;
using System.Windows.Media;

namespace SRNicoNico.Models.NicoNicoViewer {

    //コメントの装飾
    [Obsolete]
    public class CommentDecoration : NotificationObject {
        
        public Brush Color { get; set; }

        public double FontSize { get; set; }
    }
}
