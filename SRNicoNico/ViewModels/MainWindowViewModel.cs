using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Livet;
using Livet.Messaging;
using Livet.Messaging.Windows;
using SRNicoNico.Models;
using SRNicoNico.Services;
using Unity;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// MainWindowクラスのDataContext
    /// </summary>
    public class MainWindowViewModel : ViewModel {

#if DEBUG
        private string _Title = "NicoNicoViewer Debug Build ";
#else
        private string _Title = "NicoNicoViewer Beta";
#endif
        public string Title {
            get { return _Title; }
            set {
                if (_Title == value)
                    return;
                _Title = value;
                RaisePropertyChanged();
            }
        }

        private string _Status = string.Empty;
        /// <summary>
        /// ステータスバーに表示する文字列
        /// </summary>
        public string Status {
            get { return _Status; }
            set { 
                if (_Status == value)
                    return;
                _Status = value;
                RaisePropertyChanged();
            }
        }

        private string _CurrentTime = string.Empty;
        /// <summary>
        /// ステータスバーに表示する現在時刻
        /// </summary>
        public string CurrentTime {
            get { return _CurrentTime; }
            set { 
                if (_CurrentTime == value)
                    return;
                _CurrentTime = value;
                RaisePropertyChanged();
            }
        }

        private bool _CanClose;
        /// <summary>
        /// 閉じても問題ないかどうか
        /// </summary>
        public bool CanClose {
            get { return _CanClose; }
            set { 
                if (_CanClose == value)
                    return;
                _CanClose = value;
                RaisePropertyChanged();
            }
        }
        public MainContentViewModel MainContent { get; private set; }

        public SignInViewModel SignIn { get; private set; }

        private readonly ISettings Settings;
        // 時刻更新用のタイマー
        private readonly Timer Timer;

        /// <summary>
        /// 各ViewModelを初期化する
        /// </summary>
        /// <param name="container">DIコンテナ</param>
        public MainWindowViewModel(IUnityContainer container) {

            // MainWindowのMessangerをDIに登録しておく
            container.RegisterInstance(Messenger);

            MainContent = container.Resolve<MainContentViewModel>();
            SignIn = container.Resolve<SignInViewModel>();
            Settings = container.Resolve<ISettings>();
            CompositeDisposable.Add(MainContent);

            // MainContentのViewModelもDIに登録しておく
            container.RegisterInstance(MainContent);

            Timer = new Timer(o => {
                CurrentTime = DateTime.Now.ToString();
            }, null, 0, 1000);
            CompositeDisposable.Add(Timer);
        }

        /// <summary>
        /// MainWindowがロードされた時に呼ばれる
        /// </summary>
        public async void OnLoaded() {

            Status = "サインイン中";

            var result = await SignIn.EnsureSignedInAsync();

            if (result == AuthenticationResult.Normal) {

                    MessageBox.Show("ベータ版のため、プレミアム会員のみ使用可能です。");
                    Environment.Exit(0);
            }

            if (result != AuthenticationResult.Unauthorized) {

                // 正常にサインイン出来たらサインイン後に使用できるタブを追加する
                MainContent.PostInitialize();
                MainContent.RegisterStatusChangeAction(status => Status = status);
                Status = "サインイン完了";
            } else {

                // ログインしたのにサインイン出来なかった
            }
        }

        public void KeyDown(KeyEventArgs e) {

            MainContent.SelectedItem?.KeyDown(e);
        }
        public void KeyUp(KeyEventArgs e) {

            MainContent.SelectedItem?.KeyUp(e);
        }
        public void MouseDown(MouseButtonEventArgs e) {

            MainContent.SelectedItem?.MouseDown(e);
        }

        /// <summary>
        /// ウィンドウを閉じる時に呼ばれる
        /// </summary>
        public void Closing() {

            if (Settings.ShowExitConfirmDialog) {

                var message = new TransitionMessage(typeof(Views.ExitConfirmWindow), this, TransitionMode.Modal);

                // View側がメッセージを処理し終えるまでブロックされる
                Messenger.Raise(message);
            } else {
                CanClose = true;
            }

            if (CanClose) {
                App.UIDispatcher!.BeginInvoke(new Action(() => {
                    Messenger.Raise(new WindowActionMessage(WindowAction.Close));
                }));
            }
        }
    }
}
