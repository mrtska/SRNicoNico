using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ニコレポの「マイリスト」タブ
    /// </summary>
    public class NicoRepoListMylistViewModel : NicoRepoListViewModel {

        /// <inheritdoc />     
        public override NicoRepoType NicoRepoType => NicoRepoType.Mylist;

        public NicoRepoListMylistViewModel(INicoRepoService nicorepoService) : base(nicorepoService, "マイリスト") {
        }
    }
}
