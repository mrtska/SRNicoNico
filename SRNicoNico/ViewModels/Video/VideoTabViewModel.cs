using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
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

        private void OnPropertyChanged(object? o, PropertyChangedEventArgs e) {

            var tabItem = (TabItemViewModel)o!;
            if (e.PropertyName == nameof(Status)) {

                Status = tabItem.Status;
            }
        }

        /// <summary>
        /// タブのリストに動画を追加する
        /// </summary>
        /// <param name="vm">動画のViewModel</param>
        public void Add(VideoViewModel vm) {

            TabItems.Add(vm);

            // ステータスプロパティが更新されたら動画タブのステータスに反映させる
            vm.PropertyChanged += OnPropertyChanged;
            // 動画タブがDisposeされたら自動的にリストから削除する
            vm.CompositeDisposable.Add(() => {

                vm.PropertyChanged -= OnPropertyChanged;
                TabItems.Remove(vm);

                // タブの一番後ろを選択状態にする
                SelectedItem = TabItems.LastOrDefault();
            });
            // タブが追加されたらそのタブを選択状態にする
            SelectedItem = vm;
        }

        public override void KeyDown(KeyEventArgs e) {

            SelectedItem?.KeyDown(e);
        }

        public override void KeyUp(KeyEventArgs e) {

            SelectedItem?.KeyUp(e);
        }
    }
}
