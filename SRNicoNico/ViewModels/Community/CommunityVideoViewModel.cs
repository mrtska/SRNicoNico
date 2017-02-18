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
    public class CommunityVideoViewModel : PageSpinnerViewModel {

        #region IsEmpty変更通知プロパティ
        private bool _IsEmpty;

        public bool IsEmpty {
            get { return _IsEmpty; }
            set { 
                if(_IsEmpty == value)
                    return;
                _IsEmpty = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CommunityVideoList変更通知プロパティ
        private DispatcherCollection<SearchResultEntryViewModel> _CommunityVideoList = new DispatcherCollection<SearchResultEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<SearchResultEntryViewModel> CommunityVideoList {
            get { return _CommunityVideoList; }
            set { 
                if(_CommunityVideoList == value)
                    return;
                _CommunityVideoList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        //OwnerViewModel
        private CommunityViewModel Community;

        public CommunityVideoViewModel(CommunityViewModel vm) : base("動画", int.MaxValue) {

            Community = vm;
        }

        public async void GetPage(int page) {

            IsActive = true;

            CommunityVideoList.Clear();

            var videos = await Community.CommunityInstance.GetCommunityVideoAsync(page);

            if(videos != null) {

                foreach(var video in videos) {

                    CommunityVideoList.Add(new SearchResultEntryViewModel(video));
                }
            }

            IsActive = false;
        }

        public async void Initialize() {

            IsActive = true;
            var count = await Community.CommunityInstance.GetCommunityVideoCountAsync();

            if(count <= 0) {

                IsEmpty = true;
                IsActive = false;
                return;
            }

            MaxPages = count / 20 + 1;
            GetPage(1);
        }

        public override void SpinPage() {

            GetPage(CurrentPage);
        }
    }
}
