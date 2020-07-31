using Livet;
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
        private DispatcherCollection<NicoNicoSearchResultEntry> _CommunityVideoList = new DispatcherCollection<NicoNicoSearchResultEntry>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<NicoNicoSearchResultEntry> CommunityVideoList {
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
        private readonly CommunityViewModel Community;

        public CommunityVideoViewModel(CommunityViewModel vm) : base("動画", int.MaxValue) {

            Community = vm;
        }

        public async void GetPage(int page) {

            IsActive = true;

            CommunityVideoList.Clear();

            var videos = await Community.CommunityInstance.GetCommunityVideoAsync(Community.CommunityInfo.Id, page);

            if(videos != null) {

                foreach(var video in videos) {

                    CommunityVideoList.Add(video);
                }
            }
            IsActive = false;
            MaxPages = CommunityVideoList.Count / 20 + 1;
        }

        public void Initialize() {

            IsActive = true;
            //GetPage(1);

        }

        public override void SpinPage() {

            //GetPage(CurrentPage);
        }
    }
}
