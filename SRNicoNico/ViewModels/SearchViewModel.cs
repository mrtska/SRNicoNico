using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using System.Windows.Controls;

using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
	public class SearchViewModel : ViewModel {
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

		private string[] sort_by = { "f:d", "f:a",
									 "v:d", "v:a" };

		#region searchText変更通知プロパティ
		private string _searchText;

		public string searchText {
			get { return _searchText; }
			set { 
				if(_searchText == value)
					return;
				_searchText = value;
				RaisePropertyChanged();
			}
		}
		#endregion


		#region SelectedIndex変更通知プロパティ
		private int _SelectedIndex = 2;

		public int SelectedIndex {
			get { return _SelectedIndex; }
			set { 
				if(_SelectedIndex == value)
					return;
				_SelectedIndex = value;
				RaisePropertyChanged();
			}
		}
		#endregion



		public void Initialize() {

			
		}

		private NicoNicoSearch currentSearch;

		//検索ボタン押下
		public void DoSearch() {


			Task.Run(new Action(() => {

				App.ViewModelRoot.Content = App.ViewModelRoot.SearchResult;

				Console.WriteLine("検索キーワード:" + this.SelectedIndex);

				if(this.searchText == null || this.searchText.Length == 0) {

					return;
				}

				//検索
				this.currentSearch = new NicoNicoSearch(this.searchText, this.sort_by[this.SelectedIndex]);

				//検索結果をUIに
				NicoNicoSearchResult result = NicoNicoSearchResult.Deserialize(this.currentSearch.Response());


				DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

					App.ViewModelRoot.SearchResult.Total = String.Format("{0:#,0}", result.Total) + "件の動画";

					App.ViewModelRoot.SearchResult.List.Clear();
					foreach(NicoNicoSearchResultNode node in result.List) {

						SearchResultEntryViewModel vm = new SearchResultEntryViewModel();
						vm.Node = node;
						App.ViewModelRoot.SearchResult.List.Add(vm);
					}
				}));
			}));
		}

		public void SearchNext() {


			Task.Run(new Action(() => {

				//検索結果をUIに
				NicoNicoSearchResult result = NicoNicoSearchResult.Deserialize(this.currentSearch.Next());

				DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

					foreach(NicoNicoSearchResultNode node in result.List) {

						SearchResultEntryViewModel vm = new SearchResultEntryViewModel();
						vm.Node = node;
						App.ViewModelRoot.SearchResult.List.Add(vm);
					}
				}));


			}));

			
			


			

			;

			

		}


	}
}
