using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class CommunityFollowViewModel : PageSpinnerViewModel {


        #region CommunityNewsList変更通知プロパティ
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
