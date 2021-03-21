using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ニコレポの「ユーザー」タブ
    /// </summary>
    public class NicoRepoListUserViewModel : NicoRepoListViewModel {

        /// <inheritdoc />     
        public override NicoRepoType NicoRepoType => NicoRepoType.User;

        public NicoRepoListUserViewModel(INicoRepoService nicorepoService) : base(nicorepoService, "ユーザー") {
        }
    }
}
