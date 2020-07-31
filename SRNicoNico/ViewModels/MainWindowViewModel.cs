using Livet;
using Livet.Messaging;
using Livet.Messaging.Windows;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Views.Service;
using System;
using System.Linq;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class MainWindowViewModel : ViewModel {

        //現在のバージョン
        public double CurrentVersion {
            
            get { return 1.47; }
        }

        #region Title変更通知プロパティ
#if DEBUG

        private string _Title = "NicoNicoViewer Debug Build ";
#else
        private string _Title = "NicoNicoViewer ";
#endif
        public string Title {
            get { return _Title; }
            set {
                if(_Title == value)
                    return;
                _Title = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Statusプロパティ
        public string Status {
            set {
                StatusBar.Status = value;
            }
            get {
                return StatusBar.Status;
            }
        }
        #endregion

        #region CanShowHelpView変更通知プロパティ
        private bool _CanShowHelpView;

        public bool CanShowHelpView {
            get { return _CanShowHelpView; }
            set { 
                if(_CanShowHelpView == value)
                    return;
                _CanShowHelpView = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CanClose変更通知プロパティ
        private bool _CanClose;

        public bool CanClose {
            get { return _CanClose; }
            set { 
                if(_CanClose == value)
                    return;
                _CanClose = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        //スタートページ
        public StartViewModel Start { get; private set; }

        //ステータスバーに関するやつ
        public StatusBarViewModel StatusBar { get; private set; }

        //サインイン関係
        public SignInViewModel SignIn { get; private set; }

        //メインコンテンツ
        public MainContentViewModel MainContent { get; private set; }

        //WebView
        public WebViewViewModel WebView { get; private set; }

        //Search
        public SearchViewModel Search { get; private set; }

        //Ranking
        public RankingViewModel Ranking { get; private set; }

        //視聴履歴
        public HistoryViewModel History { get; private set; }

        //視聴履歴
        public SettingsViewModel Setting { get; private set; }

        //生放送通知
        public LiveNotifyViewModel LiveNotify { get; private set; }

        //現在のユーザー
        public NicoNicoSessionUser CurrentUser { get; set; }

        #region UserList変更通知プロパティ
        private DispatcherCollection<NicoNicoSessionUser> _UserList = new DispatcherCollection<NicoNicoSessionUser>(DispatcherHelper.UIDispatcher);
        public DispatcherCollection<NicoNicoSessionUser> UserList {
            get { return _UserList; }
            set { 
                if(_UserList == value)
                    return;
                _UserList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public ThemeSelectorService ThemeSelector { get; private set; }

        public MainWindowViewModel() {

            ThemeSelector = new ThemeSelectorService();
            ThemeSelector.ChangeAccent((EnumAccents) Enum.Parse(typeof(EnumAccents), Settings.Instance.ThemeColor));
            ThemeSelector.ChangeTheme((EnumThemes) Enum.Parse(typeof(EnumThemes), Settings.Instance.Theme));

            //ステータスバーを初期化してタイマーをスタートさせる
            StatusBar = new StatusBarViewModel();
            StatusBar.TimerStart();

            MainContent = new MainContentViewModel(this);

            MainContent.AddSystemTab(Start = new StartViewModel());
            MainContent.SelectedTab = Start;
            //
            SignIn = new SignInViewModel();
            Status = "サインイン中";

        }

        public async void Initialize() {

            // 起動時にアップデートがあるか確認する
            if (await UpdateChecker.IsUpdateAvailable()) {

                Messenger.Raise(new TransitionMessage(typeof(Views.UpdateFoundView), new UpdaterViewModel(), TransitionMode.Modal));
            }

            //自動的にサインインする サインイン情報がなければ
            foreach (var user in await SignIn.AutoSignIn()) {

                UserList.Add(user);
            }

            //一番最初に来たユーザーがカレントになる
            CurrentUser = UserList.First();

            //セッションを保存
            SignIn.SaveSession(UserList.ToList(), CurrentUser);

            Status = "サインイン完了";

            MainContent.AddSystemTab(WebView = new WebViewViewModel());
            MainContent.AddSystemTab(Ranking = new RankingViewModel());
            MainContent.AddSystemTab(Search = new SearchViewModel());
            MainContent.AddSystemTab(new FollowViewModel());
            MainContent.AddSystemTab(new NicoRepoViewModel());
            MainContent.AddSystemTab(new MylistViewModel());
            MainContent.AddSystemTab(LiveNotify = new LiveNotifyViewModel());

            MainContent.AddSystemTab(History = new HistoryViewModel());
            MainContent.AddSystemTab(new OtherViewModel());
            MainContent.AddSystemTab(Setting = new SettingsViewModel());

#if DEBUG
            MainContent.AddSystemTab(new DeployViewModel());
#endif

            var args = Environment.GetCommandLineArgs();

            if(args.Length == 2) {

                var commandline = Environment.GetCommandLineArgs()[1];

                NicoNicoOpener.TryOpen(commandline);
            }
        }

        public void SetCurrent(TabItemViewModel vm) {

            MainContent.SelectedTab = vm;
        }
        //
        public void AddWebViewTab(string url, bool forceWebView) {

            WebView.AddTab(url, forceWebView);
            SetCurrent(WebView);
        }

        public void ShowHelpView() {

            MainContent?.SelectedTab?.ShowHelpView(Messenger);
        }

        public void KeyDown(KeyEventArgs e) {

#if DEBUG
            Console.WriteLine("KeyDown: " + e.Key + ", " + e.ImeProcessedKey);
#endif

            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.W) {

            }

            MainContent.SelectedTab?.KeyDown(e);
        }
        public void KeyUp(KeyEventArgs e) {

#if DEBUG
            Console.WriteLine("KeyUp: " + e.Key + ", " + e.ImeProcessedKey);
#endif
            MainContent.SelectedTab?.KeyUp(e);
        }

        public void MouseDown(MouseButtonEventArgs e) {

#if DEBUG
            Console.WriteLine("MouseDown: " + e.ChangedButton);
#endif
            MainContent.SelectedTab?.MouseDown(e);
        }

        public void Closing() {

            if(!Settings.Instance.ConfirmExit) {

                CanClose = true;
                DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {
                    Messenger.Raise(new WindowActionMessage(WindowAction.Close));
                }));
                return;
            }

            var message = new TransitionMessage(typeof(Views.ExitConfirmView), this, TransitionMode.Modal);

            // View側がメッセージを処理し終えるまでブロックされる
            Messenger.Raise(message);

            if(CanClose) {

                DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {
                    Messenger.Raise(new WindowActionMessage(WindowAction.Close));
                }));
            }
        }

        public void ExitCancel() {

            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Exit"));
        }

    }
}
