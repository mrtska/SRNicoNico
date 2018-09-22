using Codeplex.Data;
using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SRNicoNico.ViewModels {
    public class VideoPopupViewModel : ViewModel {


        #region Thumbnail変更通知プロパティ
        private ImageSource _Thumbnail;

        public ImageSource Thumbnail {
            get { return _Thumbnail; }
            set { 
                if (_Thumbnail == value)
                    return;
                _Thumbnail = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Title変更通知プロパティ
        private string _Title;

        public string Title {
            get { return _Title; }
            set {
                if (_Title == value)
                    return;
                _Title = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Description変更通知プロパティ
        private string _Description;

        public string Description {
            get { return _Description; }
            set { 
                if (_Description == value)
                    return;
                _Description = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region PostedAt変更通知プロパティ
        private DateTime _PostedAt;

        public DateTime PostedAt {
            get { return _PostedAt; }
            set { 
                if (_PostedAt == value)
                    return;
                _PostedAt = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Duration変更通知プロパティ
        private int _Duration;

        public int Duration {
            get { return _Duration; }
            set { 
                if (_Duration == value)
                    return;
                _Duration = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region LoadFailed変更通知プロパティ
        private bool _LoadFailed = false;

        public bool LoadFailed {
            get { return _LoadFailed; }
            set { 
                if (_LoadFailed == value)
                    return;
                _LoadFailed = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region IsActive変更通知プロパティ
        private bool _IsActive = true;

        public bool IsActive {
            get { return _IsActive; }
            set { 
                if (_IsActive == value)
                    return;
                _IsActive = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private readonly NicoNicoVideoWithPlaylistToken Model;

        public VideoPopupViewModel(string id, string token) {

            Model = new NicoNicoVideoWithPlaylistToken(id, token);
            Initialize();
        }

        private async void Initialize() {

            var str = await Model.Initialize();
            if(string.IsNullOrEmpty(str)) {

                LoadFailed = true;
                return;
            }
            var json = DynamicJson.Parse(str);

            Title = json.video.title;
            Thumbnail = new BitmapImage(new Uri(json.video.largeThumbnailURL ?? json.video.thumbnailURL));
            PostedAt = DateTime.Parse(json.video.postedDateTime);
            Description = HyperLinkReplacer.Replace(json.video.description);
            Duration = (int)json.video.duration;
            IsActive = false;
        }

    }
}
