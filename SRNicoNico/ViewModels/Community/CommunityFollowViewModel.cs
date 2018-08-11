﻿using Livet;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class CommunityFollowViewModel : PageSpinnerViewModel {


        #region CommunityMemberList変更通知プロパティ
        private DispatcherCollection<NicoNicoCommunityMember> _CommunityMemberList = new DispatcherCollection<NicoNicoCommunityMember>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<NicoNicoCommunityMember> CommunityMemberList {
            get { return _CommunityMemberList; }
            set { 
                if(_CommunityMemberList == value)
                    return;
                _CommunityMemberList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        //OwnerViewModel
        private CommunityViewModel Community;

        public CommunityFollowViewModel(CommunityViewModel vm) : base("フォロワー", int.MaxValue) {

            Community = vm;
        }

        public async void GetPage(int page) {

            IsActive = true;

            CommunityMemberList.Clear();

            var follower = await Community.CommunityInstance.GetCommunityFolowerAsync(page);

            if(follower != null) {

                foreach(var video in follower) {

                    CommunityMemberList.Add(video);
                }
            }
            IsActive = false;
        }

        public override void SpinPage() {

            GetPage(CurrentPage);
        }

        public async void Initialize() {

            IsActive = true;
            var count = await Community.CommunityInstance.GetCommunityFollowerCountAsync();

            if(count < 0) {

                IsActive = false;
                return;
            }

            MaxPages = count / 35 + 1;
            GetPage(1);
        }

    }
}
