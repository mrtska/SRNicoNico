using Livet;
using System.Linq;

namespace SRNicoNico.ViewModels {
    public class MainContentViewModel : ViewModel {

        //左側のシステムタブのリスト
        public ObservableSynchronizedCollection<TabItemViewModel> SystemItems { get; set; }

        public ObservableSynchronizedCollection<TabItemViewModel> UserItems { get; set; }

        #region SelectedTab変更通知プロパティ
        private TabItemViewModel _SelectedTab;

        public TabItemViewModel SelectedTab {
            get { return _SelectedTab; }
            set {
                if(_SelectedTab == value)
                    return;
                _SelectedTab = value;
                RaisePropertyChanged();
                if(value != null) {

                    Owner.Status = value.Status;
                    Owner.CanShowHelpView = value.CanShowHelp();
                }
            }
        }
        #endregion

        // VideoTab
        public VideoTabViewModel VideoTab { get; private set; }

        private MainWindowViewModel Owner;

        public MainContentViewModel(MainWindowViewModel owner) {

            Owner = owner;
            SystemItems = new ObservableSynchronizedCollection<TabItemViewModel>();
            UserItems = new ObservableSynchronizedCollection<TabItemViewModel>();

            VideoTab = new VideoTabViewModel();
        }

        public void AddSystemTab(TabItemViewModel vm) {

            SystemItems.Add(vm);
        }

        public void AddUserTab(TabItemViewModel vm) {

            UserItems.Add(vm);
            SelectedTab = vm;
        }
        public void RemoveUserTab(TabItemViewModel vm) {

            if(UserItems.Contains(vm)) {

                UserItems.Remove(vm);

                if(UserItems.Count == 0) {

                    SelectedTab = Owner.Start;
                } else {

                    SelectedTab = UserItems.Last();
                }
            }
        }

        public void AddVideoView(VideoViewModel vm) {
            
            //UserItemsにVideoViewが無かったら追加する
            if(!UserItems.Contains(VideoTab)) {

                AddUserTab(VideoTab);
            }
            VideoTab.Add(vm);
            SelectedTab = VideoTab;
        }

        public void RemoveVideoView(VideoViewModel vm) {

            var index = VideoTab.VideoList.IndexOf(vm);

            if(index < 0) {

                return;
            }
            if(index == 0) {

                VideoTab.Remove(vm);
                VideoTab.SelectedList = VideoTab.VideoList.FirstOrDefault();
            } else {

                VideoTab.Remove(vm);
                VideoTab.SelectedList = VideoTab.VideoList[index - 1];
            }

            if(VideoTab.VideoList.Count == 0) {

                UserItems.Remove(VideoTab);
            }
        }
    }
}
