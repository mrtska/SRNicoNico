using System.Collections.Generic;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ニコレポの「チャンネル」タブ
    /// </summary>
    public class NicoRepoListChannelViewModel : NicoRepoListViewModel {

        /// <inheritdoc />     
        public override NicoRepoType NicoRepoType => NicoRepoType.Channel;

        /// <inheritdoc />
        public override IEnumerable<NicoRepoFilter> FilterItems => FilterAll;

        public NicoRepoListChannelViewModel(INicoRepoService nicorepoService) : base(nicorepoService, "チャンネル") {
        }
    }
}
