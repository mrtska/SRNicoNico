using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class UserVideoViewModel : PageSpinnerViewModel {

        private UserViewModel Owner;

        public NicoNicoUserVideo Model { get; set; }

        public UserVideoViewModel(UserViewModel vm) : base("投稿動画") {

            Owner = vm;
            Model = new NicoNicoUserVideo(vm.Model.UserInfo.UserId);
        }

        public async void Initialize() {

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
            MaxPages = Model.VideoCount / 25 + 1;
            IsActive = false;
        }
    }
}
