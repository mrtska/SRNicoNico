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

        private List<ViewModel> UnFilteredNicoRepoList = new List<ViewModel>();

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

        #region Filter変更通知プロパティ
        private string _Filter;

        public string Filter {
            get { return _Filter; }
            set {
                if (_Filter == value)
                    return;
                _Filter = value;

                FilterNicoRepo();
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
            UnFilteredNicoRepoList.Clear();
            UserNicoRepoList.Clear();
            GetMore();
        }

        public async void GetMore() {


            IsActive = true;

            //一番最後にあるボタンを消す
            if(UserNicoRepoList.Count > 0) {

                UserNicoRepoList.RemoveAt(UserNicoRepoList.Count - 1);
            }
            /*
            var ret = await User.UserInstance.GetUserNicoRepoAsync(User.UserInfo.UserId, UnFilteredNicoRepoList.Count != 0 ? ((NicoNicoNicoRepoResultEntry) UnFilteredNicoRepoList.Last()).Item.Id : null);

            if (ret != null) {

                foreach (var entry in ret.Item1) {

                    var vm = new NicoNicoNicoRepoResultEntry(entry);
                    UnFilteredNicoRepoList.Add(vm);

                    if (FilterEntry(vm)) {

                        UserNicoRepoList.Add(vm);
                    }
                }

                if (ret.Item2) {

                    UserNicoRepoList.Add(new NicoRepoNextButtonEntryViewModel(this));
                }

                if (UnFilteredNicoRepoList.Count == 0) {

                    Closed = true;
                }

                IsActive = false;
            }*/
        }

        public void FilterNicoRepo() {

            if (UnFilteredNicoRepoList.Count == 0) {

                return;
            }

            /*bool isnotEnd = UserNicoRepoList.Count != 0 && UserNicoRepoList.Last() is NicoRepoNextButtonEntryViewModel;
            UserNicoRepoList?.Clear();

            foreach (var raw in UnFilteredNicoRepoList) {
                /*
                if (raw is NicoNicoNicoRepoResultEntry item) {

                    if (FilterEntry(item)) {

                        UserNicoRepoList.Add(raw);
                    }
                }
            }
            if (isnotEnd) {

                //UserNicoRepoList.Add(new NicoRepoNextButtonEntryViewModel(this));*/
        }

        private bool FilterEntry(NicoNicoNicoRepoResultEntry item) {

            if(item.Muted) {

                return false;
            }

            switch (Filter) {
                case "すべて":
                    return true;
                case "動画投稿のみ":
                    if (item.Topic.EndsWith("video.upload")) {
                        return true;
                    }
                    return false;
                case "生放送開始のみ":
                    if (item.Topic.EndsWith("program.onairs")) {
                        return true;
                    }
                    return false;
                case "マイリスト登録のみ":
                    if (item.Topic.Contains("mylist.add")) {
                        return true;
                    }
                    return false;
                default:
                    return true;
            }
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
