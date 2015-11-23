using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRNicoNico.ViewModels {
    public class UserNicoRepoViewModel : TabItemViewModel {

        #region UserNicoRepoList変更通知プロパティ
        private DispatcherCollection<NicoRepoResultEntryViewModel> _UserNicoRepoList;

        public DispatcherCollection<NicoRepoResultEntryViewModel> UserNicoRepoList {
            get { return _UserNicoRepoList; }
            set {
                if(_UserNicoRepoList == value)
                    return;
                _UserNicoRepoList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SelectedItem変更通知プロパティ
        private NicoRepoResultEntryViewModel _SelectedItem;

        public NicoRepoResultEntryViewModel SelectedItem {
            get { return _SelectedItem; }
            set {
                if(_SelectedItem == value)
                    return;
                _SelectedItem = value;
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


        //OwnerViewModel
        private UserViewModel User;

        //ニコレポを全て取得し終わったら
        private bool IsEnd = false;

        public UserNicoRepoViewModel(UserViewModel user) : base("ニコレポ") {

            User = user;
            UserNicoRepoList = new DispatcherCollection<NicoRepoResultEntryViewModel>(DispatcherHelper.UIDispatcher);

        }

        public void Initialize() {

            IsActive = true;
            Task.Run(() => {

                var timeline = User.UserInstance.GetUserNicoRepo();

                if(timeline == null) {

                    if(UserNicoRepoList.Count == 0) {

                        //非公開、又は表示期限切れ
                        Closed = true;
                    }
                    IsActive = false;
                    IsEnd = true;
                    return;
                }
                foreach(var entry in timeline) {

                    UserNicoRepoList.Add(new NicoRepoResultEntryViewModel(entry));
                }

                IsActive = false;
            });
        }

        //インフィニットスクロール発動で呼ばれる
        public void Next() {

            if(IsEnd) {

                return;
            }
            IsActive = true;

            Task.Run(() => {

                var timeline = User.UserInstance.GetUserNicoRepo();

                if(timeline == null) {

                    IsEnd = true;
                    IsActive = false;
                    return;
                }
                foreach(var entry in timeline) {

                    UserNicoRepoList.Add(new NicoRepoResultEntryViewModel(entry));
                }

                IsActive = false;
            });
        }

        public void Open() {
            //not existsの時など
            if(SelectedItem == null || SelectedItem.Entry.VideoUrl == null) {

                SelectedItem = null;
                return;
            }
            NicoNicoOpener.Open(SelectedItem.Entry.VideoUrl);
            SelectedItem = null;
        }
    }

}
