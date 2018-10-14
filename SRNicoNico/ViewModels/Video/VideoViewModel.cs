using Livet;
using Livet.Messaging;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class VideoViewModel : TabItemViewModel {

        public event EventHandler CloseRequest;

        public event EventHandler VideoEnded;
        internal void VideoEnd() {

            VideoEnded?.Invoke(this, EventArgs.Empty);
        }
        internal bool IsPlayList {

            get {
                //プレイリストはこのイベントを使う
                //もうちょっとなんとかならんかったんかな・・・
                return VideoEnded != null;
            }
        }
        #region VideoUrl変更通知プロパティ
        private string _VideoUrl;

        public string VideoUrl {
            get { return _VideoUrl; }
            set { 
                if (_VideoUrl == value)
                    return;
                _VideoUrl = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region ControllerState変更通知プロパティ
        private bool _ControllerState = false;

        public bool ControllerState {
            get { return _ControllerState; }
            set { 
                if (_ControllerState == value)
                    return;
                _ControllerState = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Comment変更通知プロパティ
        private VideoCommentViewModel _Comment;

        public VideoCommentViewModel Comment {
            get { return _Comment; }
            set { 
                if (_Comment == value)
                    return;
                _Comment = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Mylist変更通知プロパティ
        private VideoMylistViewModel _Mylist;

        public VideoMylistViewModel Mylist {
            get { return _Mylist; }
            set { 
                if (_Mylist == value)
                    return;
                _Mylist = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Html5Handler変更通知プロパティ
        private VideoHtml5Handler _Html5Handler;

        public VideoHtml5Handler Html5Handler {
            get { return _Html5Handler; }
            set { 
                if (_Html5Handler == value)
                    return;
                _Html5Handler = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SplitterHeight変更通知プロパティ

        public GridLength SplitterHeight {
            get { return Settings.Instance.SplitterHeight; }
            set {
                if (Settings.Instance.SplitterHeight.Value == value.Value)
                    return;
                Settings.Instance.SplitterHeight = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region ShowFullScreenDescription変更通知プロパティ
        private bool _ShowFullScreenDescription = false;

        public bool ShowFullScreenDescription {
            get { return _ShowFullScreenDescription; }
            set { 
                if (_ShowFullScreenDescription == value)
                    return;
                _ShowFullScreenDescription = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Favorited変更通知プロパティ
        private bool _Favorited;

        public bool Favorited {
            get { return _Favorited; }
            set { 
                if (_Favorited == value)
                    return;
                _Favorited = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public override string Status {
            get {
                return base.Status;
            }
            set {
                base.Status = value;
                if (App.ViewModelRoot.MainContent.VideoTab.VideoList.Contains(this)) {

                    if (App.ViewModelRoot.MainContent.VideoTab.SelectedList == this) {

                        App.ViewModelRoot.MainContent.VideoTab.Status = value;
                    }
                }
            }
        }

        #region Model変更通知プロパティ
        private NicoNicoVideo _Model;

        public NicoNicoVideo Model {
            get { return _Model; }
            set { 
                if (_Model == value)
                    return;
                _Model = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public VideoViewModel(string url) : base(url) {

            VideoUrl = url;
            Html5Handler = new VideoHtml5Handler();
        }

        public void Initialize(string url) {

            VideoUrl = url;
            Html5Handler?.Dispose();
            Initialize();
        }

        public async void Initialize() {

            IsActive = true;
            Status = "動画情報取得中";
            Model = new NicoNicoVideo(VideoUrl);
            Status = await Model.GetVideoDataAsync();

            if(Model.ApiData == null) {

            } else {

                Name = Model.ApiData.Title;
                Html5Handler.Initialize(this, Model.ApiData);
                Comment = new VideoCommentViewModel(Model.ApiData, Html5Handler);
                Mylist = new VideoMylistViewModel(this);

                if(Model.ApiData.UploaderInfo != null) {

                    Favorited = Model.ApiData.UploaderInfo.Followed;
                }

                IsActive = false;
                ControllerState = true;

                if(Model.ApiData.IsNeedPayment) {

                    App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.VideoPaymentView), new VideoPaymentViewModel(this), TransitionMode.NewOrActive));
                } else {

                    App.ViewModelRoot.History.Refresh();
                }
            }
        }

        /// <summary>
        /// WebBrowserコントロールの初期化が終わった後呼ばれる
        /// </summary>
        internal void PostInitialize() {

            Comment.PostInitialize();
        }

        public void TogglePlay() {

            Html5Handler.IsPlaying ^= true;
        }
        public void Restart() {

            Html5Handler?.Seek(0);
        }

        public void ToggleRepeat() {

            Html5Handler.IsRepeat ^= true;
        }

        public void ToggleMute() {

            Html5Handler.IsMute ^= true;
        }

        public void ToggleComment() {

            if (Comment != null) {

                Comment.CommentVisibility ^= true;
            }
        }

        public async void ToggleFollow() {

            if(Model.ApiData.UploaderInfo.IsChannel) {

                if(await Model.FavChannelAsync(!Favorited)) {

                    Favorited = !Favorited;
                    Status = Favorited ? "フォローしました" : "フォローを解除しました";
                }
            } else {

                if (await Model.FavUserAsync(!Favorited)) {

                    Favorited = !Favorited;
                    Status = Favorited ? "フォローしました" : "フォローを解除しました";
                }
            }
        }

        public void Seek(double request) {

            Html5Handler.Seek(request);
        }


        public void Refresh() {

            Html5Handler?.Dispose();
            Initialize();
        }

        public void Refresh(string url) {

            Html5Handler?.Dispose();
            VideoUrl = url;
            Initialize();
        }

        public void Close() {

            Html5Handler?.Dispose();
            if (IsPlayList) {

                CloseRequest?.Invoke(this, EventArgs.Empty);
            } else {

                //VideoViewから削除 VideoViewに無かったらスルーされる
                App.ViewModelRoot.MainContent.RemoveVideoView(this);
            }
        }

        public override void KeyUp(KeyEventArgs e) {

            if (e.Key == Key.D) {

                ShowFullScreenDescription = false;
            }
        }


        #region TweetPopupOpen変更通知プロパティ
        private bool _TweetPopupOpen;

        public bool TweetPopupOpen {
            get { return _TweetPopupOpen; }
            set {
                if (_TweetPopupOpen == value)
                    return;
                _TweetPopupOpen = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region TweetBrowser変更通知プロパティ
        private WebBrowser _TweetBrowser;

        public WebBrowser TweetBrowser {
            get { return _TweetBrowser; }
            set {
                if (_TweetBrowser == value)
                    return;
                _TweetBrowser = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public void OpenTweetView() {

            if(TweetPopupOpen) {

                TweetPopupOpen = false;
                return;
            }

            var query = new GetRequestQuery("https://twitter.com/intent/tweet");
            query.AddQuery("url", Uri.EscapeDataString("http://www.nicovideo.jp" + new Uri(VideoUrl).AbsolutePath));
            query.AddQuery("hashtags", Model.ApiData.VideoId);
            query.AddQuery("original_referer", Uri.EscapeDataString(VideoUrl));
            query.AddQuery("text", Uri.EscapeDataString(Model.ApiData.Title + "(" + NicoNicoUtil.ConvertTime(Model.ApiData.Duration) + ")"));

            TweetBrowser = new WebBrowser();
            // ツイート検知
            TweetBrowser.Navigating += (obj, e) => {

                if (e.Uri.Query == "") {

                    TweetPopupOpen = false;
                }
            };
            TweetPopupOpen = true;
            TweetBrowser.Navigate(query.TargetUrl);
        }

        public override void KeyDown(KeyEventArgs e) {

            if (IsActive) {

                return;
            }

            if(Comment.Post.IsFocused) {

                return;
            }

            //IMEを無効にする
            if (e.Key == Key.ImeProcessed) {

                InputMethod.Current.ImeState = InputMethodState.Off;
                return;
            }

            if (Html5Handler.IsFullScreen) {

                if (e.Key == Key.D) {

                    ShowFullScreenDescription = true;
                    return;
                }
            }

            switch (e.Key) {
                case Key.F5:
                    Refresh();
                    break;
                case Key.Space:
                    TogglePlay();
                    break;
                case Key.R:
                    ToggleRepeat();
                    break;
                case Key.M:
                    ToggleMute();
                    break;
                case Key.F:
                    Html5Handler?.ToggleFullscreen();
                    break;
                case Key.S:
                    Restart();
                    break;
                case Key.Down:
                    Html5Handler.Volume -= 5;
                    break;
                case Key.Up:
                    Html5Handler.Volume += 5;
                    break;
                case Key.Right:
                    Html5Handler?.Seek(Html5Handler.CurrentTime + 10);
                    break;
                case Key.Left:
                    Html5Handler?.Seek(Html5Handler.CurrentTime - 10);
                    break;
                case Key.C:
                    ToggleComment();
                    break;
            }
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control) {

                switch (e.Key) {
                    case Key.R:
                        Refresh();
                        break;
                    case Key.W:
                        Close();
                        e.Handled = true;
                        break;
                }
            }
        }
    }
}
