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

using SRNicoNico.Models;

namespace SRNicoNico.ViewModels {
    public class TwitterSignInViewModel : ViewModel {

        //TwitterログインURL
        private const string TwitterSignInUrl = "https://account.nicovideo.jp/login/linkages/twitter/authorize?show_button_twitter=1&site=niconico&show_button_facebook=1&next_url=";


        #region TwitterSingInUrl変更通知プロパティ
        private Uri _TwitterSingInUrl = new Uri(TwitterSignInUrl);

        public Uri TwitterSingInUrl {
            get { return _TwitterSingInUrl; }
            set { 
                if(_TwitterSingInUrl == value)
                    return;
                _TwitterSingInUrl = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public TwitterSignInViewModel() {

        }



    }
}
