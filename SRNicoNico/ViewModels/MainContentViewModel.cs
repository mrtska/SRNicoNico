using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRNicoNico.ViewModels {
    public class MainContentViewModel : ViewModel {


        //左側のシステムタブのリスト
        public DispatcherCollection<TabItemViewModel> SystemItems { get; set; }


        public DispatcherCollection<TabItemViewModel> UserItems { get; set; }


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

        //VideoView
        public VideoViewViewModel VideoView { get; private set; }

        private MainWindowViewModel Owner;

        public MainContentViewModel(MainWindowViewModel owner) {

            Owner = owner;
            SystemItems = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);
            UserItems = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);



            VideoView = new VideoViewViewModel();
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
            if(!UserItems.Contains(VideoView)) {

                AddUserTab(VideoView);
            }
            VideoView.Add(vm);
            SelectedTab = VideoView;
        }

        public void RemoveVideoView(VideoViewModel vm) {

            var index = VideoView.VideoList.IndexOf(vm);

            if(index < 0) {

                return;
            }
            if(index == 0) {

                VideoView.Remove(vm);
                VideoView.SelectedList = VideoView.VideoList.FirstOrDefault();
            } else {

                VideoView.Remove(vm);
                VideoView.SelectedList = VideoView.VideoList[index - 1];
            }

            if(VideoView.VideoList.Count == 0) {

                UserItems.Remove(VideoView);
            }
        }

    }
}
