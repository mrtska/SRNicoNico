using Codeplex.Data;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Installer.Models {
    public class UpdateChecker {

        //アップデート確認するURL
        private const string CheckUrl = "https://mrtska.net/niconicoviewer/latest";

        public static async Task<string> GetLatestBinaryUrl(WebClient wc) {

            try {

                var a = await wc.DownloadStringTaskAsync(CheckUrl);

                var json = DynamicJson.Parse(a);

                return "https://mrtska.net/niconicoviewer/download/" + json.version;
            } catch(Exception) {

                return null;
            }
        }
    }
}
