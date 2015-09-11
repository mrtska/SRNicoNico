using Livet;

using SRNicoNico.Models.NicoNicoWrapper;


namespace SRNicoNico.ViewModels {

	public class SearchResultEntryViewModel : ViewModel {
	
		//検索結果
		public NicoNicoSearchResultNode Node { get; private set; }

        public SearchResultEntryViewModel(NicoNicoSearchResultNode node) {

            Node = node;
        }

	}
}
