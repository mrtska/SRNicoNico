using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Livet;
using System.IO;
using Codeplex.Data;

namespace SRNicoNico.Models.NicoNicoWrapper {

	public enum SigninStatus {

		ServiceUnavailable = -2,    //メンテナンス中
		Failed = -1,                //ログイン失敗
		Success = 1,                //ログイン成功
	}

	public enum NiconicoAccountAuthority {

		//ログインしていない
		NotSignedIn = 0,

		//一般会員
		Normal = 1,

		//プレミアム会員
		Premium = 3,
	}

	public class NicoNicoSession : NotificationObject {

		//サインインURL
		private const string SignInURL = "https://secure.nicovideo.jp/secure/login?site=niconico";

        //アカウント権限
        private const string UserAgent = "SRNicoNico/1.0";

        //ニコニコトップ
        private const string NicoNicoTop = "http://www.nicovideo.jp/";

        //Http通信用
        private HttpClient HttpClient;
        private HttpClientHandler HttpHandler;

		//ユーザーID
		public string UserId { get; internal set; }

		//ログイン情報
		public string Key { get; set; }
		public DateTimeOffset Expire;

		//アカウント権限
		private NiconicoAccountAuthority Authority = NiconicoAccountAuthority.NotSignedIn;


		public NicoNicoSession() {
			
			HttpHandler = new HttpClientHandler();
			HttpHandler.UseCookies = true;
			HttpHandler.AllowAutoRedirect = false;
			HttpHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
			HttpClient = new HttpClient(HttpHandler, false);
			HttpClient.DefaultRequestHeaders.Add("user-agent", UserAgent);
            HttpClient.Timeout = TimeSpan.FromSeconds(30);

        }

        //オートログイン時
        public NicoNicoSession(string key, DateTimeOffset expire) {

			HttpHandler = new HttpClientHandler();
			HttpHandler.UseCookies = true;
			HttpHandler.AllowAutoRedirect = false;
			HttpHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
			HttpClient = new HttpClient(HttpHandler, false);
			HttpClient.DefaultRequestHeaders.Add("user-agent", UserAgent);
            HttpClient.Timeout = TimeSpan.FromSeconds(30);
			Key = key;

			//Cookieを設定する
			Cookie cookieKey = new Cookie("user_session", key, "/", ".nicovideo.jp");
			cookieKey.Expires = expire.DateTime;
			HttpHandler.CookieContainer.Add(cookieKey);
		}

        
        public async Task<string> GetAsync(HttpRequestMessage request)  {

            try {

                VerifyRequest(request);

                HttpResponseMessage response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                VerifyResponse(response);

                return await response.Content.ReadAsStringAsync();

            } catch(AggregateException e) {

                throw new RequestTimeout(e);
            }
        }

        public async Task<string> GetAsync(string uri) {
            
            return await GetAsync(new HttpRequestMessage(HttpMethod.Get, uri));
        }

        public async Task<HttpResponseMessage> GetResponseAsync(HttpRequestMessage request) {

            try {

                VerifyRequest(request);

                HttpResponseMessage response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                VerifyResponse(response);

                return response;

            } catch(AggregateException e) {

                throw new RequestTimeout(e);
            }
        }
        
        public async Task<HttpResponseMessage> GetResponseAsync(string uri) {

            return await GetResponseAsync(new HttpRequestMessage(HttpMethod.Get, uri));
        }


        public async Task<Stream> GetStreamAsync(HttpRequestMessage request) {

            try {
                VerifyRequest(request);

                HttpResponseMessage response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                VerifyResponse(response);

                return await response.Content.ReadAsStreamAsync();

            } catch(AggregateException e) {

                throw new RequestTimeout(e);
            }
        }

        public async Task<Stream> GetStreamAsync(string uri) {

            return await GetStreamAsync(new HttpRequestMessage(HttpMethod.Get, uri));
        }

        private void VerifyRequest(HttpRequestMessage request) {

            //アクセスログに登録
            App.ViewModelRoot.AccessLog.StartAccessUrl(request);
        }

        //レスポンスの正当性を確認
        private void VerifyResponse(HttpResponseMessage response) {

            
            System.Diagnostics.Debug.WriteLine("Access:" + response.RequestMessage.RequestUri + " ResponseCode:" + response.StatusCode);
            App.ViewModelRoot.AccessLog.EndAccessUrl(response);
        }

        //サインイン
        public SigninStatus SignIn(string address, string passwd) {

			var request = new Dictionary<string, string>();
			request.Add("mail_tel", address);		//アドレス
			request.Add("password", passwd);		//パスワード

			//サインインリクエスト
			return HttpClient.PostAsync(SignInURL, new FormUrlEncodedContent(request)).ContinueWith(prevTask => {

				return SignInInternal();
			}).Result;
		}

		//セッションを確立した後に呼ぶ
		public SigninStatus SignInInternal() {

			//ニコニコTOPにレスポンスヘッダを要求する
			HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Head, NicoNicoTop);

			HttpResponseMessage response = HttpClient.SendAsync(message).Result;

			//成功したら
			if(response.StatusCode == HttpStatusCode.OK) {

				//レスポンスヘッダにユーザーIDが無かったらログイン失敗
				if(!response.Headers.Contains("x-niconico-id")) {

					return SigninStatus.Failed;
				}
                //ユーザーIDを取得
                UserId = response.Headers.GetValues("x-niconico-id").Single();

                //アカウント権限
                Authority = (NiconicoAccountAuthority)int.Parse(response.Headers.GetValues("x-niconico-authflag").Single());

				//cookieを取得
				var cookie = HttpHandler.CookieContainer.GetCookies(new Uri("http://nicovideo.jp/")).Cast<Cookie>()
									.Where( c => c.Name == "user_session" && c.Path == "/" ).OrderByDescending( c => c.Expires.Ticks ).First();

				if(cookie != null && cookie.Expires != null) {

                    //cookieをもとにキーと有効期限を取得
                    Key = cookie.Value;
                    Expire = cookie.Expires;

                    App.SetCookie(new Uri("http://nicovideo.jp/"), "user_session=" + Key);

                    App.ViewModelRoot.LogedInInit();
					return SigninStatus.Success;
				}
			}

			//サインイン失敗
			return SigninStatus.Failed;
		}


        //終了時
        public void Dispose() {

            HttpHandler.Dispose();
            HttpClient.Dispose();
        }

	}
}
