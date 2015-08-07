using Livet;

using SRNicoNico.Models.NicoNicoWrapper;


namespace SRNicoNico.ViewModels {

	public class SearchResultEntryViewModel : ViewModel {
	
		//検索結果
		public NicoNicoSearchResultNode Node { get; set; }

		//動画を開く
		public void OpenVideo() {

			App.ViewModelRoot.Video.Stream = new NicoNicoStream();
			App.ViewModelRoot.Video.Stream.OpenVideo(Node);

		}
	}
}
