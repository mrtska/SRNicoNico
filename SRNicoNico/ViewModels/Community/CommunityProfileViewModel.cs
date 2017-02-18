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
    public class CommunityProfileViewModel : TabItemViewModel {


        #region ProfileHtml変更通知プロパティ
        private string _ProfileHtml;

        public string ProfileHtml {
            get { return _ProfileHtml; }
            set { 
                if(_ProfileHtml == value)
                    return;
                _ProfileHtml = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        //OwnerViewModel
        private CommunityViewModel Community;

        public CommunityProfileViewModel(CommunityViewModel vm) : base("プロフィール") {

            Community = vm;

            ProfileHtml = Community.CommunityInfo.CommunityProfile;
        }
    }
}
