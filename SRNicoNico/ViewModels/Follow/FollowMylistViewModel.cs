using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class FollowMylistViewModel : PageSpinnerViewModel {

        private readonly FollowViewModel Owner;

        public NicoNicoFollowMylist Model { get; set; }

        public FollowMylistViewModel(FollowViewModel owner) : base("マイリスト", 30) {

            Owner = owner;
            Model = new NicoNicoFollowMylist();
        }

        public async void Initialize() {

            IsActive = true;
            Owner.Status = "フォローマイリスト数を取得中";
            Owner.Status = await Model.FetchFollowedMylistCountAsync();

            if(Model.MylistCount == 0) {

                IsActive = false;
                Owner.Status = "フォローしているマイリストはありません。";
                return;
            }

            if(Model.MylistCount != -1) {

                MaxPages = (Model.MylistCount / 20) + 1;
            } else {

                return;
            }

            CurrentPage = 1;
            GetPage();
        }

        public async void GetPage() {

            IsActive = true;
            Owner.Status = "フォローマイリストを取得中";
            Owner.Status = await Model.GetFollowedMylistAsync(CurrentPage);
            IsActive = false;
        }

        public void Refresh() {

            Initialize();
        }
        public override void KeyDown(KeyEventArgs e) {
            base.KeyDown(e);

            switch(e.Key) {
                case Key.F5:
                    Refresh();
                    break;
            }
        }

        public override void SpinPage() {

            GetPage();
        }
    }
}
