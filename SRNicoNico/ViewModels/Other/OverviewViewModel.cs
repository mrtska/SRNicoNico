using Unity;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// その他ページのアプリ説明タブのViewModel
    /// </summary>
    public class OverviewViewModel : TabItemViewModel {

        public OverviewViewModel() : base("このソフトウェアについて") {

        }

        /// <summary>
        /// NicoNicoViewerのアップデートを確認する
        /// </summary>
        public void CheckUpdate() {

        }
    }
}
