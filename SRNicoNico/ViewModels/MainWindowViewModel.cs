using System;
using System.Threading;
using System.Windows.Input;
using Livet;
using Unity;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// MainWindowクラスのDataContext
    /// </summary>
    public class MainWindowViewModel : ViewModel {

#if DEBUG
        private string _Title = "NicoNicoViewer Debug Build ";
#else
        private string _Title = "NicoNicoViewer";
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

        public MainContentViewModel MainContent { get; private set; }

        public SignInViewModel SignIn { get; private set; }

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

            if (await SignIn.EnsureSignedInAsync()) {

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

    }
}
