using Livet;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class FollowViewModel : TabItemViewModel {

        #region FavoriteList変更通知プロパティ
        private DispatcherCollection<TabItemViewModel> _FavoriteList = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<TabItemViewModel> FavoriteList {
            get { return _FavoriteList; }
            set {
                if(_FavoriteList == value)
                    return;
                _FavoriteList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SelectedList変更通知プロパティ
        private TabItemViewModel _SelectedList;

        public TabItemViewModel SelectedList {
            get { return _SelectedList; }
            set {
                if(_SelectedList == value)
                    return;
                _SelectedList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public FollowViewModel() : base("フォロー") {

            FavoriteList.Add(new FollowUserViewModel(this));
            FavoriteList.Add(new FollowMylistViewModel(this));
            FavoriteList.Add(new FollowChannelViewModel(this));
            FavoriteList.Add(new FollowCommunityViewModel(this));
        }

        public override void KeyDown(KeyEventArgs e) {

            SelectedList?.KeyDown(e);
        }
    }
}
