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
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class FollowCommunityViewModel : TabItemViewModel {



        #region CommunityList変更通知プロパティ
        private DispatcherCollection<FollowCommunityEntryViewModel> _CommunityList = new DispatcherCollection<FollowCommunityEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<FollowCommunityEntryViewModel> CommunityList {
            get { return _CommunityList; }
            set {
                if(_CommunityList == value)
                    return;
                _CommunityList = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region FollowedCommunityCount変更通知プロパティ
        private int _FollowedCommunityCount = -2;

        public int FollowedCommunityCount {
            get { return _FollowedCommunityCount; }
            set { 
                if(_FollowedCommunityCount == value)
                    return;
                _FollowedCommunityCount = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region MaxPages変更通知プロパティ
        private int _MaxPages = 0;

        public int MaxPages {
            get { return _MaxPages; }
            set {
                if(_MaxPages == value)
                    return;
                if(value > 30) {

                    value = 30;
                }
                _MaxPages = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region CurrentPage変更通知プロパティ
        private int _CurrentPage = 1;

        public int CurrentPage {
            get { return _CurrentPage; }
            set {
                if(_CurrentPage == value)
                    return;
                if(value > MaxPages) {

                    value = MaxPages;
                }
                if(value <= 1) {

                    LeftButtonEnabled = false;
                } else {

                    LeftButtonEnabled = true;
                }
                if(value >= MaxPages) {

                    RightButtonEnabled = false;
                } else {

                    RightButtonEnabled = true;
                }

                _CurrentPage = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region LeftButtonEnabled変更通知プロパティ
        private bool _LeftButtonEnabled = false;

        public bool LeftButtonEnabled {
            get { return _LeftButtonEnabled; }
            set {
                if(_LeftButtonEnabled == value)
                    return;
                _LeftButtonEnabled = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region RightButtonEnabled変更通知プロパティ
        private bool _RightButtonEnabled = true;

        public bool RightButtonEnabled {
            get { return _RightButtonEnabled; }
            set {
                if(_RightButtonEnabled == value)
                    return;
                _RightButtonEnabled = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        private FollowViewModel Owner;

        private NicoNicoFollow FollowInstance;

        public FollowCommunityViewModel(FollowViewModel owner, NicoNicoFollow instance) : base("コミュニティ") {

            Owner = owner;
            FollowInstance = instance;
        }

        public async void Initialize() {

            IsActive = true;
            Owner.Status = "フォローコミュニティ数を取得中";
            FollowedCommunityCount = await FollowInstance.GetFollowedCommunityCountAsync();

            if(FollowedCommunityCount == 0) {

                Owner.Status = "フォローしているコミュニティはありません。";
                IsActive = false;
                return;
            }


            if(FollowedCommunityCount != -1) {

                MaxPages = (FollowedCommunityCount / 10) + 1;
            } else {

                return;
            }

            CurrentPage = 1;
            GetPage();

        }

        public async void GetPage() {

            Owner.Status = "フォローコミュニティを取得中";
            var list = await FollowInstance.GetFollowedCommunityAsync(CurrentPage);

            if(list == null) {

                return;
            }

            CommunityList.Clear();

            foreach(var entry in list) {

                CommunityList.Add(new FollowCommunityEntryViewModel(entry));
            }
            IsActive = false;
            Owner.Status = "";
        }

        public void Refresh() {

            Initialize();
        }

        public void LeftButtonClick() {

            if(LeftButtonEnabled) {

                CurrentPage--;
                GetPage();
            }
        }
        public void RightButtonClick() {

            if(RightButtonEnabled) {

                CurrentPage++;
                GetPage();
            }
        }

        public override void KeyDown(KeyEventArgs e) {

            switch(e.Key) {
                case Key.Left:
                    LeftButtonClick();
                    break;
                case Key.Right:
                    RightButtonClick();
                    break;
                case Key.F5:
                    Refresh();
                    break;
            }
        }
    }
}
