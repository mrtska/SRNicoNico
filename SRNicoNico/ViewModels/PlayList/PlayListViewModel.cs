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
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using GongSolutions.Wpf.DragDrop;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop.Utilities;
using System.Collections;

namespace SRNicoNico.ViewModels {
    public class PlayListViewModel : TabItemViewModel, IDropTarget {

#if false
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

#endif

        #region PlayList変更通知プロパティ
        private ObservableCollection<PlayListEntryViewModel> _PlayList = new ObservableCollection<PlayListEntryViewModel>();

        public ObservableCollection<PlayListEntryViewModel> PlayList {
            get { return _PlayList; }
            set {
                if(_PlayList == value)
                    return;
                _PlayList = value;
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

    #region SelectedPlayList変更通知プロパティ
        private PlayListEntryViewModel _SelectedPlayList;

        public PlayListEntryViewModel SelectedPlayList {
            get { return _SelectedPlayList; }
            set {
                if(_SelectedPlayList == value ||  value == null)
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

        public PlayListViewModel(string title, IEnumerable<NicoNicoMylistEntry> entries) : base("プレイリスト\n" + title) {

            foreach(var entry in entries) {

                PlayList.Add(new PlayListEntryViewModel(entry));
            }

            SelectedPlayList = PlayList.First();
        }

        public PlayListViewModel(string title, IEnumerable<NicoNicoSearchResultEntry> entries) : base("プレイリスト\n" + title) {

            foreach(var entry in entries) {

                PlayList.Add(new PlayListEntryViewModel(entry));
            }

            SelectedPlayList = PlayList.First();
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

            if(Video == null) {

                Video = new VideoViewModel(entry.ContentUrl);
                Video.VideoEnded += (obj, e) => {

                    if(Video.IsRepeat) {

                        return;
                    }

                    if(SelectedPlayList == PlayList.Last() && !IsRepeat) {

                        if(Video.IsFullScreen) {

                            Window.GetWindow(Video.FullScreenWebBrowser)?.Close();
                        }
                    } else {

                        Next();
                    }
                };
                Video.CloseRequest += (obj, e) => {

                    App.ViewModelRoot.MainContent.RemoveUserTab(this);
                };
            } else {

                Video.VideoUrl = entry.ContentUrl;
                Video.Initialize();
            }
        }
        
        public void DragOver(IDropInfo dropInfo) {

            if(DefaultDropHandler.CanAcceptData(dropInfo)) {
                // default should always the move action/effect
                var copyData = (dropInfo.DragInfo.DragDropCopyKeyState != default(DragDropKeyStates)) && dropInfo.KeyStates.HasFlag(dropInfo.DragInfo.DragDropCopyKeyState)
                               //&& (dropInfo.DragInfo.VisualSource != dropInfo.VisualTarget)
                               && !(dropInfo.DragInfo.SourceItem is HeaderedContentControl)
                               && !(dropInfo.DragInfo.SourceItem is HeaderedItemsControl)
                               && !(dropInfo.DragInfo.SourceItem is ListBoxItem);
                dropInfo.Effects = copyData ? DragDropEffects.Copy : DragDropEffects.Move;
                var isTreeViewItem = dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter) && dropInfo.VisualTargetItem is TreeViewItem;
                dropInfo.DropTargetAdorner = isTreeViewItem ? DropTargetAdorners.Highlight : DropTargetAdorners.Insert;
            }
        }

        public void Drop(IDropInfo dropInfo) {
            if(dropInfo == null || dropInfo.DragInfo == null) {
                return;
            }
            var insertIndex = dropInfo.InsertIndex != dropInfo.UnfilteredInsertIndex ? dropInfo.UnfilteredInsertIndex : dropInfo.InsertIndex;

            var itemsControl = dropInfo.VisualTarget as ItemsControl;
            if(itemsControl != null) {
                var editableItems = itemsControl.Items as IEditableCollectionView;
                if(editableItems != null) {
                    var newItemPlaceholderPosition = editableItems.NewItemPlaceholderPosition;
                    if(newItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning && insertIndex == 0) {
                        ++insertIndex;
                    } else if(newItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd && insertIndex == itemsControl.Items.Count) {
                        --insertIndex;
                    }
                }
            }

            var destinationList = dropInfo.TargetCollection.TryGetList();
            var data = DefaultDropHandler.ExtractData(dropInfo.Data);

            // default should always the move action/effect
            var copyData = (dropInfo.DragInfo.DragDropCopyKeyState != default(DragDropKeyStates)) && dropInfo.KeyStates.HasFlag(dropInfo.DragInfo.DragDropCopyKeyState)
                           //&& (dropInfo.DragInfo.VisualSource != dropInfo.VisualTarget)
                           && !(dropInfo.DragInfo.SourceItem is HeaderedContentControl)
                           && !(dropInfo.DragInfo.SourceItem is HeaderedItemsControl)
                           && !(dropInfo.DragInfo.SourceItem is ListBoxItem);
            var selected = false;
            if(!copyData) {
                var sourceList = dropInfo.DragInfo.SourceCollection.TryGetList();

                foreach(var o in data) {
                    var index = sourceList.IndexOf(o);

                    var vm = (PlayListEntryViewModel)o;
                    selected = vm.IsSelected;
                    
                    if(index != -1) {
                        sourceList.RemoveAt(index);
                        // so, is the source list the destination list too ?
                        if(Equals(sourceList, destinationList) && index < insertIndex) {
                            --insertIndex;
                        }
                    }
                }
            }
            foreach(var o in data) {
                var obj2Insert = (PlayListEntryViewModel) o;

                destinationList.Insert(insertIndex++, obj2Insert);

                if(selected) {

                    itemsControl.SetSelectedItem(obj2Insert);
                }
            }
        }
    }
}
