using System.Collections.Generic;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ニコレポの「ユーザー」タブ
    /// </summary>
    public class NicoRepoListUserViewModel : NicoRepoListViewModel {

        /// <inheritdoc />     
        public override NicoRepoType NicoRepoType => NicoRepoType.User;

        /// <inheritdoc />
        public override IEnumerable<NicoRepoFilter> FilterItems => FilterAll;

        public NicoRepoListUserViewModel(INicoRepoService nicorepoService) : base(nicorepoService, "ユーザー") {
        }
    }
}
