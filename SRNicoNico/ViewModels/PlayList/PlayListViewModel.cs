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
using SRNicoNico.Views.Contents.Video;

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
                Jump(value);
                _SelectedPlayList = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region IsRepeat変更通知プロパティ
        private bool _IsRepeat;

        public bool IsRepeat {
            get { return _IsRepeat; }
            set { 
                if(_IsRepeat == value)
                    return;
                _IsRepeat = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Video変更通知プロパティ
        private VideoViewModel _Video;

        public VideoViewModel Video {
            get { return _Video; }
            set {
                if(_Video == value)
                    return;
                _Video = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        

        public bool IsFullScreen;

        public PlayListViewModel(IList<MylistListEntryViewModel> list, string title) : base(title) {

            foreach(var entry in list) {

                //ブロマガとかプレイリストに突っ込まれても困るので弾く
                if(entry.Entry.Type == 0) {

                    PlayList.Add(new PlayListEntryViewModel(entry).RegisterOwner(this));
                }
            }
            Initialize();
        }
        public PlayListViewModel(IList<PlayListEntryViewModel> list, string title) : base(title) {
            
            foreach(var entry in list) {

                PlayList.Add(entry.RegisterOwner(this));
            }
            Initialize();
        }

        private void Initialize() {

            SelectedPlayList = PlayList.First();
            Video = new VideoViewModel(SelectedPlayList.VideoUrl, this);
        }


        public override void KeyDown(KeyEventArgs e) {
            base.KeyDown(e);
            Video?.KeyDown(e);

            switch(e.Key) {
                case Key.N:
                    Next();
                    break;
                case Key.P:
                    Prev();
                    break;
            }
        }

        public void ToggleRepeat() {

            IsRepeat ^= true;
        }

        //次の動画へ
        public void Next() {

            if(PlayList.Count == 1) {

                return;
            }

            var index = PlayList.IndexOf(SelectedPlayList);

            if(index + 1 >= PlayList.Count) {

                if(IsRepeat) {

                    SelectedPlayList = PlayList.First();
                }
            } else {

                SelectedPlayList = PlayList[index + 1];
            }

        }
        //前の動画へ
        public void Prev() {

            if(PlayList.Count == 1) {

                return;
            }

            var index = PlayList.IndexOf(SelectedPlayList);
            if(index <= 0) {

                SelectedPlayList = PlayList.Last();
            } else {

                SelectedPlayList = PlayList[index - 1];
            }
        }

        //指定したプレイリストエントリに飛ぶ
        public void Jump(PlayListEntryViewModel entry) {

            Video?.JumpTo(entry.VideoUrl);
        }
        
        
    }
}
