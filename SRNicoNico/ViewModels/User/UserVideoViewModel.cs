using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class UserVideoViewModel : PageSpinnerViewModel {

        private UserViewModel Owner;

        public NicoNicoUserVideo Model { get; set; }

        public UserVideoViewModel(UserViewModel vm) : base("投稿動画") {

            Owner = vm;
            Model = new NicoNicoUserVideo(vm.UserPageUrl);
        }

        public async void Initialize() {

            MaxPages = await Model.GetUserVideoCountAsync() / 30 + 1;
            CurrentPage = 1;
            GetPage();
        }

        public override void SpinPage() {

            GetPage();
        }

        public async void GetPage() {

            IsActive = true;
            Owner.Status = "投稿動画を取得中";
            Owner.Status = await Model.GetUserVideoAsync(CurrentPage);
            IsActive = false;
        }
    }
}
