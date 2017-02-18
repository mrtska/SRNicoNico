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
    public class UserMylistViewModel : TabItemViewModel {

        #region UserMylistList変更通知プロパティ
        private DispatcherCollection<UserMylistEntryViewModel> _UserMylistList;

        public DispatcherCollection<UserMylistEntryViewModel> UserMylistList {
            get { return _UserMylistList; }
            set {
                if(_UserMylistList == value)
                    return;
                _UserMylistList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SelectedItem変更通知プロパティ
        private UserMylistEntryViewModel _SelectedItem;

        public UserMylistEntryViewModel SelectedItem {
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

        private UserViewModel User;

        public UserMylistViewModel(UserViewModel vm) : base("マイリスト") {

            User = vm;
            UserMylistList = new DispatcherCollection<UserMylistEntryViewModel>(DispatcherHelper.UIDispatcher);

        }

        public async void Initialize() {

            IsActive = true;

            var list = await User.UserInstance.GetUserMylistAsync();

            //非公開
            if(list == null) {

                Closed = true;
                IsActive = false;
                return;
            }

            foreach(var entry in list) {

                UserMylistList.Add(new UserMylistEntryViewModel(entry));
            }
            IsActive = false;
        }

    }
}
