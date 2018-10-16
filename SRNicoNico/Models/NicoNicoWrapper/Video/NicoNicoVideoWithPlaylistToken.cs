using Codeplex.Data;
using SRNicoNico.Models.NicoNicoViewer;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoVideoWithPlaylistToken {

        private readonly string VideoId;

        private readonly string PlaylistToken;

        public NicoNicoVideoWithPlaylistToken(string videoId, string playlistToken) {

            VideoId = videoId;
            PlaylistToken = playlistToken;
        }

        public async Task<string> Initialize() {

            var query = new GetRequestQuery("https://www.nicovideo.jp/watch/" + VideoId);
            query.AddQuery("mode", "pc_html5");
            query.AddQuery("eco", 0);
            query.AddQuery("playlist_token", PlaylistToken);
            query.AddQuery("watch_harmful", 2);
            query.AddQuery("continue_watching", 1);

            var json = DynamicJson.Parse(await App.ViewModelRoot.CurrentUser.Session.GetAsync(query.TargetUrl));

            if(json.status != "ok") {

                return "";
            }

            return json.ToString();
        }
    }
}
