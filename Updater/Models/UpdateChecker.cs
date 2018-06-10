using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using System.Net;

using Codeplex.Data;

namespace Updater.Models {
    public class UpdateChecker {

        //アップデート確認するURL
        private const string CheckUrl = "https://mrtska.net/niconicoviewer/latest";

        public static async Task<string> GetLatestBinaryUrl(WebClient wc) {

            try {

                var a = await wc.DownloadStringTaskAsync(CheckUrl);

                var json = DynamicJson.Parse(a);

                return "https://mrtska.net/niconicoviewer/download/" + json.version;
            } catch (Exception) {

                return null;
            }
        }
    }
}
