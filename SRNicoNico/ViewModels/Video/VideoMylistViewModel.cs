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
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.ViewModels {
    public class VideoMylistViewModel : ViewModel {

        internal VideoViewModel Owner;

        private readonly NicoNicoMylist MylistInstance;


        #region MylistList変更通知プロパティ
        private DispatcherCollection<NicoNicoMylistGroupEntry> _MylistList = new DispatcherCollection<NicoNicoMylistGroupEntry>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<NicoNicoMylistGroupEntry> MylistList {
            get { return _MylistList; }
            set { 
                if (_MylistList == value)
                    return;
                _MylistList = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region SelectedMylist変更通知プロパティ
        private NicoNicoMylistGroupEntry _SelectedMylist;

        public NicoNicoMylistGroupEntry SelectedMylist {
            get { return _SelectedMylist; }
            set { 
                if (_SelectedMylist == value)
                    return;
                _SelectedMylist = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region MylistDescription変更通知プロパティ
        private string _MylistDescription;

        public string MylistDescription {
            get { return _MylistDescription; }
            set { 
                if (_MylistDescription == value)
                    return;
                _MylistDescription = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public VideoMylistViewModel(VideoViewModel vm) {

            Owner = vm;
            MylistInstance = new NicoNicoMylist(vm);
        }

        public async void AddDeflist() {

            await MylistInstance.Item.AddDefListAsync(Owner.ApiData.Video.Id, Owner.ApiData.Context.CsrfToken);
        }

        public async void OpenMylistView() {

            MylistList.Clear();
            //Viewを出す前に最新のマイリスト一覧を取得する
            var list = await MylistInstance.Group.GetMylistGroupAsync();

            if(list != null) {

                foreach(var group in list) {

                    MylistList.Add(group);
                }

                App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.AddMylistView), this, TransitionMode.Modal));
            }
        }

        public async void AddMylistCore() {

            await MylistInstance.Item.AddMylistAsync(SelectedMylist, Owner.ApiData.Video.Id, MylistDescription, Owner.ApiData.Context.CsrfToken);
            MylistDescription = "";
        }
    }
}
