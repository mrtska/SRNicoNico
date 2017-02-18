using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class SearchViewModel : TabItemViewModel {

        private NicoNicoSearch CurrentSearch;


        //ソート方法
        private string[] sortBy = { "f:d", "f:a",
                                     "v:d", "v:a",
                                     "n:d", "n:a",
                                     "m:d", "m:a",
                                     "l:d", "l:a"
                                    };

        #region SearchText変更通知プロパティ
        private string _SearchText;

        public string SearchText {
            get { return _SearchText; }
            set {
                if(_SearchText == value)
                    return;
                _SearchText = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SearchType変更通知プロパティ
        private SearchType _SearchType;

        public SearchType SearchType {
            get { return _SearchType; }
            set {
                if(_SearchType == value)
                    return;
                _SearchType = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SearchResult変更通知プロパティ
        private SearchResultViewModel _SearchResult;

        public SearchResultViewModel SearchResult {
            get { return _SearchResult; }
            set {
                if(_SearchResult == value)
                    return;
                _SearchResult = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SearchHistory変更通知プロパティ
        private DispatcherCollection<SearchHistoryViewModel> _SearchHistory = new DispatcherCollection<SearchHistoryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<SearchHistoryViewModel> SearchHistory {
            get { return _SearchHistory; }
            set {
                if(_SearchHistory == value)
                    return;
                _SearchHistory = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public bool TextBoxFocused { get; set; }


        public SearchViewModel() : base("検索") {

            SearchResult = new SearchResultViewModel(this);

            //検索
            CurrentSearch = new NicoNicoSearch(this);

            foreach(var entry in Settings.Instance.SearchHistory) {

                SearchHistory.Add(new SearchHistoryViewModel(this, entry));
            }
        }

        public void Search() {

            if(SearchText == null || SearchText.Length == 0) {

                return;
            }

            SearchResult.CurrentPage = 1;
            Search(SearchText);
        }

        //TextBoxBehaviorで使うのでアレ
        public void Search(string tex) {

            Search(tex, 1);
        }

        //検索ボタン押下
        public async void Search(string text, int page = 1) {

            //ページ数がおかしい場合は一番ギリギリセーフな値にバリデートする
            if(page != 1 && SearchResult.MaxPages < page) {

                page = SearchResult.MaxPages;
                SearchResult.CurrentPage = page;
            }

            //どうしてそれで検索出来ると思ったんだ
            if(text == null || text.Length == 0) {

                return;
            }

            SearchResult.List.Clear();

            SearchResult.IsActive = true;

            //履歴に存在しなかったら履歴に追加する
            if(!Settings.Instance.SearchHistory.Contains(text)) {

                //一番上に追加する
                SearchHistory.Insert(0, new SearchHistoryViewModel(this, text));
            } else {

                //存在したら一番上に持ってくる
                SearchHistory.Move(SearchHistory.IndexOf(SearchHistory.Where(e => e.Content == text).First()), 0);
            }
            ApplyHistory();

            var result = await CurrentSearch.Search(text, SearchType, sortBy[Settings.Instance.SearchIndex], page);

            if(result == null) {

                SearchResult.IsActive = false;
                return;
            }

            //検索結果をUIに
            SearchResult.Total = result.Total;
            SearchResult.MaxPages = result.Total / 30;

            if(SearchResult.Total == 0) {

                Status = "0件です";
            }

            foreach(var node in result.List) {

                SearchResult.List.Add(new SearchResultEntryViewModel(node));
            }

            SearchResult.IsActive = false;
        }

        public void SearchPage(string page) {

            SearchPage(int.Parse(page));
        }

        public void SearchPage(int page) {

            Search(SearchText, page);
        }

        public void SearchWithHistory(string tex) {

            SearchText = tex;
            Search();
        }

        public void SearchWithHistory() {

        }

        #region DeleteHistoryCommand
        private ListenerCommand<SearchHistoryViewModel> _DeleteHistoryCommand;

        public ListenerCommand<SearchHistoryViewModel> DeleteHistoryCommand {
            get {
                if(_DeleteHistoryCommand == null) {
                    _DeleteHistoryCommand = new ListenerCommand<SearchHistoryViewModel>(DeleteHistory);
                }
                return _DeleteHistoryCommand;
            }
        }
        public void DeleteHistory(SearchHistoryViewModel vm) {

            SearchHistory.Remove(vm);
            ApplyHistory();
        }
        #endregion

        private void ApplyHistory() {

            var ret = new List<string>();
            foreach(var s in SearchHistory) {

                ret.Add(s.Content);
            }

            Settings.Instance.SearchHistory = ret;
        }

        public override void KeyDown(KeyEventArgs e) {

            //TextBoxにフォーカスが合ったら無視する
            if(TextBoxFocused) {

                return;
            }

            if(e.Key == Key.Right) {

                SearchResult.RightButtonClick();
            }
            if(e.Key == Key.Left) {

                SearchResult.LeftButtonClick();
            }
        }

        public override bool CanShowHelp() {
            return true;
        }

        public override void ShowHelpView(InteractionMessenger Messenger) {

            Messenger.Raise(new TransitionMessage(typeof(Views.SearchHelpView), this, TransitionMode.NewOrActive));
        }

    }
}
