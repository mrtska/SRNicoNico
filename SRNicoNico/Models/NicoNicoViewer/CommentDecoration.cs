using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using System.Windows.Media;

namespace SRNicoNico.Models.NicoNicoViewer {

    //コメントの装飾
    public class CommentDecoration : NotificationObject {
        
        public Brush Color { get; set; }

        public double FontSize { get; set; }
    }
}
