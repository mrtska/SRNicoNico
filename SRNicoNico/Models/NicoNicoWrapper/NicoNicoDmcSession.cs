using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;

using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoDmcSession : NotificationObject {

        private NicoNicoGetDmc GetDmc;


        private string LastResponseXml;

        public Timer HeartbeatTimer;



        public NicoNicoDmcSession(NicoNicoGetDmc dmc) {

            GetDmc = dmc;
        }

        public async Task<DmcSession> CreateAsync() {

            var xml = new XmlDocument();

            var session = xml.CreateElement("session");
            {

                var recipeId = xml.CreateElement("recipe_id");
                recipeId.InnerText = GetDmc.RecipeId;
                session.AppendChild(recipeId);

                var contentId = xml.CreateElement("content_id");
                contentId.InnerText = GetDmc.ContentId;
                session.AppendChild(contentId);

                var contentType = xml.CreateElement("content_type");
                contentType.InnerText = "movie";
                session.AppendChild(contentType);

                var protocol = xml.CreateElement("protocol");
                {
                    var name = xml.CreateElement("name");
                    name.InnerText = "http";
                    protocol.AppendChild(name);

                    var parameters = xml.CreateElement("parameters");
                    {
                        var httpParameters = xml.CreateElement("http_parameters");
                        {
                            var method = xml.CreateElement("method");
                            method.InnerText = "GET";
                            httpParameters.AppendChild(method);

                            var parameter = xml.CreateElement("parameters");
                            {
                                var httpOutputDownloadParameters = xml.CreateElement("http_output_download_parameters");
                                {
                                    var fileExtension = xml.CreateElement("file_extension");
                                    fileExtension.InnerText = "mp4";
                                    httpOutputDownloadParameters.AppendChild(fileExtension);
                                }
                                parameter.AppendChild(httpOutputDownloadParameters);
                            }
                            httpParameters.AppendChild(parameter);
                        }
                        parameters.AppendChild(httpParameters);
                    }
                    protocol.AppendChild(parameters);
                }
                session.AppendChild(protocol);

                var priority = xml.CreateElement("priority");
                priority.InnerText = GetDmc.Priority.ToString();
                session.AppendChild(priority);

                var contentSrcIdSets = xml.CreateElement("content_src_id_sets");
                {
                    var contentSrcIdSet = xml.CreateElement("content_src_id_set");
                    {
                        var contentSrcIds = xml.CreateElement("content_src_ids");
                        {
                            var srcIdToMux = xml.CreateElement("src_id_to_mux");
                            {
                                var videoSrcIds = xml.CreateElement("video_src_ids");
                                foreach(var video in GetDmc.Videos) {

                                    var str = xml.CreateElement("string");
                                    str.InnerText = video;
                                    videoSrcIds.AppendChild(str);
                                }
                                srcIdToMux.AppendChild(videoSrcIds);

                                var audioSrcIds = xml.CreateElement("audio_src_ids");
                                foreach(var video in GetDmc.Audios) {

                                    var str = xml.CreateElement("string");
                                    str.InnerText = video;
                                    audioSrcIds.AppendChild(str);
                                }
                                srcIdToMux.AppendChild(audioSrcIds);
                            }

                            contentSrcIds.AppendChild(srcIdToMux);
                        }
                        contentSrcIdSet.AppendChild(contentSrcIds);
                    }
                    contentSrcIdSets.AppendChild(contentSrcIdSet);
                }
                session.AppendChild(contentSrcIdSets);

                var keepMethod = xml.CreateElement("keep_method");
                {
                    var heartBeat = xml.CreateElement("heartbeat");
                    {
                        var lifetime = xml.CreateElement("lifetime");
                        lifetime.InnerText = GetDmc.HeartbeatLifeTime.ToString();
                        heartBeat.AppendChild(lifetime);
                    }
                    keepMethod.AppendChild(heartBeat);
                }
                session.AppendChild(keepMethod);

                var timingConstraint = xml.CreateElement("timing_constraint");
                timingConstraint.InnerText = "unlimited";
                session.AppendChild(timingConstraint);

                var sessionOperationAuth = xml.CreateElement("session_operation_auth");
                {
                    var sessionOperationAuthBySignature = xml.CreateElement("session_operation_auth_by_signature");
                    {
                        var token = xml.CreateElement("token");
                        token.InnerText = GetDmc.Token;
                        sessionOperationAuthBySignature.AppendChild(token);

                        var signature = xml.CreateElement("signature");
                        signature.InnerText = GetDmc.Signature;
                        sessionOperationAuthBySignature.AppendChild(signature);
                    }
                    sessionOperationAuth.AppendChild(sessionOperationAuthBySignature);
                }
                session.AppendChild(sessionOperationAuth);

                var contentAuth = xml.CreateElement("content_auth");
                {
                    var authType = xml.CreateElement("auth_type");
                    authType.InnerText = GetDmc.AuthType;
                    contentAuth.AppendChild(authType);

                    var serviceId = xml.CreateElement("service_id");
                    serviceId.InnerText = "nicovideo";
                    contentAuth.AppendChild(serviceId);

                    var serviceUserId = xml.CreateElement("service_user_id");
                    serviceUserId.InnerText = GetDmc.ServiceUserId;
                    contentAuth.AppendChild(serviceUserId);

                    var maxContentCount = xml.CreateElement("max_content_count");
                    maxContentCount.InnerText = "10";
                    contentAuth.AppendChild(maxContentCount);

                    var contentKeyTimeout = xml.CreateElement("content_key_timeout");
                    contentKeyTimeout.InnerText = GetDmc.ContentKeyTimeout.ToString();
                    contentAuth.AppendChild(contentKeyTimeout);
                }
                session.AppendChild(contentAuth);

                var clientInfo = xml.CreateElement("client_info");
                {
                    var playerId = xml.CreateElement("player_id");
                    playerId.InnerText = GetDmc.PlayerId;
                    clientInfo.AppendChild(playerId);
                }
                session.AppendChild(clientInfo);
            }
            xml.AppendChild(session);


            try {

                var query = new GetRequestQuery(GetDmc.ApiUrl);
                query.AddQuery("_format", "xml");
                query.AddQuery("suppress_response_codes", "true");

                var request = new HttpRequestMessage(HttpMethod.Post, new Uri(query.TargetUrl));

                request.Content = new StringContent(xml.OuterXml);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var a = await NicoNicoWrapperMain.Session.GetAsync(request);

                var ret = new DmcSession();

                var doc = new XmlDocument();
                doc.LoadXml(a);
                ret.Id = doc.SelectSingleNode("/object/data/session/id").InnerText;
                ret.ContentUri = doc.SelectSingleNode("/object/data/session/content_uri").InnerText;

                LastResponseXml = doc.SelectSingleNode("/object/data").InnerXml;
                return ret;
            } catch(RequestTimeout) {

                return null;
            }
        }
        

        internal void Heartbeat(object state) {

            var id = Convert.ToString(state);
            HeartbeatAsync(id);
        }

        public async void HeartbeatAsync(string id) {


            try {

                var query = new GetRequestQuery(GetDmc.ApiUrl + "/" + id);
                query.AddQuery("_format", "xml");
                query.AddQuery("_method", "PUT");
                query.AddQuery("suppress_response_codes", "true");

                var request = new HttpRequestMessage(HttpMethod.Post, new Uri(query.TargetUrl));

                request.Content = new StringContent(LastResponseXml);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var a = await NicoNicoWrapperMain.Session.GetAsync(request);


                var doc = new XmlDocument();
                doc.LoadXml(a);

                LastResponseXml = doc.SelectSingleNode("/object/data").InnerXml;

                return;
            } catch(RequestTimeout) {

                return;
            }

        }

    }

    public class DmcSession {

        //セッションID
        public string Id { get; set; }

        //
        public string ContentUri { get; set; }
    }

    public class NicoNicoGetDmc {

        //session API
        public string ApiUrl { get; set; }

        //オーディオコーデック
        public List<string> Audios { get; set; }

        //認証タイプ
        public string AuthType { get; set; }

        //まだ分からぬ
        public string ContentId { get; set; }

        //
        public int ContentKeyTimeout { get; set; }

        //
        public int HeartbeatLifeTime { get; set; }

        //
        public List<string> Movies { get; set; }

        //
        public string PlayerId { get; set; }

        //
        public double Priority { get; set; }

        //http以外あるのか謎
        public List<string> Protocols { get; set; }

        //
        public string RecipeId { get; set; }

        //どうみても自分のユーザーID
        public string ServiceUserId { get; set; }

        //
        public string Signature { get; set; }

        //
        public string Token { get; set; }

        //
        public List<string> Videos { get; set; }

        //コメントサーバーURL
        public string MessageServerUrl { get; set; }

        //コメントのスレッドID
        public string ThreadId { get; set; }

        //
        public bool ThreadKeyRequired { get; set; }

        //
        public bool PostKeyAvailable { get; set; }

        //オリジナルデータはstringじゃないけどintじゃ扱えないのでメンドイからstringにした
        public string Time { get; set; }



    }
}
