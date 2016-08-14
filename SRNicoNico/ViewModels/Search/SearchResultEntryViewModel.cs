using Livet;

using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows;

namespace SRNicoNico.ViewModels {

	public class SearchResultEntryViewModel : ViewModel {
	
		//検索結果
		public NicoNicoVideoInfoEntry Node { get; private set; }

        public SearchResultEntryViewModel(NicoNicoVideoInfoEntry node) {

            Node = node;
        }


        public void OpenWebView() {

            App.ViewModelRoot.AddWebViewTab("http://www.nicovideo.jp/watch/" + Node.Cmsid, true);
        }

        public void CopyUrl() {

            Clipboard.SetText("http://www.nicovideo.jp/watch/" + Node.Cmsid);
        }

    }
}
