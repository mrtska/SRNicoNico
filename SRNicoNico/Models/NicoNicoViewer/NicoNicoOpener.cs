using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using SRNicoNico.ViewModels;

namespace SRNicoNico.Models.NicoNicoViewer {
    public class NicoNicoOpener : NotificationObject {


        public static void Open(string url) {

            if(url.StartsWith("http://www.nicovideo.jp/watch/")) {

                new VideoViewModel(url);
            }

        }

    }
}
