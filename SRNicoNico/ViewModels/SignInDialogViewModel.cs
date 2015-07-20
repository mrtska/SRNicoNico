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


using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.ViewModels;


namespace SRNicoNico.ViewModels {
	public class SignInDialogViewModel : ViewModel {
		/* コマンド、プロパティの定義にはそれぞれ 
		 * 
		 *  lvcom   : ViewModelCommand
		 *  lvcomn  : ViewModelCommand(CanExecute無)
		 *  llcom   : ListenerCommand(パラメータ有のコマンド)
		 *  llcomn  : ListenerCommand(パラメータ有のコマンド・CanExecute無)
		 *  lprop   : 変更通知プロパティ(.NET4.5ではlpropn)
		 *  
		 * を使用してください。
		 * 
		 * Modelが十分にリッチであるならコマンドにこだわる必要はありません。
		 * View側のコードビハインドを使用しないMVVMパターンの実装を行う場合でも、ViewModelにメソッドを定義し、
		 * LivetCallMethodActionなどから直接メソッドを呼び出してください。
		 * 
		 * ViewModelのコマンドを呼び出せるLivetのすべてのビヘイビア・トリガー・アクションは
		 * 同様に直接ViewModelのメソッドを呼び出し可能です。
		 */

		/* ViewModelからViewを操作したい場合は、View側のコードビハインド無で処理を行いたい場合は
		 * Messengerプロパティからメッセージ(各種InteractionMessage)を発信する事を検討してください。
		 */

		/* Modelからの変更通知などの各種イベントを受け取る場合は、PropertyChangedEventListenerや
		 * CollectionChangedEventListenerを使うと便利です。各種ListenerはViewModelに定義されている
		 * CompositeDisposableプロパティ(LivetCompositeDisposable型)に格納しておく事でイベント解放を容易に行えます。
		 * 
		 * ReactiveExtensionsなどを併用する場合は、ReactiveExtensionsのCompositeDisposableを
		 * ViewModelのCompositeDisposableプロパティに格納しておくのを推奨します。
		 * 
		 * LivetのWindowテンプレートではViewのウィンドウが閉じる際にDataContextDisposeActionが動作するようになっており、
		 * ViewModelのDisposeが呼ばれCompositeDisposableプロパティに格納されたすべてのIDisposable型のインスタンスが解放されます。
		 * 
		 * ViewModelを使いまわしたい時などは、ViewからDataContextDisposeActionを取り除くか、発動のタイミングをずらす事で対応可能です。
		 */

		/* UIDispatcherを操作する場合は、DispatcherHelperのメソッドを操作してください。
		 * UIDispatcher自体はApp.xaml.csでインスタンスを確保してあります。
		 * 
		 * LivetのViewModelではプロパティ変更通知(RaisePropertyChanged)やDispatcherCollectionを使ったコレクション変更通知は
		 * 自動的にUIDispatcher上での通知に変換されます。変更通知に際してUIDispatcherを操作する必要はありません。
		 */



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



		private bool Success  = false;


		//サインイン
		public void SignIn() {

			this.StateText = "ログイン中・・・";
			this.Enabled = false;


			Task.Run(new Action(() => {


				SigninStatus status = NicoNicoWrapperMain.getSession().SignIn(MailAddress, Password);

				//サインイン失敗
				if(status != SigninStatus.Success) {

					this.StateText = "ログインに失敗しました。";
					this.Enabled = true;
					return;
				}


				NicoNicoWrapperMain.Instance.init();
				


				this.Success = true;
				Messenger.Raise(new WindowActionMessage(WindowAction.Close, "WindowAction"));

			}));

		}


		public void Close() {


			//ログイン成功
			if(this.Success) {

				//自動的にログインするにチェックが入っていたら
				if(this.AutoLogin) {

					StreamWriter writer = new StreamWriter(NicoNicoUtil.CurrentDirectory.DirectoryName + @"\session");

					writer.WriteLine("Key:" + NicoNicoWrapperMain.getSession().Key);
					writer.WriteLine("Expire:" + NicoNicoWrapperMain.getSession().Expire);

					writer.Flush();
					writer.Close();
				}


			} else {

				Environment.Exit(0);
			}

			
		}

		//初期化
		public void Initialize() {

			this.Enabled = true;
		}
	}
}
