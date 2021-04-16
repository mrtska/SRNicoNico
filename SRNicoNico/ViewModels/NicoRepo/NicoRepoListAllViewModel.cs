using System.Collections.Generic;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ニコレポの「すべて」タブ
    /// </summary>
    public class NicoRepoListAllViewModel : NicoRepoListViewModel {

        /// <inheritdoc />
        public override NicoRepoType NicoRepoType => NicoRepoType.All;

        /// <inheritdoc />
        public override IEnumerable<NicoRepoFilter> FilterItems => FilterAll;

        public NicoRepoListAllViewModel(INicoRepoService nicorepoService) : base(nicorepoService, "すべて") {
        }
    }
}
