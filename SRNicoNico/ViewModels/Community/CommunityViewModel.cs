using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;
using System.Threading.Tasks;

namespace SRNicoNico.ViewModels {
    public class CommunityViewModel : TabItemViewModel {

        private readonly NicoNicoCommunity Community;


        #region Content変更通知プロパティ
        private NicoNicoCommunityContent _Content;

        public NicoNicoCommunityContent Content {
            get { return _Content; }
            set { 
                if(_Content == value)
                    return;
                _Content = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public CommunityViewModel(string url) : base("読込中") {

            Community = new NicoNicoCommunity(url);


            App.ViewModelRoot.AddTabAndSetCurrent(this);

            IsActive = true;
            Content = Community.GetCommunity();
            IsActive = false;
            Name = Content.CommunityTitle;
        }


        public void OpenBrowser() {

            System.Diagnostics.Process.Start(Content.CommunityUrl);
        }

        public void Close() {

            App.ViewModelRoot.RemoveTabAndLastSet(this);
        }

        public void Reflesh() {

            Task.Run(() => {

                Close();
                new CommunityViewModel(Content.CommunityUrl);
            });

        }




    }
}
