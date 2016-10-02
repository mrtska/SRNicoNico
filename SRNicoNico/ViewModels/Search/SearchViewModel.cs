using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Threading.Tasks;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using System.Windows.Controls;


using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Models.NicoNicoViewer;
using System.Windows.Media.Imaging;
using System.Net.Cache;

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
                if (_SearchText == value)
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
                


            if(page != 1 && SearchResult.MaxPages < page) {

                page = SearchResult.MaxPages;
                SearchResult.CurrentPage = page;
            }

			if (text == null || text.Length == 0) {

				return;
			}



            SearchResult.IsActive = true;

            if(!Settings.Instance.SearchHistory.Contains(text)) {

                SearchHistory.Add(new SearchHistoryViewModel(this, text));
                ApplyHistory();
            }
            var result = await CurrentSearch.Search(text, SearchType, sortBy[Settings.Instance.SearchIndex], page);

            if(result == null) {

                SearchResult.IsActive = false;
                return;
            }

            //検索結果をUIに
            SearchResult.Total = string.Format("{0:#,0}", result.Total) + "件の動画";
            SearchResult.MaxPages = result.Total / 30;

            SearchResult.List.Clear();
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

            //What's?!
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





    }

}
