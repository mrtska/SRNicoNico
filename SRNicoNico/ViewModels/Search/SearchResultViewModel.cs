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
    public class SearchResultViewModel : PageSpinnerViewModel {



        //検索結果の総数
     
            


        #region List変更通知プロパティ
        private DispatcherCollection<SearchResultEntryViewModel> _List = new DispatcherCollection<SearchResultEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<SearchResultEntryViewModel> List {
            get { return _List; }
            set {
                if(_List == value)
                    return;
                _List = value;
                RaisePropertyChanged();

            }
        }
        #endregion

        public SearchViewModel Owner { get; private set; }

        public SearchResultViewModel(SearchViewModel vm) : base("") {

            Owner = vm;
        }
        

        public void MakePlayList() {

            var filteredList = List.Select(e => e.Item);

            if(filteredList.Count() == 0) {

               Owner.Status = "連続再生できる動画がありません";
                return;
            }

            var vm = new PlayListViewModel(Owner.SearchText, filteredList);
            App.ViewModelRoot.MainContent.AddUserTab(vm);
        }

        public override void SpinPage() {

            Owner.SearchPage(CurrentPage);
        }
    }
}
