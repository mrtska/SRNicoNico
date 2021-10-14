using Livet;
using Livet.Commands;
using Livet.Messaging;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class SearchViewModel : PageSpinnerViewModel {

        public NicoNicoSearch Model { get; set; }

        //ソート方法
        private readonly string[] sortBy = { "sortKey=registeredAt&sortOrder=desc", "sortKey=registeredAt&sortOrder=asc",
                                     "sortKey=viewCount&sortOrder=desc", "sortKey=viewCount&sortOrder=asc",
                                     "sortKey=commentCount&sortOrder=desc", "sortKey=commentCount&sortOrder=asc",
                                     "sortKey=mylistCount&sortOrder=desc", "sortKey=mylistCount&sortOrder=asc",
                                     "sortKey=duration&sortOrder=desc", "sortKey=duration&sortOrder=asc"
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

        private string PreviousSearchText;

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

        #region SearchHistory変更通知プロパティ
        private ObservableSynchronizedCollection<string> _SearchHistory = new ObservableSynchronizedCollection<string>();

        public ObservableSynchronizedCollection<string> SearchHistory {
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

            //検索
            Model = new NicoNicoSearch();

            foreach(var entry in Settings.Instance.SearchHistory) {

                SearchHistory.Add(entry);
            }
        }

        public void Search() {

            if(SearchText == null || SearchText.Length == 0) {

                return;
            }

            Status = "検索中:" + SearchText;

            PreviousSearchText = SearchText;
            CurrentPage = 1;
            Search(SearchText);
        }

        //TextBoxBehaviorで使うのでアレ
        public void Search(string tex) {

            Search(tex, 1);
        }

        //検索ボタン押下
        public async void Search(string text, int page = 1) {

            //ページ数がおかしい場合は一番ギリギリセーフな値になるようバリデートする
            if(page != 1 && MaxPages < page) {

                page = MaxPages;
                CurrentPage = page;
            }

            //どうしてそれで検索出来ると思ったんだ
            if(text == null || text.Length == 0) {

                return;
            }
            IsActive = true;

            //履歴に存在しなかったら履歴に追加する
            if(!Settings.Instance.SearchHistory.Contains(text)) {

                //一番上に追加する
                SearchHistory.Insert(0, text);
            } else {

                //存在したら一番上に持ってくる
                SearchHistory.Move(SearchHistory.IndexOf(SearchHistory.Where(e => e == text).First()), 0);
            }
            ApplyHistory();

            Status  = await Model.Search(text, SearchType, sortBy[Settings.Instance.SearchIndex], page);

            //検索結果をUIに
            Total = Model.Total;
            MaxPages = Total / 30;

            if(Total == 0) {

                Status = "0件です";
            }

            IsActive = false;
        }

        public void SearchPage(string page) {

            SearchPage(int.Parse(page));
        }

        public void SearchPage(int page) {

            Search(PreviousSearchText, page);
        }

        public void SearchWithHistory(string tex) {

            SearchText = tex;
            Search();
        }

        #region DeleteHistoryCommand
        private ListenerCommand<string> _DeleteHistoryCommand;

        public ListenerCommand<string> DeleteHistoryCommand {
            get {
                if(_DeleteHistoryCommand == null) {
                    _DeleteHistoryCommand = new ListenerCommand<string>(DeleteHistory);
                }
                return _DeleteHistoryCommand;
            }
        }
        public void DeleteHistory(string vm) {

            SearchHistory.Remove(vm);
            ApplyHistory();
        }
        #endregion

        private void ApplyHistory() {

            var ret = new List<string>();
            foreach(var s in SearchHistory) {

                ret.Add(s);
            }
            Settings.Instance.SearchHistory = ret;
        }

        public void MakePlayList() {

            var filteredList = Model.SearchResult;

            if(filteredList.Count() == 0) {

                Status = "連続再生できる動画がありません";
                return;
            }

            var vm = new PlayListViewModel(SearchText, filteredList);
            App.ViewModelRoot.MainContent.AddUserTab(vm);
        }

        public override void KeyDown(KeyEventArgs e) {

            //TextBoxにフォーカスが合ったら無視する
            if(TextBoxFocused) {
                return;
            }
            if(e.Key == Key.Right) {

                RightButtonClick();
            }
            if(e.Key == Key.Left) {

                LeftButtonClick();
            }
        }

        public override void SpinPage() {

            SearchPage(CurrentPage);
        }
        public override bool CanShowHelp() {
            return true;
        }

        public override void ShowHelpView(InteractionMessenger Messenger) {

            Messenger.Raise(new TransitionMessage(typeof(Views.SearchHelpView), this, TransitionMode.NewOrActive));
        }
    }
}
