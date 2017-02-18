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
    public class FollowChannelViewModel : TabItemViewModel {



        #region ChannelList変更通知プロパティ
        private DispatcherCollection<FollowChannelEntryViewModel> _ChannelList = new DispatcherCollection<FollowChannelEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<FollowChannelEntryViewModel> ChannelList {
            get { return _ChannelList; }
            set {
                if(_ChannelList == value)
                    return;
                _ChannelList = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region FollowedChannelCount変更通知プロパティ
        private int _FollowedChannelCount = -2;

        public int FollowedChannelCount {
            get { return _FollowedChannelCount; }
            set { 
                if(_FollowedChannelCount == value)
                    return;
                _FollowedChannelCount = value;
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

        public FollowChannelViewModel(FollowViewModel owner, NicoNicoFollow instance) : base("チャンネル") {

            Owner = owner;
            FollowInstance = instance;
        }

        public async void Initialize() {

            IsActive = true;
            Owner.Status = "フォローチャンネル数を取得中";
            FollowedChannelCount = await FollowInstance.GetFollowedChannelCountAsync();

            if(FollowedChannelCount != -1) {

                MaxPages = (FollowedChannelCount / 10) + 1;
            } else {

                return;
            }

            CurrentPage = 1;
            GetPage();

        }

        public async void GetPage() {

            Owner.Status = "フォローチャンネルを取得中";
            var list = await FollowInstance.GetFollowedChannelAsync(CurrentPage);

            if(list == null) {

                return;
            }

            ChannelList.Clear();

            foreach(var entry in list) {

                ChannelList.Add(new FollowChannelEntryViewModel(entry));
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
