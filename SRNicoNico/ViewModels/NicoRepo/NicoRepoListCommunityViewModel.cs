using System.Collections.Generic;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ニコレポの「コミュニティ」タブ
    /// </summary>
    public class NicoRepoListCommunityViewModel : NicoRepoListViewModel {

        /// <inheritdoc />     
        public override NicoRepoType NicoRepoType => NicoRepoType.Community;

        /// <inheritdoc />
        public override IEnumerable<NicoRepoFilter> FilterItems => FilterAll;

        public NicoRepoListCommunityViewModel(INicoRepoService nicorepoService) : base(nicorepoService, "コミュニティ") {
        }
    }
}
