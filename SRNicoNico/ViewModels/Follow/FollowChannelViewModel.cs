using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class FollowChannelViewModel : PageSpinnerViewModel {

        private readonly FollowViewModel Owner;

        public NicoNicoFollowChannel Model { get; set; }

        public FollowChannelViewModel(FollowViewModel owner) : base("チャンネル", 30) {

            Owner = owner;
            Model = new NicoNicoFollowChannel();
        }

        public void Initialize() {

            IsActive = true;
            Owner.Status = "フォローチャンネル数を取得中";
            CurrentPage = 1;
            GetPage();
        }

        public async void GetPage() {

            IsActive = true;
            Owner.Status = "フォローチャンネルを取得中";
            Owner.Status = await Model.GetFollowedChannelAsync(CurrentPage);
            IsActive = false;
            if (Model.ChannelCount != -1) {

                MaxPages = (Model.ChannelCount / 25) + 1;
            }
            if (Model.ChannelCount == 0) {

                Owner.Status = "フォローしているチャンネルはありません。";
            }
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
