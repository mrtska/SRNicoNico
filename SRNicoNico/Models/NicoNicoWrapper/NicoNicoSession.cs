﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using SRNicoNico.Models.NicoNicoViewer;
using System.IO;
using System.Windows;

namespace SRNicoNico.Models.NicoNicoWrapper {

    public class NicoNicoSession : NotificationObject {

        //サインインURL
        private const string SignInURL = "https://secure.nicovideo.jp/secure/login?site=niconico";

        //User Agent
        private const string UserAgent = "SRNicoNico/1.0 (@m__gl user/23425727)";

        //ニコニコトップ
        private const string NicoNicoTop = "https://www.nicovideo.jp/";

        //Http通信用
        private readonly HttpClient HttpClient;
        public readonly HttpClientHandler HttpHandler;


        //ユーザーID
        public string UserId { get; private set; }


        //ログイン情報
        public string Key { get; set; }
        public DateTimeOffset Expire;

        public NicoNicoSession(string userSession) {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            HttpHandler = new HttpClientHandler();
            HttpHandler.UseCookies = true;
            HttpHandler.AllowAutoRedirect = false;
            HttpHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            HttpClient = new HttpClient(HttpHandler, false);
            HttpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            HttpClient.Timeout = TimeSpan.FromSeconds(30);

            SetUserSession(userSession);
        }

        //Cookieを設定する
        public void SetUserSession(string userSession) {

            HttpHandler.CookieContainer.Add(new Cookie("user_session", userSession, "/", ".nicovideo.jp"));
            HttpHandler.CookieContainer.Add(new Cookie("watch_html5", "1", "/", ".nicovideo.jp"));
            HttpHandler.CookieContainer.Add(new Cookie("official_player_name", "alies", "/", ".live2.nicovideo.jp"));
            HttpHandler.CookieContainer.Add(new Cookie("player_version", "leo", "/", ".live2.nicovideo.jp"));
            Key = userSession;
        }
    

        public async Task<string> GetAsync(HttpRequestMessage request) {

            try {

                VerifyRequest(request);

                using(var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)) {

                    return await (await VerifyAndResolveResponse(response)).Content.ReadAsStringAsync();
                }
            } catch(AggregateException e) {

                throw new RequestFailed(e, FailedType.TimeOut);
            } catch(Exception e) {

               throw new RequestFailed(e, FailedType.Failed);
            }
        }

        public async Task<string> GetAsync(string uri) {

            return await GetAsync(new HttpRequestMessage(HttpMethod.Get, uri));
        }

        public async Task<HttpResponseMessage> GetResponseAsync(HttpRequestMessage request) {

            try {

                VerifyRequest(request);

                var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                response = await VerifyAndResolveResponse(response);

                return response;

            } catch(AggregateException e) {

                throw new RequestFailed(e, FailedType.TimeOut);
            } catch(Exception e) {

                throw new RequestFailed(e, FailedType.Failed);
            }
        }

        public async Task<HttpResponseMessage> GetResponseAsync(string uri) {

            return await GetResponseAsync(new HttpRequestMessage(HttpMethod.Get, uri));
        }


        public async Task<Stream> GetStreamAsync(HttpRequestMessage request) {

            try {
                VerifyRequest(request);

                using(var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)) {
                    
                    return await (await VerifyAndResolveResponse(response)).Content.ReadAsStreamAsync();
                }

            } catch(AggregateException e) {

                throw new RequestFailed(e, FailedType.TimeOut);
            } catch(Exception e) {

                throw new RequestFailed(e, FailedType.Failed);
            }
        }

        public async Task<Stream> GetStreamAsync(string uri) {

            return await GetStreamAsync(new HttpRequestMessage(HttpMethod.Get, uri));
        }
        

        private void VerifyRequest(HttpRequestMessage request) {

            //アクセスログに登録
            //App.ViewModelRoot.AccessLog.StartAccessUrl(request);
        }

        //レスポンスの正当性を確認
        private async Task<HttpResponseMessage> VerifyAndResolveResponse(HttpResponseMessage response) {

            System.Diagnostics.Debug.WriteLine("Access:" + response.RequestMessage.RequestUri + " ResponseCode:" + response.StatusCode);

            if(response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.Moved) {

                return await GetResponseAsync(response.Headers.Location.OriginalString);
            }

            //レスポンスヘッダにユーザーIDが無かったらログイン失敗
            if (response.RequestMessage.RequestUri.OriginalString.Contains("www.nicovideo.jp/") && !response.Headers.Contains("x-niconico-id")) {

                //App.ViewModelRoot.CurrentUser = await App.ViewModelRoot.SignIn.SignInAsync();
                MessageBox.Show("セッションが切れました。再度ログインしてください。メンテナンスに入った場合もこの通知が出る場合があります。");
                Environment.Exit(0);
                return null;
            } else {

                return response;
            }
        }

        public async Task<SigninStatus> VerifySignInAsync() {

            try {

                //ニコニコTOPにレスポンスヘッダを要求する
                var message = new HttpRequestMessage(HttpMethod.Head, NicoNicoTop);

                using(var response = await HttpClient.SendAsync(message)) {

                    //成功したら
                    if(response.StatusCode == HttpStatusCode.OK) {

                        //レスポンスヘッダにユーザーIDが無かったらログイン失敗
                        if(!response.Headers.Contains("x-niconico-id")) {

                            return SigninStatus.Failed;
                        }
                        //ユーザーIDを取得
                        UserId = response.Headers.GetValues("x-niconico-id").Single();

                        var cookie = HttpHandler.CookieContainer.GetCookies(new Uri("https://nicovideo.jp/")).Cast<Cookie>()
                                            .Where(c => c.Name == "user_session" && c.Path == "/").OrderByDescending(c => c.Expires.Ticks).First();

                        if(cookie != null && cookie.Expires != null) {

                            return SigninStatus.Success;
                        }
                    }

                    if(response.StatusCode == HttpStatusCode.ServiceUnavailable) {

                        return SigninStatus.ServiceUnavailable;
                    }
                }

                //サインイン失敗
                return SigninStatus.Failed;
            } catch(RequestFailed) {

                return SigninStatus.Failed;
            }
        }


    }

    public enum SigninStatus {

        ServiceUnavailable = -2,    //メンテナンス中
        Failed = -1,                //ログイン失敗
        Success = 1,                //ログイン成功
    }
}
