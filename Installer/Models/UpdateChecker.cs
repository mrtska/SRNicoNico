using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using System.Net;

using Codeplex.Data;

namespace Installer.Models {
    public class UpdateChecker {



        //アップデート確認するURL
#if DEBUG
        private static string CheckUrl = "https://mrtska.net/niconicowrapper/debugupdate";
#else
        private static string CheckUrl = "http://download.mrtska.net/DownloadCounter/Download?file=NicoNicoViewer/releaseupdate";
#endif

        public static async Task<string> GetLatestBinaryUrl(WebClient wc) {

            try {

                var a = await wc.DownloadStringTaskAsync(CheckUrl);

                var json = DynamicJson.Parse(a);

                return json.url;
            } catch(Exception) {

                return null;
            }

        }



        public static bool UpdateAvailable() {



            return true;
        }




    }
}
