using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using System.Net;

using Codeplex.Data;

namespace SRNicoNico.Models.NicoNicoViewer {
    public class UpdateChecker {

        //アップデート確認するURL
#if DEBUG
        private const string CheckUrl = "https://mrtska.net/niconicowrapper/debugupdate";
#else
        private const string CheckUrl = "https://mrtska.net/niconicoviewer/latest";
#endif

        public static async Task<string> GetUpdaterBinaryUrl() {

            try {

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(CheckUrl);

                var json = DynamicJson.Parse(a);

                return json.updater;
            } catch(Exception) {

                return "";
            }
        }

        public static async Task<bool> IsUpdateAvailable() {

            try {

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(CheckUrl);

                var json = DynamicJson.Parse(a);
                return  App.ViewModelRoot.CurrentVersion < json.version;
            } catch(Exception) {

                return false;
            }
        }
    }
}
