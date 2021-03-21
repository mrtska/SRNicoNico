using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ニコレポの「コミュニティ」タブ
    /// </summary>
    public class NicoRepoListCommunityViewModel : NicoRepoListViewModel {

        /// <inheritdoc />     
        public override NicoRepoType NicoRepoType => NicoRepoType.Community;

        public NicoRepoListCommunityViewModel(INicoRepoService nicorepoService) : base(nicorepoService, "コミュニティ") {
        }
    }
}
