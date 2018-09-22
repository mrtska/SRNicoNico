using Livet;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class VideoCommentListViewModel : ViewModel {

        #region ListName変更通知プロパティ
        private string _ListName;

        public string ListName {
            get { return _ListName; }
            set {
                if (_ListName == value)
                    return;
                _ListName = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CommentList変更通知プロパティ
        private ObservableSynchronizedCollection<VideoCommentEntryViewModel> _CommentList;

        public ObservableSynchronizedCollection<VideoCommentEntryViewModel> CommentList {
            get { return _CommentList; }
            set { 
                if (_CommentList == value)
                    return;
                _CommentList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public NicoNicoCommentList ListInstance;

        public VideoCommentListViewModel(NicoNicoCommentList instance, VideoHtml5Handler handler) {

            ListInstance = instance;
            ListName = instance.ListName;
            CommentList = new ObservableSynchronizedCollection<VideoCommentEntryViewModel>();
            foreach(var entry in instance.CommentList) {

                //コメントをふるいにかける
                App.ViewModelRoot.Setting.NGFilter.Filter(entry);

                if (entry.Rejected) {

                    continue;
                }
                CommentList.Add(new VideoCommentEntryViewModel(entry, handler));
            }
        }
    }
}
