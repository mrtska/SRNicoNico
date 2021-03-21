using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ニコレポの「すべて」タブ
    /// </summary>
    public class NicoRepoListAllViewModel : NicoRepoListViewModel {

        /// <inheritdoc />     
        public override NicoRepoType NicoRepoType => NicoRepoType.All;

        public NicoRepoListAllViewModel(INicoRepoService nicorepoService) : base(nicorepoService, "すべて") {
        }
    }
}
