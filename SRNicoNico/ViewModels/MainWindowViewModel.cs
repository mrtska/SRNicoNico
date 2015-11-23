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

namespace SRNicoNico.ViewModels {
	public class MainWindowViewModel : ViewModel {

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


        #region UserSelectedFont変更通知プロパティ
        public FontFamily UserSelectedFont {
            get { return Properties.Settings.Default.UserSelectedFont; }
            set { 
                if(Properties.Settings.Default.UserSelectedFont == value)
                    return;
                Properties.Settings.Default.UserSelectedFont = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }
        #endregion

        public void ApplyFont() {

            RaisePropertyChanged("UserSelectedFont");
        }

        
        public NicoNicoUserEntry User { get; private set; }

        public SignInViewModel SignIn { get; private set; }

        public StatusBarViewModel StatusBar { get; private set; }

        public SearchViewModel Search { get; private set; }

        public ConfigViewModel Config { get; private set; }

        public AccessLogViewModel AccessLog { get; private set; }

        public string Status {
            set {
                    StatusBar.Status = value;
            }
            get {
                return StatusBar.Status;
            }
        }

        public DispatcherCollection<TabItemViewModel> TabItems { get; set; }


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

        public MainWindowViewModel() {

            StatusBar = new StatusBarViewModel();
            StatusBar.TimerStart();

            SignIn = new SignInViewModel();

            TabItems = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher) {

                (SelectedTab = new StartViewModel())
            };

            AccessLog = new AccessLogViewModel();

		}

        //ログイン後の初期化処理
        public void LogedInInit() {

            User = new NicoNicoUserEntry();
            User.UserId = NicoNicoWrapperMain.GetSession().UserId;
            User.UserName = NicoNicoUser.LookupUserName(User.UserId);

            App.ViewModelRoot.Title += "(user:" + User.UserName + ")";

            TabItems.Add(Search = new SearchViewModel());
            TabItems.Add(new FavoriteViewModel());
            TabItems.Add(new NicoRepoViewModel());
            TabItems.Add(new MylistViewModel());
            TabItems.Add(new HistoryViewModel());
            TabItems.Add(new OtherViewModel());
            TabItems.Add(Config = new ConfigViewModel());

        }

		public void Initialize() {
            
			//Modelsを初期化
			Task.Run(() => {



				if(File.Exists(NicoNicoUtil.CurrentDirectory.DirectoryName + @"\session")) {

                    StatusBar.Status = "自動ログイン中";

					//セッション情報を取得する
					StreamReader reader = new StreamReader(NicoNicoUtil.CurrentDirectory.DirectoryName + @"\session");
							
					string key = reader.ReadLine().Split(':')[1];
					DateTimeOffset expire = DateTimeOffset.Parse(reader.ReadLine().Replace("Expire:", ""));

					reader.Close();

					//セッションが有効か比較する
					if(DateTimeOffset.Compare(expire, DateTimeOffset.Now) < 0) {

						//セッションが有効期限切れ
						this.SignIn.StateText = "有効期限が切れています。\n再度ログインしてください。";
						this.SignIn.AutoLogin = true;

						//ログインダイアログ表示
						Messenger.Raise(new TransitionMessage(typeof(SignInDialog), this.SignIn, TransitionMode.Modal));
						return;
					}

					//セッションが有効だった場合
					NicoNicoWrapperMain.Instance = new NicoNicoWrapperMain(new NicoNicoSession(key, expire));
					if(NicoNicoWrapperMain.Session.SignInInternal() != SigninStatus.Success) {

						//ログイン失敗
						this.SignIn.StateText = "ログインに失敗しました。";
						this.SignIn.AutoLogin = true;

						//ログインダイアログ表示
						Messenger.Raise(new TransitionMessage(typeof(SignInDialog), SignIn, TransitionMode.Modal));
						return;
					}

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

                NicoNicoWrapperMain.GetSession().Dispose();
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


    }
}
