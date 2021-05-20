using System;
using Livet;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// 動画のタブ管理用のViewModel
    /// </summary>
    public class VideoTabViewModel : TabItemViewModel {

        private DispatcherCollection<VideoViewModel> _TabItems = new DispatcherCollection<VideoViewModel>(App.UIDispatcher);
        /// <summary>
        /// 動画のタブのリスト
        /// </summary>
        public DispatcherCollection<VideoViewModel> TabItems {
            get { return _TabItems; }
            set { 
                if (_TabItems == value)
                    return;
                _TabItems = value;
                RaisePropertyChanged();
            }
        }

        private VideoViewModel? _SelectedItem;
        /// <summary>
        /// 現在選択中のタブ
        /// </summary>
        public VideoViewModel? SelectedItem {
            get { return _SelectedItem; }
            set { 
                if (_SelectedItem == value)
                    return;
                _SelectedItem = value;
                RaisePropertyChanged();
            }
        }

        public VideoTabViewModel(Action collectionClearedAction) : base("動画") {

            // 動画が0になった時にアクションを実行する
            TabItems.CollectionChanged += (o, e) => {
                if (TabItems.Count == 0) {
                    collectionClearedAction();
                }
            };
        }

        /// <summary>
        /// タブのリストに動画を追加する
        /// </summary>
        /// <param name="vm">動画のViewModel</param>
        public void Add(VideoViewModel vm) {

            TabItems.Add(vm);
            // 動画タブがDisposeされたら自動的にリストから削除する
            vm.CompositeDisposable.Add(() => {

                TabItems.Remove(vm);
            });
            // タブが追加されたらそのタブを選択状態にする
            SelectedItem = vm;
        }

    }
}
