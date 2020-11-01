using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Livet;
using Livet.Messaging;
using Microsoft.Web.WebView2.Wpf;
using SRNicoNico.Services;
using SRNicoNico.Views;

namespace SRNicoNico.ViewModels {
    public class SignInViewModel : ViewModel {

        private readonly NicoNicoSessionService SessionService;

        private WebView2? _WebView;

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
            WebView = new WebView2 {
                Source = new Uri("https://account.nicovideo.jp/login")
            };
            WebView.CoreWebView2Ready += (o, e) => {

                WebView.CoreWebView2.WebResourceResponseReceived += (o, e) => {

                    // まだ使えなくて困った
                    ;
                };
            };
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
