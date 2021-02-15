using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Livet;
using Livet.Messaging;
using Livet.Messaging.Windows;
using Microsoft.Web.WebView2.Wpf;
using SRNicoNico.Services;
using SRNicoNico.Views;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// サインイン時に使用するViewModel
    /// </summary>
    public class SignInViewModel : ViewModel {

        private const string SignInUrl = "https://account.nicovideo.jp/login";

        private readonly NicoNicoSessionService SessionService;

        private WebView2? _WebView;
        /// <summary>
        /// サインイン時に使用するWebView
        /// </summary>
        public WebView2? WebView {
            get { return _WebView; }
            set { 
                if (_WebView == value)
                    return;
                _WebView = value;
                RaisePropertyChanged();
            }
        }

        private readonly InteractionMessenger MainWindowMessenger;

        public SignInViewModel(MainWindowViewModel vm, NicoNicoSessionService sessionService) {

            MainWindowMessenger = vm.Messenger;
            SessionService = sessionService;
        }

        public async void Initialize() {

            // SessionServiceがサインインを必要とした時に呼ばれる
            SessionService.SignInDialogHandler = () => {
                // サインインダイアログを出す
                return ActivateSignInDialogAsync();
            };

            var a = await SessionService.VerifyAsync();
        }

        public Task ActivateSignInDialogAsync() {

            WebView = new WebView2 {
                Source = new Uri(SignInUrl)
            };
            WebView.CoreWebView2InitializationCompleted += (o, e) => {
                WebView.CoreWebView2.WebResourceResponseReceived += async (o, e) => {

                    // トップページに遷移した時にCookieの値を確認する
                    if (e.Request.Uri.EndsWith("nicovideo.jp/")) {

                        var cookies = await WebView.CoreWebView2.CookieManager.GetCookiesAsync("https://nicovideo.jp/");
                        var session = cookies.SingleOrDefault(s => s.Name == "user_session");
                        if (session != null) {

                            // セッションCookieを保存してサインインウィンドウを閉じる
                            SessionService.StoreSession(session.Value);
                            await Messenger.RaiseAsync(new WindowActionMessage(WindowAction.Close, "SignIn"));

                            WebView.Dispose();
                        } else {
                            // トップページに遷移したのにCookieが無かった場合は再度サインインページに遷移する
                            WebView.Source = new Uri(SignInUrl);
                        }
                    }
                };
            };
            return MainWindowMessenger.RaiseAsync(new TransitionMessage(typeof(SignInView), this, TransitionMode.Modal));
        }

        /// <summary>
        /// サインインダイアログを閉じようとした時はアプリを終了する
        /// </summary>
        public void ExitButtonDown() {

            Application.Current.Shutdown();
        }



    }
}
