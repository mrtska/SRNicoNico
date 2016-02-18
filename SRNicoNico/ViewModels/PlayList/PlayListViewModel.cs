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

using SRNicoNico.Models.NicoNicoViewer;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class PlayListViewModel : TabItemViewModel {



        #region PlayList変更通知プロパティ
        private DispatcherCollection<PlayListEntryViewModel> _PlayList = new DispatcherCollection<PlayListEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<PlayListEntryViewModel> PlayList {
            get { return _PlayList; }
            set { 
                if(_PlayList == value)
                    return;
                _PlayList = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region SelectedPlayList変更通知プロパティ
        private PlayListEntryViewModel _SelectedPlayList;

        public PlayListEntryViewModel SelectedPlayList {
            get { return _SelectedPlayList; }
            set { 
                if(_SelectedPlayList == value)
                    return;
                _SelectedPlayList?.Video?.DisposeViewModel();
                _SelectedPlayList = value;
                RaisePropertyChanged();
                value.Video = new VideoViewModel(value);
                value.Video.Initialize();
            }
        }
        #endregion



        public PlayListViewModel(IList<MylistListEntryViewModel> list, string title) : base(title) {

            foreach(var entry in list) {

                //ブロマガとかプレイリストに突っ込まれても困るので弾く
                if(entry.Entry.Type == 0) {

                    PlayList.Add(new PlayListEntryViewModel(entry).RegisterOwner(this));
                }
            }
            SelectedPlayList = PlayList.First();
        }
        public PlayListViewModel(IList<PlayListEntryViewModel> list, string title) : base(title) {

            foreach(var entry in list) {

                PlayList.Add(entry.RegisterOwner(this));
            }
            SelectedPlayList = PlayList.First();
        }

        public override void KeyDown(KeyEventArgs e) {
            base.KeyDown(e);
            SelectedPlayList?.Video?.KeyDown(e);
        }




    }
}
