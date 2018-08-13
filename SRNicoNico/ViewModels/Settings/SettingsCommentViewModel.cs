using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.ViewModels {
    public class SettingsCommentViewModel : TabItemViewModel {

        #region CommentAlpha変更通知プロパティ
        public float CommentAlpha {
            get { return Settings.Instance.CommentAlpha; }
            set {
                if(Settings.Instance.CommentAlpha == value)
                    return;
                Settings.Instance.CommentAlpha = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region DefaultCommentSize変更通知プロパティ
        public string DefaultCommentSize {
            get { return Settings.Instance.CommentSize; }
            set {
                if(Settings.Instance.CommentSize == value)
                    return;
                Settings.Instance.CommentSize = value;
                RaisePropertyChanged();
                ApplyConfig();
            }
        }
        #endregion

        #region Hide3DSComment変更通知プロパティ
        public bool Hide3DSComment {
            get { return Settings.Instance.Hide3DSComment; }
            set {
                if(Settings.Instance.Hide3DSComment == value)
                    return;
                Settings.Instance.Hide3DSComment = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region HideWiiUComment変更通知プロパティ
        public bool HideWiiUComment {
            get { return Settings.Instance.HideWiiUComment; }
            set {
                if(Settings.Instance.HideWiiUComment == value)
                    return;
                Settings.Instance.HideWiiUComment = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region NGSharedLevel変更通知プロパティ
        public string NGSharedLevel {
            get { return Settings.Instance.NGSharedLevel; }
            set {
                if(Settings.Instance.NGSharedLevel == value)
                    return;
                Settings.Instance.NGSharedLevel = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public SettingsCommentViewModel() : base("コメント") {

        }

        public void ApplyConfig() {

            foreach(var video in App.ViewModelRoot.MainContent.VideoView.VideoList) {

                video.Comment.ApplyFontSize();
            }
        }
    }
}
