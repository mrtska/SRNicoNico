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

        public MainContentViewModel MainContent { get; private set; }

        public SignInViewModel SignIn { get; private set; }

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
        }

        /// <summary>
        /// MainWindowがロードされた時に呼ばれる
        /// </summary>
        public async void OnLoaded() {

            if (await SignIn.EnsureSignedInAsync()) {

                // 正常にサインイン出来たらサインイン後に使用できるタブを追加する
                MainContent.PostInitialize();
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
