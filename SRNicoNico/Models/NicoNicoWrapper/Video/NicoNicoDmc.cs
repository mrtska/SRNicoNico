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
        /// 
        private readonly dynamic DmcInfo;
        private dynamic SessionApi;
        //
        public List<NicoNicoDmcVideoQuality> Videos { get; set; }

        //オーディオコーデック
        public List<NicoNicoDmcAudioQuality> Audios { get; set; }

        private bool IsEncrypted = false;
        private dynamic Encryption;

        /// <summary>
        /// トラッキングID
        /// </summary>
        public string TrackingId { get; private set; }

        public NicoNicoDmc(dynamic dmcInfo) {

            DmcInfo = dmcInfo;
            Initialize();
        }

        private void Initialize() {

            if (DmcInfo.encryption()) {

                IsEncrypted = true;
                Encryption = DmcInfo.encryption;
            }

            Videos = new List<NicoNicoDmcVideoQuality>();
            foreach (var video in DmcInfo.quality.videos) {

                if (video.available) {

                    Videos.Add(new NicoNicoDmcVideoQuality(video.id, (int)video.bitrate));
                }
            }

            Audios = new List<NicoNicoDmcAudioQuality>();
            foreach (var audio in DmcInfo.quality.audios) {

                if (audio.available) {

                    Audios.Add(new NicoNicoDmcAudioQuality(audio.id, (int)audio.bitrate));
                }
            }
        }

        public int GetHeartBeatLifeTime() {

            if(DmcInfo == null) {

                throw new InvalidOperationException("初期化処理が走っていない");
            }
            return (int)DmcInfo.session_api.heartbeat_lifetime;
        }


        public async Task<string> CreateAsync(NicoNicoDmcVideoQuality videoQuality, NicoNicoDmcAudioQuality audioQuality) {

            dynamic json = new DynamicJson();

            var availableProtocols = DmcInfo.session_api.protocols;
            var isOnlyHls = true;
            foreach (var protocol in availableProtocols) {

                if(protocol == "http") {

                    isOnlyHls = false;
                }
            }

            json.session = new {
                DmcInfo.session_api.recipe_id,
                DmcInfo.session_api.content_id,
                content_type = "movie",
                content_src_id_sets = new List<dynamic>(),
                timing_constraint = "unlimited",
                keep_method = new {
                    heartbeat = new {
                        lifetime = (int)DmcInfo.session_api.heartbeat_lifetime
                    }
                },
                protocol = new {
                    name = "http",
                    parameters = new {
                        http_parameters = new {
                            parameters =  isOnlyHls ? (object) new {
                                hls_parameters = new {
                                    use_well_known_port = "yes",
                                    use_ssl = "yes",
                                    transfer_preset = "standard2",
                                    segment_duration = 5000
                                }
                            } : new {
                                http_output_download_parameters = new { }
                            }
                        }
                    }
                },
                content_uri = "",
                session_operation_auth = new {
                    session_operation_auth_by_signature = new {
                        token = SessionApi?.session.session_operation_auth.session_operation_auth_by_signature.token ?? DmcInfo.session_api.token,
                        signature = SessionApi?.session.session_operation_auth.session_operation_auth_by_signature.signature ?? DmcInfo.session_api.signature
                    }
                },
                content_auth = new {
                    auth_type = DmcInfo.session_api.auth_types.hls,
                    DmcInfo.session_api.content_key_timeout,
                    service_id = "nicovideo",
                    DmcInfo.session_api.service_user_id
                },
                client_info = new {
                    player_id = SessionApi?.session.client_info.player_id ?? DmcInfo.session_api.player_id
                },
                DmcInfo.session_api.priority
            };

            if(IsEncrypted) {
                json.session.protocol.parameters.http_parameters.parameters.hls_parameters.encryption = new {
                    hls_encryption_v1 = new {
                        Encryption.hls_encryption_v1.encrypted_key,
                        Encryption.hls_encryption_v1.key_uri
                    }
                };
            }

            var mux = new {
                video_src_ids = new List<string>(),
                audio_src_ids = new List<string>()
            };
            mux.video_src_ids.Add(videoQuality.Raw);
            mux.audio_src_ids.Add(audioQuality.Raw);
            json.session.content_src_id_sets = new[] { new { content_src_ids = new[] { new { src_id_to_mux = mux } } } };

            try {
                var str = json.ToString();

                dynamic session = DmcInfo.session_api.urls[0];

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

                var query = new GetRequestQuery(DmcInfo.session_api.urls[0].url + "/" + SessionApi.session.id);
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

                var query = new GetRequestQuery(DmcInfo.session_api.urls[0].url + "/" + SessionApi.session.id);
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

                var json = DynamicJson.Parse(a);

                if(json.data()) {

                    SessionApi = json.data;
                }

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

        public NicoNicoDmcVideoQuality(string id, int bitrate) {

            Raw = id;
            var str = id.Split('_');

            Codec = str[1];
            Resolution = str[str.Length - 1];

            if(bitrate < 1000000) {

                Bitrate = (bitrate / 1000) + "Kbps";
            } else {

                Bitrate = (bitrate / 1000 / 1000) + "Mbps";
            }
        }
    }
    public class NicoNicoDmcAudioQuality {
        public string Raw { get; private set; }

        public string Codec { get; private set; }

        public string Bitrate { get; private set; }

        public NicoNicoDmcAudioQuality(string raw, int bitrate) {

            Raw = raw;
            var str = raw.Split('_');
            Codec = str[1];
            Bitrate = bitrate / 1000 + "Kbps";
        }
    }
}
