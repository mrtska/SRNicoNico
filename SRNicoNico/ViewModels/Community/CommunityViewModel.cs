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
    public class CommunityViewModel : TabItemViewModel {

        internal readonly NicoNicoCommunity CommunityInstance;


        #region CommunityInfo変更通知プロパティ
        private NicoNicoCommunityEntry _CommunityInfo;

        public NicoNicoCommunityEntry CommunityInfo {
            get { return _CommunityInfo; }
            set { 
                if(_CommunityInfo == value)
                    return;
                _CommunityInfo = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region UserContentList変更通知プロパティ
        private DispatcherCollection<TabItemViewModel> _CommunityContentList = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<TabItemViewModel> CommunityContentList {
            get { return _CommunityContentList; }
            set {
                if(_CommunityContentList == value)
                    return;
                _CommunityContentList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public CommunityViewModel(string communityUrl) : base("コミュニティ") {

            CommunityInstance = new NicoNicoCommunity(this, communityUrl);
        }


        public async void Initialize() {

            IsActive = true;

            var info = await CommunityInstance.GetCommunityAsync();

            if(info == null) {

                IsActive = false;
                Status = "コミュニティの読み込みに失敗しました。";
                return;
            }

            CommunityInfo = info;

            Name += "\n" + info.CommmunityName;

            CommunityContentList.Clear();

            //お知らせがあれば表示
            if(info.CommunityNews.Count > 0) {

                CommunityContentList.Add(new CommunityNoticeViewModel(this));
            }

            CommunityContentList.Add(new CommunityProfileViewModel(this));
            CommunityContentList.Add(new CommunityVideoViewModel(this));
            CommunityContentList.Add(new CommunityFollowViewModel(this));

            IsActive = false;

        }



        public void Refresh() {

            Initialize();
        }

        public void Close() {

            App.ViewModelRoot.MainContent.RemoveUserTab(this);
        }

        public override void KeyDown(KeyEventArgs e) {

            if(e.KeyboardDevice.Modifiers == ModifierKeys.Control) {

                if(e.Key == Key.W) {

                    Close();
                    return;
                }

                if(e.Key == Key.F5) {

                    Refresh();
                    return;
                }
            }

        }
        public override bool CanShowHelp() {
            return true;
        }

        public override void ShowHelpView(InteractionMessenger Messenger) {

            Messenger.Raise(new TransitionMessage(typeof(Views.StartHelpView), this, TransitionMode.NewOrActive));
        }

    }
}
