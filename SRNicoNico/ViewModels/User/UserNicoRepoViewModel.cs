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
    public class UserNicoRepoViewModel : TabItemViewModel {

        #region UserNicoRepoList変更通知プロパティ
        private DispatcherCollection<ViewModel> _UserNicoRepoList = new DispatcherCollection<ViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<ViewModel> UserNicoRepoList {
            get { return _UserNicoRepoList; }
            set {
                if(_UserNicoRepoList == value)
                    return;
                _UserNicoRepoList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Closed変更通知プロパティ
        private bool _Closed;

        public bool Closed {
            get { return _Closed; }
            set {
                if(_Closed == value)
                    return;
                _Closed = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        private UserViewModel User;
        public UserNicoRepoViewModel(UserViewModel user) : base("ニコレポ") {

            User = user;
        }

        public void Initialize() {

            Closed = false;
            UserNicoRepoList.Clear();
            GetMore();
        }

        public async void GetMore() {

            if(IsActive) {

                return;
            }

            IsActive = true;

            //一番最後にあるボタンを消す
            if(UserNicoRepoList.Count > 0) {

                UserNicoRepoList.RemoveAt(UserNicoRepoList.Count - 1);
            }

            var timeline = await User.UserInstance.GetUserNicoRepoAsync("");
            /*
            if(timeline == null || timeline.Items.Count == 0) {

                //非公開、又は表示期限切れ
                Closed = true;
                IsActive = false;
                return;
            }
            foreach(var entry in timeline.Items) {

                UserNicoRepoList.Add(new NicoRepoResultEntryViewModel(entry));
            }

            IsActive = false;
            if(!timeline.IsEnd) {

                UserNicoRepoList.Add(new NicoRepoNextButtonEntryViewModel(this));
            }*/
        }

        public void Refresh() {

            Initialize();
        }

        public override void KeyDown(KeyEventArgs e) {

            if(e.Key == Key.F5) {

                Refresh();
            }
            if(e.Key == Key.Space) {

                GetMore();
            }
        }
    }
}
