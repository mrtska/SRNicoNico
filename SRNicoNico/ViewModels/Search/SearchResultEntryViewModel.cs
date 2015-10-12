using Livet;

using SRNicoNico.Models.NicoNicoWrapper;


namespace SRNicoNico.ViewModels {

	public class SearchResultEntryViewModel : ViewModel {
	
		//検索結果
		public NicoNicoSearchResultEntry Node { get; private set; }

        public SearchResultEntryViewModel(NicoNicoSearchResultEntry node) {

            Node = node;
        }
	}
}
