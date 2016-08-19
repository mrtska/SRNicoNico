using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using Codeplex.Data;

using SRNicoNico.Models.NicoNicoWrapper;
using System.Xml;

namespace SRNicoNico.Models.NicoNicoViewer {
    public class UpdateCheck : NotificationObject {


        //アップデート確認するURL
#if DEBUG
        private static string CheckUrl = "https://mrtska.net/niconicowrapper/update";
#else
        private static string CheckUrl = "http://download.mrtska.net/DownloadCounter/Download?file=NicoNicoViewer/update";
#endif

        //アップデート確認 trueならアップデートあり
        //urlはダウンロードできるURL
        public static bool IsUpdateAvailable(double cur, ref string url) {

            try {

                var a = NicoNicoWrapperMain.Session.GetAsync(CheckUrl).Result;

                if(a.Length == 0) {

                    return false;
                }
                var json = DynamicJson.Parse(a);

                //現在のバージョンとダウンロードしてきたjsonのバージョンを比較してjsonのほうが高かったらアップデートがあるってこと
                if(cur < json.version) {

                    url = json.url;
                    return true;
                }


                return false;
                
            //例外処理でたとえ私のサーバーが落ちてもViewerには何も影響が出ないように
            } catch(Exception ex) when (ex is XmlException || ex is RequestTimeout) {

                return false;
            }
        }
    }
}
