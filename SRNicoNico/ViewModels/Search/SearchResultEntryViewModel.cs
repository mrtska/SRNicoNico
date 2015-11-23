using Livet;

using SRNicoNico.Models.NicoNicoWrapper;


namespace SRNicoNico.ViewModels {

	public class SearchResultEntryViewModel : ViewModel {
	
		//検索結果
		public NicoNicoVideoInfoEntry Node { get; private set; }

        public SearchResultEntryViewModel(NicoNicoVideoInfoEntry node) {

            Node = node;
        }
	}
}
