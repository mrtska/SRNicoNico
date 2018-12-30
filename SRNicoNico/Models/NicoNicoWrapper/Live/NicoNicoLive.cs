using Codeplex.Data;
using HtmlAgilityPack;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SRNicoNico.Models.NicoNicoWrapper {

    public class NicoNicoLive : NotificationObject {

        public string LiveUrl { get; private set; }

        #region ApiData変更通知プロパティ
        private NicoNicoLiveApi _ApiData;

        public NicoNicoLiveApi ApiData {
            get { return _ApiData; }
            set { 
                if (_ApiData == value)
                    return;
                _ApiData = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private ClientWebSocket LiveWebSocket;


        public NicoNicoLive(string url) {

            if(string.IsNullOrEmpty(url)) {

                throw new ArgumentNullException(url);
            }
            LiveUrl = url;
        }

        


        private async void HandleLiveSocket(dynamic body) {

            Console.WriteLine(body.ToString());

            if(LiveWebSocket.State != WebSocketState.Open) {

                return;
            }

            if(body.type == "ping") {

                await LiveWebSocket.SendAsync(@"{""type"":""pong"",""body"":{}}", Encoding.ASCII);
                return;
            }

            if(body.type == "watch") {

                if(body.body.command()) {

                    switch (body.body.command) {

                        case "currentstream": {

                                var stream = body.body.currentStream;

                                ApiData.HlsUrl = stream.uri;
                                break;
                            }
                        case "currentroom": {

                                var room = body.body.room;

                                var comment = new CommentRoom {

                                    RoomName  = room.roomName,
                                    ThreadId  = room.threadId,
                                    CommentSocketUrl = room.messageServerUri
                                };
                                ApiData.Comment = comment;

                                break;
                            }
                    }
                }
                return;
            }

            ;
        }

        public async Task<string> GetLiveData() {

            try {

                var a = await App.ViewModelRoot.CurrentUser.Session.GetAsync(LiveUrl);

                var html = new HtmlDocument();
                html.LoadHtml(a);

                var node = html.GetElementbyId("embedded-data");

                if(node == null) {

                    return "Node が null";
                }

                var json = DynamicJson.Parse(HttpUtility.HtmlDecode(node.Attributes["data-props"].Value));

                var api = new NicoNicoLiveApi();
                api.LiveId = json.program.nicoliveProgramId;
                api.Title = json.program.title;
                api.Description = json.program.description;
                api.BroadcastId = json.program.broadcastId;

                api.ApiBaseUrl = json.site.relive.apiBaseUrl;
                api.WebSocketUrl = json.site.relive.webSocketUrl;

                ApiData = api;

                LiveWebSocket = new ClientWebSocket();
                LiveWebSocket.Options.Cookies = App.ViewModelRoot.CurrentUser.Session.HttpHandler.CookieContainer;
                LiveWebSocket.Options.SetRequestHeader("Origin", "http://live2.nicovideo.jp");

                await LiveWebSocket.ConnectAsync(new Uri(api.WebSocketUrl), default(CancellationToken));

                
                // WebSocketのレスポンスハンドリングを別スレッドで
#pragma warning disable CS4014 // この呼び出しを待たないため、現在のメソッドの実行は、呼び出しが完了する前に続行します
                Task.Run(async () => {

                    while(LiveWebSocket.State == WebSocketState.Open) {

                        var buffer = new ArraySegment<byte>(new byte[196765]);
                        var ret = await LiveWebSocket.ReceiveAsync(buffer, default(CancellationToken));
                        
                        if(ret.MessageType == WebSocketMessageType.Text) {

                            HandleLiveSocket(DynamicJson.Parse(Encoding.UTF8.GetString(buffer.Take(ret.Count).ToArray())));
                        } else {

                            ;
                        }
                    }
                    ;
                });
#pragma warning restore CS4014 // この呼び出しを待たないため、現在のメソッドの実行は、呼び出しが完了する前に続行します

                Console.WriteLine(LiveWebSocket.State);

                var getpermit = new {
                    type = "watch",
                    body = new {
                        command = "getpermit",
                        requirement = new {
                            broadcastId = ApiData.BroadcastId,
                            route = "",
                            room = new {
                                isCommentable = true,
                                protocol = "webSocket"
                            },
                            stream = new {

                                isLowLatency = true,
                                priorStreamQuality = "super_high",
                                protocol = "hls",
                                requireNewStream = true
                            }
                        }
                    }
                };
                await LiveWebSocket.SendAsync(DynamicJson.Serialize(getpermit), Encoding.UTF8);


                return "ok";
            } catch(RequestFailed) {

                return "しっぱい";
            }
        }




    }
}
