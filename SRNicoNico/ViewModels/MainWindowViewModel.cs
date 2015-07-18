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

using SRNicoNico.Views.Contents;
using SRNicoNico.Views.Contents.Search;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
	public class MainWindowViewModel : ViewModel {
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



		#region Content変更通知プロパティ
		private ViewModel _Content;

		public ViewModel Content {
			get { return _Content; }
			set {
				if(_Content == value)
					return;
				_Content = value;
				RaisePropertyChanged();
			}
		}
		#endregion



		public SignInDialogViewModel signin { get; private set; }

		public SearchViewModel search { get; private set; }

		public SearchResultViewModel searchResult { get; private set; }

		public VideoViewModel video { get; private set; }

		public MainWindowViewModel() {

			this.signin = new SignInDialogViewModel();

			this.search = new SearchViewModel();

			this.searchResult = new SearchResultViewModel();

			this.video = new VideoViewModel();

			this.Content = new StartViewModel();
		}


		public void Initialize() {


			//Modelsを初期化
			Task.Run(() => {

				NicoNicoWrapperMain.instance = new NicoNicoWrapperMain();


				if(File.Exists(NicoNicoUtil.currentDirectory.DirectoryName + @"\session")) {

					//セッション情報を取得する
					StreamReader reader = new StreamReader(NicoNicoUtil.currentDirectory.DirectoryName + @"\session");
							
					string key = reader.ReadLine().Split(':')[1];
					DateTimeOffset expire = DateTimeOffset.Parse(reader.ReadLine().Replace("Expire:", ""));

					reader.Close();

					//セッションが有効か比較する
					if(DateTimeOffset.Compare(expire, DateTimeOffset.Now) < 0) {

						//セッションが有効期限切れ
						this.signin.StateText = "有効期限が切れています。\n再度ログインしてください。";
						this.signin.AutoLogin = true;

						//ログインダイアログ表示
						Messenger.Raise(new TransitionMessage(typeof(Views.SignInDialog), this.signin, TransitionMode.Modal));
						return;
					}


					//セッションが有効だった場合
					NicoNicoWrapperMain.instance.session = new NicoNicoSession(key, expire);
					if(NicoNicoWrapperMain.instance.session.SignInInternal() != SigninStatus.Success) {

						//ログイン失敗
						this.signin.StateText = "ログインに失敗しました。";
						this.signin.AutoLogin = true;

						//ログインダイアログ表示
						Messenger.Raise(new TransitionMessage(typeof(Views.SignInDialog), this.signin, TransitionMode.Modal));
						return;
					}

					//ログイン成功
					NicoNicoWrapperMain.instance.init();



				//手動ログイン
				} else {

					//セッションを確立
					NicoNicoWrapperMain.instance.session = new NicoNicoSession();
					Messenger.Raise(new TransitionMessage(typeof(Views.SignInDialog), this.signin, TransitionMode.Modal));
					return;
				}

			});
		}


	}
}
