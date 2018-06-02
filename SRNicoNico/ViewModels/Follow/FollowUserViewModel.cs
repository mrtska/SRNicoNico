using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class FollowUserViewModel : PageSpinnerViewModel {

        private readonly FollowViewModel Owner;

        public NicoNicoFollowUser Model { get; set; }

        public FollowUserViewModel(FollowViewModel owner) : base("ユーザー", 30) {

            Owner = owner;
            Model = new NicoNicoFollowUser();
        }

        public async void Initialize() {

            IsActive = true;
            Owner.Status = "フォローユーザー数を取得中";
            Owner.Status = await Model.FetchFollowedUserCountAsync();

            if(Model.UserCount == 0) {

                IsActive = false;
                Owner.Status = "フォローしているユーザーはいません。";
                return;
            }

            if(Model.UserCount != -1) {

                MaxPages = (Model.UserCount / 20) + 1;
            } else {

                return;
            }

            CurrentPage = 1;
            GetPage();
        }

        public async void GetPage() {

            IsActive = true;
            Owner.Status = "フォローユーザーを取得中";
            Owner.Status = await Model.GetFollowedUserAsync(CurrentPage);
            IsActive = false;
        }

        public void Refresh() {

            Initialize();
        }

        public override void SpinPage() {

            GetPage();
        }

        public override void KeyDown(KeyEventArgs e) {
            base.KeyDown(e);

            switch(e.Key) {
                case Key.F5:
                    Refresh();
                    break;
            }
        }
    }
}
