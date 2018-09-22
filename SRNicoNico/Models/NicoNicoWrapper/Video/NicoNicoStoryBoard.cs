using Codeplex.Data;
using SRNicoNico.Models.NicoNicoViewer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoStoryBoard {

        //サムネイル一つの横幅
        public int Width { get; set; }

        //サムネイル一つの縦幅
        public int Height { get; set; }

        //サムネイルの間隔
        public int Interval { get; set; }

        public Dictionary<int, Bitmap> Bitmap = new Dictionary<int, Bitmap>();

        private string ApiUrl;
        private dynamic Payload;

        public NicoNicoStoryBoard(dynamic session_api) {

            Initialize(session_api);
        }
        private void Initialize(dynamic session_api) {

            dynamic json = new DynamicJson();

            var api = session_api.urls[0];

            var query = new GetRequestQuery(api.url);
            query.AddQuery("_format", "json");

            ApiUrl = query.TargetUrl;

            json.session = new {
                session_api.recipe_id,
                session_api.content_id,
                content_type = "video",
                content_src_id_sets = new[] {
                    new {
                        content_src_ids = (object[]) session_api.videos
                    }
                },
                timing_constraint = "unlimited",
                keep_method = new {
                    heartbeat = new {
                        lifetime = session_api.heartbeat_lifetime
                    }
                },
                protocol = new {
                    name = "http",
                    parameters = new {
                        http_parameters = new {
                            parameters = new {
                                storyboard_download_parameters = new {
                                    use_well_known_port = api.is_well_known_port ? "yes" : "no",
                                    use_ssl = api.is_ssl ? "yes" : "no"
                                }
                            }
                        }
                    }
                },
                content_uri = "",
                session_operation_auth = new {
                    session_operation_auth_by_signature = new {
                        session_api.token,
                        session_api.signature
                    }
                },
                content_auth = new {
                    auth_type = session_api.auth_types.storyboard,
                    session_api.content_key_timeout,
                    service_id = "nicovideo",
                    session_api.service_user_id
                },
                client_info = new {
                    session_api.player_id
                },
                session_api.priority
            };
            Payload = json;
        }

        public async Task GetStoryBoardAsync() {

            try {
                var request = new HttpRequestMessage(HttpMethod.Post, new Uri(ApiUrl)) {
                    Content = new StringContent(Payload.ToString())
                };
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);
                var doc = DynamicJson.Parse(a);

                var res = DynamicJson.Parse(await App.ViewModelRoot.CurrentUser.Session.GetAsync(doc.data.session.content_uri));
                var storyboard = res.data.storyboards[0];

                Width = (int)storyboard.thumbnail_width;
                Height = (int)storyboard.thumbnail_height;
                Interval = (int)(storyboard.interval / 1000); // ミリ秒を秒へ

                int bitmapindex = 0;
                foreach(var image in storyboard.images) {

                    using(var picture = await App.ViewModelRoot.CurrentUser.Session.GetResponseAsync(image.uri)) {

                        var bitmap = new Bitmap(await picture.Content.ReadAsStreamAsync());

                        for (int j = 0; j < (int)storyboard.columns; j++) {
                            for (int k = 0; k < (int)storyboard.rows; k++) {

                                var rect = new Rectangle(Width * k, Height * j, Width, Height);

                                Bitmap[bitmapindex] = bitmap.Clone(rect, bitmap.PixelFormat);
                                bitmapindex += Interval;
                            }
                        }
                    }
                }
            } catch(Exception) {
            }
        }
    }
}
