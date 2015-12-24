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

        //ニコニコのURLが何を指しているかを返す
        public static NicoNicoUrlType GetType(string url) {

            if(url.StartsWith("http://www.nicovideo.jp/watch/")) {

                return NicoNicoUrlType.Video;
            }
            if(url.StartsWith("http://www.nicovideo.jp/user/")) {

                return NicoNicoUrlType.User;
            }
            if(url.StartsWith("http://www.nicovideo.jp/mylist/")) {

                return NicoNicoUrlType.Mylist;
            }

            return NicoNicoUrlType.Other;
        }

    }
    
    public enum NicoNicoUrlType {

        Video,
        Live,
        Mylist,
        User,
        Other

    }
}
