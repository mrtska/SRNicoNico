using Codeplex.Data;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoViewer {
    public class UpdateChecker {

        //アップデート確認するURL
        private const string CheckUrl = "https://mrtska.net/niconicoviewer/latest";

        public static async Task<bool> IsUpdateAvailable() {

            using (var httpClient = new HttpClient()) {

                try {

                    var a = await httpClient.GetStringAsync(CheckUrl);

                    var json = DynamicJson.Parse(a);
                    return App.ViewModelRoot.CurrentVersion < json.version;
                } catch (Exception) {

                    return false;
                }
            }


        }
    }
}
