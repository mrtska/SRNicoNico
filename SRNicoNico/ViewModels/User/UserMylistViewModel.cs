using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class UserMylistViewModel : TabItemViewModel {

        private readonly UserViewModel Owner;

        public NicoNicoUserMylist Model { get; set; }

        public UserMylistViewModel(UserViewModel vm) : base("マイリスト") {

            Owner = vm;
            Model = new NicoNicoUserMylist(vm.UserPageUrl);
        }

        public async void Initialize() {

            IsActive = true;
            Owner.Status = "ユーザーマイリスト取得中";
            Owner.Status = await Model.GetUserMylistAsync();
            IsActive = false;
        }
    }
}
