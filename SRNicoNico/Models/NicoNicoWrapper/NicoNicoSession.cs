using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Livet;

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


		/*
		 * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
		 */


		//サインインURL
		private const string SignInURL = "https://secure.nicovideo.jp/secure/login?site=niconico";


		//アカウント権限
		private const string UserAgent = "SRNicoNico/1.0";

		private const string NicoNicoTop = "http://www.nicovideo.jp/";


		//Http通信用
		public HttpClient HttpClient { get; internal set; }
		public HttpClientHandler HttpHandler { get; internal set; }

		//ユーザーID
		public uint UserId { get; internal set; }

		//ログイン情報
		public string Key { get; set; }
		public DateTimeOffset Expire;

		//アカウント権限
		private NiconicoAccountAuthority Authority = NiconicoAccountAuthority.NotSignedIn;


		public NicoNicoSession() {

			
			this.HttpHandler = new HttpClientHandler();
			this.HttpHandler.UseCookies = true;
			this.HttpHandler.AllowAutoRedirect = false;
			this.HttpHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
			this.HttpClient = new HttpClient(this.HttpHandler, false);
			this.HttpClient.DefaultRequestHeaders.Add("user-agent", NicoNicoSession.UserAgent);
		}

		//オートログイン時
		public NicoNicoSession(string key, DateTimeOffset expire) {

			this.HttpHandler = new HttpClientHandler();
			this.HttpHandler.UseCookies = true;
			this.HttpHandler.AllowAutoRedirect = false;
			this.HttpHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
			this.HttpClient = new HttpClient(this.HttpHandler, false);
			this.HttpClient.DefaultRequestHeaders.Add("user-agent", NicoNicoSession.UserAgent);

			this.Key = key;

			//Cookieを設定する
			Cookie cookieKey = new Cookie("user_session", key, "/", ".nicovideo.jp");
			cookieKey.Expires = expire.DateTime;
			this.HttpHandler.CookieContainer.Add(cookieKey);
		}

		//サインイン
		public SigninStatus SignIn(string address, string passwd) {


			var request = new Dictionary<string, string>();
			request.Add("mail_tel", address);		//アドレス
			request.Add("password", passwd);		//パスワード

			//サインインリクエスト
			return this.HttpClient.PostAsync(SignInURL, new FormUrlEncodedContent(request)).ContinueWith(prevTask => {

				return SignInInternal();
			}).Result;
		}

		//セッションを確立した後に呼ぶ
		public SigninStatus SignInInternal() {



			//ニコニコTOPにレスポンスヘッダを要求する
			HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Head, NicoNicoTop);


			HttpResponseMessage response = this.HttpClient.SendAsync(message).Result;

			//成功したら
			if(response.StatusCode == HttpStatusCode.OK) {

				//レスポンスヘッダにユーザーIDが無かったらログイン失敗
				if(!response.Headers.Contains("x-niconico-id")) {

					return SigninStatus.Failed;
				}
				//ユーザーIDを取得
				this.UserId = uint.Parse(response.Headers.GetValues("x-niconico-id").Single());

				//アカウント権限
				this.Authority = (NiconicoAccountAuthority)int.Parse(response.Headers.GetValues("x-niconico-authflag").Single());

				//cookieを取得
				var cookie = HttpHandler.CookieContainer.GetCookies(new Uri("http://nicovideo.jp/")).Cast<Cookie>()
									.Where( c => c.Name == "user_session" && c.Path == "/" ).OrderByDescending( c => c.Expires.Ticks ).First();

				if(cookie != null && cookie.Expires != null) {

					//cookieをもとにキーと有効期限を取得
					this.Key = cookie.Value;
					this.Expire = cookie.Expires;


					return SigninStatus.Success;
				}
			}

			//サインイン失敗
			return SigninStatus.Failed;
		}
	}
}
