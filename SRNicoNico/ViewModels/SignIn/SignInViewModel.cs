using System;
using System.Threading.Tasks;
using System.IO;

using Livet;
using Livet.Messaging;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Views.Contents.Signin;

namespace SRNicoNico.ViewModels {
	public class SignInViewModel : ViewModel {
		
		#region MailAddress変更通知プロパティ
		private string _MailAddress;

		public string MailAddress {
			get { return _MailAddress; }
			set { 
				if(_MailAddress == value)
					return;
				_MailAddress = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region Password変更通知プロパティ
		private string _Password;

		public string Password {
			get { return _Password; }
			set { 
				if(_Password == value)
					return;
				_Password = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region Enabled変更通知プロパティ
		private bool _Enabled;

		public bool Enabled {
			get { return _Enabled; }
			set { 
				if(_Enabled == value)
					return;
				_Enabled = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region StateText変更通知プロパティ
		private string _StateText;

		public string StateText {
			get { return _StateText; }
			set { 
				if(_StateText == value)
					return;
				_StateText = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region AutoLogin変更通知プロパティ
		private bool _AutoLogin;

		public bool AutoLogin {
			get { return _AutoLogin; }
			set { 
				if(_AutoLogin == value)
					return;
				_AutoLogin = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		private bool Success = false;

		//サインイン
		public async void SignIn() {

			StateText = "ログイン中・・・";
			Enabled = false;

			var status = await NicoNicoWrapperMain.Session.SignInAsync(MailAddress, Password);

			//サインイン失敗
			if(status != SigninStatus.Success) {

				StateText = "ログインに失敗しました。";
				Enabled = true;
				return;
			}

			Success = true;
			Messenger.Raise(new WindowActionMessage(WindowAction.Close, "WindowAction"));
		}

		public void Close() {

			//ログイン成功
			if(Success) {

				//自動的にログインするにチェックが入っていたら
				if(AutoLogin) {

					StreamWriter writer = new StreamWriter(NicoNicoUtil.CurrentDirectory + @"\session");

					writer.WriteLine("Key:" + NicoNicoWrapperMain.Session.Key);
					writer.WriteLine("Expire:" + NicoNicoWrapperMain.Session.Expire);

					writer.Flush();
					writer.Close();
				}
			} else {

				Environment.Exit(0);
			}
		}

		//初期化
		public void Initialize() {

			Enabled = true;
		}


        public void TwitterSignin() {



            Messenger.Raise(new TransitionMessage(typeof(TwitterSignInDialog), new TwitterSignInViewModel(), TransitionMode.Modal));
        }

	}
}
