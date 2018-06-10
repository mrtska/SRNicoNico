using Codeplex.Data;
using System;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoViewer {
    public class UpdateChecker {

        //アップデート確認するURL
        private const string CheckUrl = "https://mrtska.net/niconicoviewer/latest";

        public static async Task<bool> IsUpdateAvailable() {

            try {

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(CheckUrl);

                var json = DynamicJson.Parse(a);
                return App.ViewModelRoot.CurrentVersion < json.version;
            } catch(Exception) {

                return false;
            }
        }
    }
}
