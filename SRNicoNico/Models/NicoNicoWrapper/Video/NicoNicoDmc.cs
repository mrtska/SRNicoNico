using Codeplex.Data;
using SRNicoNico.Models.NicoNicoViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {

    public class NicoNicoDmc {

        /// <summary>
        /// すべてdynamicで解決する
        /// </summary>
        private readonly dynamic FirstSessionApi;
        private dynamic SessionApi;
        //
        public List<NicoNicoDmcVideoQuality> Videos { get; set; }

        //オーディオコーデック
        public List<NicoNicoDmcAudioQuality> Audios { get; set; }

        public int HeartbeatLifeTime { get; private set; }

        public NicoNicoDmc(dynamic session_api) {

            FirstSessionApi = session_api;
            HeartbeatLifeTime = (int) session_api.heartbeat_lifetime;

            Videos = new List<NicoNicoDmcVideoQuality>();
            foreach (var video in session_api.videos) {

                Videos.Add(new NicoNicoDmcVideoQuality(video));
            }

            Audios = new List<NicoNicoDmcAudioQuality>();
            foreach (var audio in session_api.audios) {

                Audios.Add(new NicoNicoDmcAudioQuality(audio));
            }
        }

        public async Task<string> CreateAsync(NicoNicoDmcVideoQuality videoQuality, NicoNicoDmcAudioQuality audioQuality) {

            dynamic json = new DynamicJson();

            json.session = new {
                FirstSessionApi.recipe_id,
                FirstSessionApi.content_id,
                content_type = "movie",
                content_src_id_sets = new List<dynamic>(),
                timing_constraint = "unlimited",
                keep_method = new {
                    heartbeat = new {
                        lifetime = HeartbeatLifeTime
                    }
                },
                protocol = new {
                    name = "http",
                    parameters = new {
                        http_parameters = new {
                            parameters = new {
                                http_output_download_parameters = new {}
                            }
                        }
                    }
                },
                content_uri = "",
                session_operation_auth = new {
                    session_operation_auth_by_signature = new {
                        token = SessionApi?.session.session_operation_auth.session_operation_auth_by_signature.token ?? FirstSessionApi.token,
                        signature = SessionApi?.session.session_operation_auth.session_operation_auth_by_signature.signature ?? FirstSessionApi.signature
                    }
                },
                content_auth = new {
                    auth_type = FirstSessionApi.auth_types.http,
                    FirstSessionApi.content_key_timeout,
                    service_id = "nicovideo",
                    FirstSessionApi.service_user_id
                },
                client_info = new {
                    player_id = SessionApi?.session.client_info.player_id ?? FirstSessionApi.player_id
                },
                FirstSessionApi.priority
            };
            var mux = new {
                video_src_ids = new List<string>(),
                audio_src_ids = new List<string>()
            };
            mux.video_src_ids.Add(videoQuality.Raw);
            mux.audio_src_ids.Add(audioQuality.Raw);
            json.session.content_src_id_sets = new[] { new { content_src_ids = new[] { new { src_id_to_mux = mux } } } };

            try {
                var str = json.ToString();

                dynamic session = FirstSessionApi.urls[0];

                var query = new GetRequestQuery(session.url);
                query.AddQuery("_format", "json");

                var request = new HttpRequestMessage(HttpMethod.Post, new Uri(query.TargetUrl)) {
                    Content = new StringContent(json.ToString())
                };
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                var doc = DynamicJson.Parse(a);
                SessionApi = doc.data;

                return SessionApi.session.content_uri;
            } catch(RequestFailed) {

                return null;
            }
        }

        public async Task DeleteAsync() {

            try {

                var query = new GetRequestQuery(FirstSessionApi.urls[0].url + "/" + SessionApi.session.id);
                query.AddQuery("_format", "json");
                query.AddQuery("_method", "DELETE");

                var request = new HttpRequestMessage(HttpMethod.Post, new Uri(query.TargetUrl)) {
                    Content = new StringContent(SessionApi.ToString())
                };
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                var doc = DynamicJson.Parse(a);
                ;

            } catch (RequestFailed) {

            }

        }

        public async Task HeartbeatAsync() {

            try {

                var query = new GetRequestQuery(FirstSessionApi.urls[0].url + "/" + SessionApi.session.id);
                query.AddQuery("_format", "json");
                query.AddQuery("_method", "PUT");

                var request = new HttpRequestMessage(HttpMethod.Post, new Uri(query.TargetUrl)) {
                    Content = new StringContent(SessionApi.ToString())
                };
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                if (!NicoNicoUtil.IsValidJson(a)) {

                    return;
                }
                SessionApi = DynamicJson.Parse(a).data;

                return;
            } catch (RequestFailed) {

                return;
            }
        }
    }

    public class NicoNicoDmcVideoQuality {

        public string Raw { get; private set; }

        public string Codec { get; private set; }

        public string Bitrate { get; private set; }

        public string Resolution { get; private set; }

        public NicoNicoDmcVideoQuality(string raw) {

            Raw = raw;
            var str = raw.Split('_');
            Codec = str[1];
            Bitrate = str[2];
            Resolution = str[3];
        }
    }
    public class NicoNicoDmcAudioQuality {
        public string Raw { get; private set; }

        public string Codec { get; private set; }

        public string Bitrate { get; private set; }

        public NicoNicoDmcAudioQuality(string raw) {

            Raw = raw;
            var str = raw.Split('_');
            Codec = str[1];
            Bitrate = str[2];
        }
    }
}
