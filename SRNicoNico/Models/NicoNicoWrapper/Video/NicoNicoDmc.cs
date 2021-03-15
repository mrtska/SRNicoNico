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

            DmcInfo = dmcInfo.delivery;
            Initialize();
        }

        private void Initialize() {

            if (DmcInfo.encryption != null) {

                IsEncrypted = true;
                Encryption = DmcInfo.encryption;
            }

            Videos = new List<NicoNicoDmcVideoQuality>();
            foreach (var video in DmcInfo.movie.videos) {

                if (video.isAvailable) {

                    Videos.Add(new NicoNicoDmcVideoQuality(video.id, (int)video.metadata.bitrate));
                }
            }

            Audios = new List<NicoNicoDmcAudioQuality>();
            foreach (var audio in DmcInfo.movie.audios) {

                if (audio.isAvailable) {

                    Audios.Add(new NicoNicoDmcAudioQuality(audio.id, (int)audio.metadata.bitrate));
                }
            }
        }

        public int GetHeartBeatLifeTime() {

            var session = DmcInfo.movie.session;
            if (DmcInfo == null) {

                throw new InvalidOperationException("初期化処理が走っていない");
            }
            return (int)session.heartbeatLifetime;
        }


        public async Task<string> CreateAsync(NicoNicoDmcVideoQuality videoQuality, NicoNicoDmcAudioQuality audioQuality) {

            dynamic json = new DynamicJson();

            var session = DmcInfo.movie.session;
            var availableProtocols = session.protocols;
            var isOnlyHls = true;
            foreach (var protocol in availableProtocols) {

                if (protocol == "http") {

                    isOnlyHls = false;
                }
            }

            json.session = new {
                recipe_id = session.recipeId,
                content_id = session.contentId,
                content_type = "movie",
                content_src_id_sets = new List<dynamic>(),
                timing_constraint = "unlimited",
                keep_method = new {
                    heartbeat = new {
                        lifetime = (int)session.heartbeatLifetime
                    }
                },
                protocol = new {
                    name = "http",
                    parameters = new {
                        http_parameters = new {
                            parameters = isOnlyHls ? (object)new {
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
                        token = session.token,
                        signature = session.signature
                    }
                },
                content_auth = new {
                    auth_type = session.authTypes.hls,
                    content_key_timeout = session.contentKeyTimeout,
                    service_id = "nicovideo",
                    service_user_id = session.serviceUserId
                },
                client_info = new {
                    player_id = session.playerId
                },
                session.priority
            };

            if (IsEncrypted) {
                json.session.protocol.parameters.http_parameters.parameters.hls_parameters.encryption = new {
                    hls_encryption_v1 = new {
                        encrypted_key = Encryption.encryptedKey,
                        key_uri = Encryption.keyUri
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

                var query = new GetRequestQuery(session.urls[0].url);
                query.AddQuery("_format", "json");

                var request = new HttpRequestMessage(HttpMethod.Post, new Uri(query.TargetUrl)) {
                    Content = new StringContent(json.ToString())
                };
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                var doc = DynamicJson.Parse(a);
                SessionApi = doc.data;

                return SessionApi.session.content_uri;
            } catch (RequestFailed) {

                return null;
            }
        }

        public async Task DeleteAsync() {

            try {

                var session = DmcInfo.movie.session;
                var query = new GetRequestQuery(session.urls[0].url + "/" + SessionApi.session.id);
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

                var session = DmcInfo.movie.session;
                var query = new GetRequestQuery(session.urls[0].url + "/" + SessionApi.session.id);
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

                if (json.data()) {

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

            if (bitrate < 1000000) {

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
