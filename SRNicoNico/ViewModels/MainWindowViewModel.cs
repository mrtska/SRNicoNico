using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.IO;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Views.Contents.Search;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows;
using System.Collections.ObjectModel;
using SRNicoNico.Views.Contents.SignIn;
using System.Windows.Input;
using System.Windows.Media;
using SRNicoNico.Models.NicoNicoViewer;
using System.Reflection;
using SRNicoNico.Views.Contents.Misc;

namespace SRNicoNico.ViewModels {
	public class MainWindowViewModel : ViewModel {

        //現在のバージョン
        public double CurrentVersion {

            get { return 0.8; }
        }

        
		#region Title変更通知プロパティ
		private string _Title = "NicoNicoViewer ";

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

        #region WindowState変更通知プロパティ
        private WindowState _WindowState;

        public WindowState WindowState {
            get { return _WindowState; }
            set { 
                if(_WindowState == value)
                    return;
                _WindowState = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        

        #region Visibility変更通知プロパティ
        private Visibility _Visibility;

        public Visibility Visibility {
            get { return _Visibility; }
            set { 
                if(_Visibility == value)
                    return;
                _Visibility = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        public NicoNicoUserEntry User { get; private set; }

        public SignInViewModel SignIn { get; private set; }

        public StatusBarViewModel StatusBar { get; private set; }

        public SearchViewModel Search { get; private set; }

        public NotifyLiveViewModel NotifyLive { get; private set; }

        public ConfigViewModel Config { get; private set; }

        public AccessLogViewModel AccessLog { get; private set; }

        public UpdateViewModel Update { get; private set; }

        public string Status {
            set {
                    StatusBar.Status = value;
            }
            get {
                return StatusBar.Status;
            }
        }

        public DispatcherCollection<TabItemViewModel> TabItems { get; set; }


        #region VideoTabs変更通知プロパティ
        private DispatcherCollection<TabItemViewModel> _VideoTabs;

        public DispatcherCollection<TabItemViewModel> VideoTabs {
            get { return _VideoTabs; }
            set { 
                if(_VideoTabs == value)
                    return;
                _VideoTabs = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region UserTabs変更通知プロパティ
        private DispatcherCollection<TabItemViewModel> _UserTabs;

        public DispatcherCollection<TabItemViewModel> UserTabs {
            get { return _UserTabs; }
            set { 
                if(_UserTabs == value)
                    return;
                _UserTabs = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region MylistTabs変更通知プロパティ
        private DispatcherCollection<TabItemViewModel> _MylistTabs;

        public DispatcherCollection<TabItemViewModel> MylistTabs {
            get { return _MylistTabs; }
            set { 
                if(_MylistTabs == value)
                    return;
                _MylistTabs = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CommunityTabs変更通知プロパティ
        private DispatcherCollection<TabItemViewModel> _CommunityTabs;

        public DispatcherCollection<TabItemViewModel> CommunityTabs {
            get { return _CommunityTabs; }
            set { 
                if(_CommunityTabs == value)
                    return;
                _CommunityTabs = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region PlayListTabs変更通知プロパティ
        private DispatcherCollection<TabItemViewModel> _PlayListTabs;

        public DispatcherCollection<TabItemViewModel> PlayListTabs {
            get { return _PlayListTabs; }
            set {
                if(_PlayListTabs == value)
                    return;
                _PlayListTabs = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region LiveTabs変更通知プロパティ
        private DispatcherCollection<TabItemViewModel> _LiveTabs;

        public DispatcherCollection<TabItemViewModel> LiveTabs {
            get { return _LiveTabs; }
            set { 
                if(_LiveTabs == value)
                    return;
                _LiveTabs = value;
                RaisePropertyChanged();
            }
        }
        #endregion




        #region SelectedTab変更通知プロパティ
        private TabItemViewModel _SelectedTab;

        public TabItemViewModel SelectedTab {
            get { return _SelectedTab; }
            set { 
                if(_SelectedTab == value)
                    return;
                _SelectedTab = value;
                RaisePropertyChanged();
                if(value != null) {

                    Status = value.Status;
                }
            }
        }
        #endregion


        public NicoNicoNGComment NGCommentInstance;


        public MainWindowViewModel() {

            StatusBar = new StatusBarViewModel();
            StatusBar.TimerStart();

            SignIn = new SignInViewModel();

            TabItems = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher) {

                (SelectedTab = new StartViewModel())
            };

            VideoTabs = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);
            UserTabs = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);
            MylistTabs = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);
            CommunityTabs = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);
            LiveTabs = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);
            PlayListTabs = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);


            AccessLog = new AccessLogViewModel();

            Update = new UpdateViewModel(CurrentVersion);

		}

        //ログイン後の初期化処理
        public void LogedInInit() {

            User = new NicoNicoUserEntry();
            User.UserId = NicoNicoWrapperMain.Session.UserId;
            User.UserName = NicoNicoUser.LookupUserName(User.UserId);

            App.ViewModelRoot.Title += "(user:" + User.UserName + ")";

            TabItems.Add(new WebViewViewModel());
            TabItems.Add(new RankingViewModel());
            TabItems.Add(Search = new SearchViewModel());
            TabItems.Add(new FavoriteViewModel());
            TabItems.Add(new NicoRepoViewModel());
            TabItems.Add(new MylistViewModel());
            TabItems.Add(NotifyLive = new NotifyLiveViewModel());
            TabItems.Add(new HistoryViewModel());
            TabItems.Add(new OtherViewModel());
            TabItems.Add(Config = new ConfigViewModel());

            //生放送通知を更新するタイマーを動かす
            StatusBar.StartRefreshTimer();

            //サーバーからアップデートがあるか確認してあったらアップデートダイアログを出す
            Update.CheckUpdate();

            //公式NG機能を初期化
            NGCommentInstance = new NicoNicoNGComment();
            //NGCommentInstance.GetNGClient();

            if(Environment.GetCommandLineArgs().Length > 1) {

                Task.Run(() => NicoNicoOpener.Open(Environment.GetCommandLineArgs()[1]));
            }
            
    
        }

		public void Initialize() {
            
			//Modelsを初期化
			Task.Run(() => {

				if(File.Exists(NicoNicoUtil.CurrentDirectory + @"\session")) {

                    StatusBar.Status = "自動ログイン中";

					//セッション情報を取得する
					var reader = new StreamReader(NicoNicoUtil.CurrentDirectory + @"\session");
							
					var key = reader.ReadLine().Split(':')[1];
					var expire = DateTimeOffset.Parse(reader.ReadLine().Replace("Expire:", ""));

					reader.Close();

					//セッションが有効か比較する
					if(DateTimeOffset.Compare(expire, DateTimeOffset.Now) < 0) {

						//セッションが有効期限切れ
						SignIn.StateText = "有効期限が切れています。\n再度ログインしてください。";
						SignIn.AutoLogin = true;
                        NicoNicoWrapperMain.Instance = new NicoNicoWrapperMain(new NicoNicoSession());

                        //ログインダイアログ表示
                        Messenger.Raise(new TransitionMessage(typeof(SignInDialog), SignIn, TransitionMode.Modal));
						return;
					}

					//セッションが有効だった場合
					NicoNicoWrapperMain.Instance = new NicoNicoWrapperMain(new NicoNicoSession(key, expire));
					if(NicoNicoWrapperMain.Session.SignInInternal() != SigninStatus.Success) {
                   
						//ログイン失敗
						SignIn.StateText = "ログインに失敗しました。";
						SignIn.AutoLogin = true;

						//ログインダイアログ表示
						Messenger.Raise(new TransitionMessage(typeof(SignInDialog), SignIn, TransitionMode.Modal));
						return;
					}

                    //セッション情報を更新
                    var writer = new StreamWriter(NicoNicoUtil.CurrentDirectory + @"\session");

                    writer.WriteLine("Key:" + NicoNicoWrapperMain.Session.Key);
                    writer.WriteLine("Expire:" + NicoNicoWrapperMain.Session.Expire);

                    writer.Flush();
                    writer.Close();
                    //ログイン成功
                    StatusBar.Status = "ログイン完了";
				//手動ログイン
				} else {
					//セッションを確立
					NicoNicoWrapperMain.Instance = new NicoNicoWrapperMain(new NicoNicoSession());
					Messenger.Raise(new TransitionMessage(typeof(SignInDialog), SignIn, TransitionMode.Modal));
					return;
				}
			});
		}

		//終了処理
		protected override void Dispose(bool disposing) {

			if(disposing) {

                NicoNicoWrapperMain.Session.Dispose();
			}
			base.Dispose(disposing);
		}

        public void KeyDown(KeyEventArgs e) {

            SelectedTab?.KeyDown(e);
        }

        public void SearchText(SearchType type, string text) {

            Search.SearchType = type;
            Search.SearchText = text;
            Search.DoSearch();
            SelectedTab = Search;

        }

        public void RemoveTab(TabItemViewModel vm) {

            if(vm is VideoViewModel) {

                VideoTabs.Remove(vm);
            } else if(vm is UserViewModel) {

                UserTabs.Remove(vm);
            } else if(vm is PublicMylistViewModel) {

                MylistTabs.Remove(vm);
            } else if(vm is CommunityViewModel) {

                CommunityTabs.Remove(vm);
            } else if(vm is LiveViewModel) {

                LiveTabs.Remove(vm);
            } else if(vm is PlayListViewModel) {

                PlayListTabs.Remove(vm);
            } else {

                TabItems.Remove(vm);
            }
        }

        public void AddTab(TabItemViewModel vm) {

            if(vm is VideoViewModel) {

                VideoTabs.Add(vm);
            } else if(vm is UserViewModel) {

                UserTabs.Add(vm);
            } else if(vm is PublicMylistViewModel) {

                MylistTabs.Add(vm);
            } else if(vm is CommunityViewModel) {

                CommunityTabs.Add(vm);
            } else if(vm is LiveViewModel) {

                LiveTabs.Add(vm);
            } else if(vm is PlayListViewModel) {

                PlayListTabs.Add(vm);
            } else {

                TabItems.Add(vm);
            }
        }

        public void ReplaceTab(TabItemViewModel old, TabItemViewModel current) {

            if(VideoTabs.Contains(old)) {

                VideoTabs[VideoTabs.IndexOf(old)] = current;
            } else if(LiveTabs.Contains(old)) {

                LiveTabs.Insert(LiveTabs.IndexOf(old), current);
            }
        }

        public void ReplaceTabAndSetCurrent(TabItemViewModel old, TabItemViewModel current) {

            ReplaceTab(old, current);
            SelectedTab = current;
        }

        public void SetCurrent(TabItemViewModel vm) {

            SelectedTab = vm;
        }

        public void RemoveTabAndLastSet(TabItemViewModel vm) {

            RemoveTab(vm);
            if(vm is VideoViewModel && VideoTabs.Count > 0) {

                SelectedTab = VideoTabs.FirstOrDefault();
            } else if(vm is UserViewModel && UserTabs.Count > 0) {

                SelectedTab = UserTabs.FirstOrDefault();
            } else if(vm is PublicMylistViewModel && MylistTabs.Count > 0) {

                SelectedTab = MylistTabs.FirstOrDefault();
            } else if(vm is CommunityViewModel && CommunityTabs.Count > 0) {

                SelectedTab = CommunityTabs.FirstOrDefault();
            } else if(vm is LiveViewModel && LiveTabs.Count > 0) {

                SelectedTab = LiveTabs.FirstOrDefault();
            } else if(vm is PlayListViewModel && PlayListTabs.Count > 0) {

                SelectedTab = PlayListTabs.FirstOrDefault();
            } else {

                SelectedTab = TabItems.Last();
            }

        }

        public void AddTabAndSetCurrent(TabItemViewModel vm) {

            AddTab(vm);
            SelectedTab = vm;
        }



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



        public void Closing() {

            if(!Settings.Instance.ConfirmExit) {

                CanClose = true;
                DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {
                    Messenger.Raise(new WindowActionMessage(WindowAction.Close, "WindowAction"));
                }));
                return;
            }

            var message = new TransitionMessage(typeof(ExitDialog), this, TransitionMode.Modal);

            // View側がメッセージを処理し終えるまでブロックされる
            Messenger.Raise(message);

        }

        public void YesClose() {

            CanClose = true;
            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "WindowAction"));


            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {
                Messenger.Raise(new WindowActionMessage(WindowAction.Close, "WindowAction"));
            }));
        }

        public void NoClose() {

            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "WindowAction"));
        }

    }
}
