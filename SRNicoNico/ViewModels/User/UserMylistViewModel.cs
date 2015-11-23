using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRNicoNico.ViewModels {
    public class UserMylistViewModel : TabItemViewModel {




        #region UserMylistList変更通知プロパティ
        private DispatcherCollection<NicoNicoUserMylistEntry> _UserMylistList;

        public DispatcherCollection<NicoNicoUserMylistEntry> UserMylistList {
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
        private NicoNicoUserMylistEntry _SelectedItem;

        public NicoNicoUserMylistEntry SelectedItem {
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

        

        public UserMylistViewModel(UserViewModel vm) : base("マイリスト") {

            User = vm;
            UserMylistList = new DispatcherCollection<NicoNicoUserMylistEntry>(DispatcherHelper.UIDispatcher);

        }

        public void Initialize() {

            IsActive = true;
            Task.Run(() => {

                var list = User.UserInstance.GetUserMylist();

                //非公開
                if(list == null) {

                    Closed = true;
                    IsActive = false;
                    return;
                }

                foreach(var entry in list) {

                    UserMylistList.Add(entry);
                }
                IsActive = false;
            });
        }
        public void Open() {

            //not existsの時など
            if(SelectedItem == null || SelectedItem.Url == null) {

                SelectedItem = null;
                return;
            }
            NicoNicoOpener.Open(SelectedItem.Url);
            SelectedItem = null;
        }


    }

}
