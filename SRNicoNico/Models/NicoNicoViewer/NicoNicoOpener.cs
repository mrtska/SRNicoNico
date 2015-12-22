using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using SRNicoNico.ViewModels;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoViewer {
    public class NicoNicoOpener : NotificationObject {


        //URLから適当なViewを開く
        public static void Open(string url) {

            Task.Run(() => {

                if(url.StartsWith("http://www.nicovideo.jp/watch/")) {

                    new VideoViewModel(url);
                } else if(url.StartsWith("http://www.nicovideo.jp/user/")) {

                    new UserViewModel(url);
                } else if(url.StartsWith("http://www.nicovideo.jp/mylist/")) {

                    new PublicMylistViewModel(url);
                } else {

                    System.Diagnostics.Process.Start(url);
                }
            });
        }
    }
}
