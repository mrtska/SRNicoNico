using Livet;
using Livet.Messaging;
using System.Linq;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class VideoTabViewModel : TabItemViewModel {

        #region VideoList変更通知プロパティ
        private ObservableSynchronizedCollection<VideoViewModel> _VideoList;

        public ObservableSynchronizedCollection<VideoViewModel> VideoList {
            get { return _VideoList; }
            set { 
                if (_VideoList == value)
                    return;
                _VideoList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SelectedList変更通知プロパティ
        private VideoViewModel _SelectedList;

        public VideoViewModel SelectedList {
            get { return _SelectedList; }
            set { 
                if (_SelectedList == value)
                    return;
                _SelectedList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public VideoTabViewModel() : base("動画") {

            VideoList = new ObservableSynchronizedCollection<VideoViewModel>();
        }


        public void Add(VideoViewModel vm) {

            VideoList.Add(vm);
            SelectedList = vm;
        }

        // VideoViewから
        public void Remove(VideoViewModel vm) {

            if (VideoList.Contains(vm)) {

                VideoList.Remove(vm);
            }
        }

        public override void KeyDown(KeyEventArgs e) {
            base.KeyDown(e);

            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control)) {

                if (e.Key == Key.Tab) {

                    if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift)) {

                        Prev();
                    } else {

                        Next();
                    }
                    return;
                }
            }
            SelectedList?.KeyDown(e);
        }

        //次の動画へ
        private void Next() {

            if (VideoList.Count == 1) {

                return;
            }
            var index = VideoList.IndexOf(SelectedList);

            if (index + 1 >= VideoList.Count) {

                SelectedList = VideoList.First();
            } else {

                SelectedList = VideoList[index + 1];
            }
        }
        //前の動画へ
        private void Prev() {

            if (VideoList.Count == 1) {

                return;
            }

            var index = VideoList.IndexOf(SelectedList);
            if (index <= 0) {

                SelectedList = VideoList.Last();
            } else {

                SelectedList = VideoList[index - 1];
            }
        }

        public override bool CanShowHelp() {
            return true;
        }
        public override void ShowHelpView(InteractionMessenger Messenger) {

            Messenger.Raise(new TransitionMessage(typeof(Views.VideoHelpView), this, TransitionMode.NewOrActive));
        }
    }
}
