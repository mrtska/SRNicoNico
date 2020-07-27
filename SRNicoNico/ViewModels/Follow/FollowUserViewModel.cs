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

        public void Initialize() {

            IsActive = true;
            Owner.Status = "フォローユーザー数を取得中";
            MaxPages = 0;
            CurrentPage = 1;
            GetPage();

            if (Model.UserCount == 0) {

                IsActive = false;
                Owner.Status = "フォローしているユーザーはいません。";
                return;
            }

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
