﻿using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class FollowCommunityViewModel : PageSpinnerViewModel {

        private readonly FollowViewModel Owner;

        public NicoNicoFollowCommunity Model { get; set; }

        public FollowCommunityViewModel(FollowViewModel owner) : base("コミュニティ", 30) {

            Owner = owner;
            Model = new NicoNicoFollowCommunity();
        }

        public void Initialize() {

            IsActive = true;
            Owner.Status = "フォローコミュニティ数を取得中";
            CurrentPage = 1;
            GetPage();
        }

        public async void GetPage() {

            IsActive = true;
            Owner.Status = "フォローコミュニティを取得中";
            Owner.Status = await Model.GetFollowedCommunityAsync(CurrentPage);
            IsActive = false;
            if (Model.CommunityCount != -1) {

                MaxPages = (Model.CommunityCount / 25) + 1;
            }
            if (Model.CommunityCount == 0) {

                Owner.Status = "フォローしているコミュニティはありません。";
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
