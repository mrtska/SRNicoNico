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
    public class CommunityNoticeViewModel : TabItemViewModel {

        #region CommunityNewsList変更通知プロパティ
        private DispatcherCollection<NicoNicoCommunityNews> _CommunityNewsList = new DispatcherCollection<NicoNicoCommunityNews>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<NicoNicoCommunityNews> CommunityNewsList {
            get { return _CommunityNewsList; }
            set { 
                if(_CommunityNewsList == value)
                    return;
                _CommunityNewsList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

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

        //OwnerViewModel
        private CommunityViewModel Community;

        public CommunityNoticeViewModel(CommunityViewModel vm) : base("お知らせ") {

            Community = vm;
        }

        public void Initialize() {

            foreach(var news in Community.CommunityInfo.CommunityNews) {

                CommunityNewsList.Add(news);
            }

        }
    }
}
