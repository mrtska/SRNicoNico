using System.Collections.Generic;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ニコレポの「自分」タブ
    /// </summary>
    public class NicoRepoListSelfViewModel : NicoRepoListViewModel {

        /// <inheritdoc />     
        public override NicoRepoType NicoRepoType => NicoRepoType.Self;

        /// <inheritdoc />
        public override IEnumerable<NicoRepoFilter> FilterItems => FilterOnlyAll;

        public NicoRepoListSelfViewModel(INicoRepoService nicorepoService) : base(nicorepoService, "自分") {
        }
    }
}
