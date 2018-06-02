using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using HtmlAgilityPack;
using SRNicoNico.ViewModels;
using Codeplex.Data;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;

namespace SRNicoNico.Models.NicoNicoWrapper {

    public class NicoNicoDmcSession {

        private NicoNicoDmc Dmc;

        private string LastResponseXml;


        public NicoNicoDmcSession(NicoNicoDmc dmc) {

            Dmc = dmc;
        }

        public async Task<DmcSession> CreateAsync() {


            dynamic json = new DynamicJson();

            json.session = new {

                recipe_id = Dmc.RecipeId,
                content_id = Dmc.ContentId,
                content_type = "movie",

                content_src_id_sets = new List<dynamic>(),
                timing_constraint = "unlimited",
                keep_method = new {
                    heartbeat = new {
                        lifetime = Dmc.HeartbeatLifeTime
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

                        token = Dmc.Token,
                        signature = Dmc.Signature
                    }
                },
                content_auth = new {

                    auth_type = Dmc.AuthType,
                    max_content_count = 10,
                    content_key_timeout = Dmc.ContentKeyTimeout,
                    service_id = "nicovideo",
                    service_user_id = Dmc.ServiceUserId
                },
                client_info = new {
                    player_id = Dmc.PlayerId
                },
                priority = Dmc.Priority
            };


            var mux = new {
                video_src_ids = new List<string>(),
                audio_src_ids = new List<string>()

            };

            foreach(var video in Dmc.Videos) {

                mux.video_src_ids.Add(video);
            }
            foreach(var audio in Dmc.Audios) {

                mux.audio_src_ids.Add(audio);
            }


            json.session.content_src_id_sets = new[] { new { content_src_ids = new[] { new { src_id_to_mux = mux } } } };

            try {
                var str = json.ToString();

                dynamic session = Dmc.ApiUrls.First();

                var query = new GetRequestQuery(session.url);
                query.AddQuery("_format", "json");

                var request = new HttpRequestMessage(HttpMethod.Post, new Uri(query.TargetUrl)) {
                    Content = new StringContent(json.ToString())
                };
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);

                var ret = new DmcSession();

                var doc = DynamicJson.Parse(a);
                ret.Id = doc.data.session.id;
                ret.ContentUri = doc.data.session.content_uri;

                LastResponseXml = doc.data.ToString();
                return ret;
            } catch(RequestFailed) {

                return null;
            }
        }

        public async Task HeartbeatAsync(string id) {

            try {

                dynamic session = Dmc.ApiUrls.First();
                var query = new GetRequestQuery(session.url + "/" + id);
                query.AddQuery("_format", "json");
                query.AddQuery("_method", "PUT");

                var request = new HttpRequestMessage(HttpMethod.Post, new Uri(query.TargetUrl));

                request.Content = new StringContent(LastResponseXml);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(request);
               
                if(!NicoNicoUtil.IsValidJson(a)) {

                    return;
                }
                var doc = DynamicJson.Parse(a);

                if(doc.data()) {

                    var data = doc.data;
                    if(data != null) {

                        LastResponseXml = data.ToString();
                    } else {

                    }
                }

                return;
            } catch(RequestFailed) {

                return;
            }

        }
    }

    public class DmcSession {

        //セッションID
        public string Id { get; set; }

        //動画URL
        public string ContentUri { get; set; }
    }
}
