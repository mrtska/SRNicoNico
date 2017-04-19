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

using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows;
using SRNicoNico.Models.NicoNicoViewer;
using System.Windows.Controls;
using SRNicoNico.Views.Controls;
using System.Windows.Input;
using System.Drawing;
using System.Web;
using System.Diagnostics;

namespace SRNicoNico.ViewModels {
    public class VideoViewModel : TabItemViewModel {

        public event EventHandler CloseRequest;

        public event EventHandler VideoEnded;
        internal void VideoEnd() {

            VideoEnded?.Invoke(this, EventArgs.Empty);
        }
        internal bool IsPlayList() {

            //プレイリストはこのイベントを使う
            //もうちょっとなんとかならんかったんかな・・・
            return VideoEnded != null;
        }

        public override string Status {
            get {
                return base.Status;
            }
            set {
                base.Status = value;
                if(App.ViewModelRoot.MainContent.VideoView.VideoList.Contains(this)) {

                    if(App.ViewModelRoot.MainContent.VideoView.SelectedList == this) {

                        App.ViewModelRoot.MainContent.VideoView.Status = value;
                    }   
                }
            }
        }


        #region VideoUrl変更通知プロパティ
        private string _VideoUrl;

        public string VideoUrl {
            get { return _VideoUrl; }
            set { 
                if(_VideoUrl == value)
                    return;
                _VideoUrl = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        protected internal NicoNicoWatchApi WatchiApiInstance;

        private NicoNicoStoryBoard StoryBoardInstance;

        internal VideoHtml5Handler Handler;

        #region ApiData変更通知プロパティ
        private NicoNicoWatchApiData _ApiData;

        public NicoNicoWatchApiData ApiData {
            get { return _ApiData; }
            set { 
                if(_ApiData == value)
                    return;
                _ApiData = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        #region StoryBoardList変更通知プロパティ
        private NicoNicoStoryBoardData _StoryBoardList;

        public NicoNicoStoryBoardData StoryBoardList {
            get { return _StoryBoardList; }
            set { 
                if(_StoryBoardList == value)
                    return;
                _StoryBoardList = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Resolution変更通知プロパティ
        private string _Resolution;

        public string Resolution {
            get { return _Resolution; }
            set { 
                if(_Resolution == value)
                    return;
                _Resolution = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region TagList変更通知プロパティ
        private DispatcherCollection<VideoTagViewModel> _TagList = new DispatcherCollection<VideoTagViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<VideoTagViewModel> TagList {
            get { return _TagList; }
            set { 
                if(_TagList == value)
                    return;
                _TagList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region SplitterHeight変更通知プロパティ

        public GridLength SplitterHeight {
            get { return Settings.Instance.SplitterHeight; }
            set { 
                if(Settings.Instance.SplitterHeight.Value == value.Value)
                    return;
                Settings.Instance.SplitterHeight = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Comment変更通知プロパティ
        private VideoCommentViewModel _Comment;

        public VideoCommentViewModel Comment {
            get { return _Comment; }
            set { 
                if(_Comment == value)
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

        #region WebBrowser変更通知プロパティ
        private WebBrowser _WebBrowser;

        public WebBrowser WebBrowser {
            get { return _WebBrowser; }
            set { 
                _WebBrowser = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region FullScreenWebBrowser変更通知プロパティ
        private WebBrowser _FullScreenWebBrowser;

        public WebBrowser FullScreenWebBrowser {
            get { return _FullScreenWebBrowser; }
            set { 
                if (_FullScreenWebBrowser == value)
                    return;
                _FullScreenWebBrowser = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Controller変更通知プロパティ
        private Views.VideoController _Controller;

        public Views.VideoController Controller {
            get { return _Controller; }
            set { 
                if(_Controller == value)
                    return;
                _Controller = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region FullScreenController変更通知プロパティ
        private Views.VideoController _FullScreenController;

        public Views.VideoController FullScreenController {
            get { return _FullScreenController; }
            set { 
                if (_FullScreenController == value)
                    return;
                _FullScreenController = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region IsPlaying変更通知プロパティ
        private bool _IsPlaying = false;

        public bool IsPlaying {
            get { return _IsPlaying; }
            set { 
                if(_IsPlaying == value)
                    return;
                _IsPlaying = value;

                //一時停止したら現在再生位置を記録する
                if(!value && ApiData != null) {

                    WatchiApiInstance.RecordPlaybackPositionAsync(ApiData, CurrentTime);
                }
                RaisePropertyChanged();
            }
        }
        #endregion

        #region IsBuffering変更通知プロパティ
        private bool _IsBuffering = false;

        public bool IsBuffering {
            get { return _IsBuffering; }
            set { 
                if(_IsBuffering == value)
                    return;
                if(value) {
                    Status = "バッファリング中";
                } else {
                    Status = "";
                }
                _IsBuffering = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region IsFullScreen変更通知プロパティ
        private bool _IsFullScreen = false;

        public bool IsFullScreen {
            get { return _IsFullScreen; }
            set { 
                if(_IsFullScreen == value)
                    return;
                _IsFullScreen = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region ShowFullScreenController変更通知プロパティ
        private bool _ShowFullScreenController = true;

        public bool ShowFullScreenController {
            get { return _ShowFullScreenController; }
            set { 
                if(_ShowFullScreenController == value)
                    return;
                _ShowFullScreenController = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region IsRepeat変更通知プロパティ
        public bool IsRepeat {
            get { return Settings.Instance.IsRepeat; }
            set { 
                if(Settings.Instance.IsRepeat == value)
                    return;
                Settings.Instance.IsRepeat = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Volume変更通知プロパティ

        public int Volume {
            get { return Settings.Instance.Volume; }
            set {

                if(value > 100) {

                    value = 100;
                } else if(value < 0) {

                    value = 0;
                }
                Settings.Instance.Volume = value;
                IsMute = false;
                ApplyVolume();
                RaisePropertyChanged();
            }
        }
        #endregion

        #region VolumeIcon変更通知プロパティ
        private string _VolumeIcon;

        public string VolumeIcon {
            get { return _VolumeIcon; }
            set { 
                _VolumeIcon = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region IsMute変更通知プロパティ
        public bool IsMute {
            get { return Settings.Instance.IsMute; }
            set { 
                Settings.Instance.IsMute = value;
                ApplyVolume();
                RaisePropertyChanged();
            }
        }
        #endregion

        #region ChangePlayRateAvalilable変更通知プロパティ
        private PlayBackRateAvalilableReason _ChangePlayRateAvalilable = PlayBackRateAvalilableReason.PremiumOnly;

        public PlayBackRateAvalilableReason ChangePlayRateAvalilable {
            get { return _ChangePlayRateAvalilable; }
            set { 
                if(_ChangePlayRateAvalilable == value)
                    return;
                _ChangePlayRateAvalilable = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region PlayRate変更通知プロパティ
        private double _PlayRate = 1.0D;

        public double PlayRate {
            get { return _PlayRate; }
            set { 
                if(_PlayRate == value)
                    return;

                _PlayRate = value;
                
                Handler?.InvokeScript("VideoViewModel$setrate", value.ToString());
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CurrentTime変更通知プロパティ
        private double _CurrentTime;

        public double CurrentTime {
            get { return _CurrentTime; }
            set { 
                if(_CurrentTime == value || ApiData == null || value > ApiData.Video.Duration)
                    return;
                _CurrentTime = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region PlayedRange変更通知プロパティ
        private DispatcherCollection<TimeRange> _PlayedRange = new DispatcherCollection<TimeRange>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<TimeRange> PlayedRange {
            get { return _PlayedRange; }
            set { 
                if(_PlayedRange == value)
                    return;
                _PlayedRange = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region BufferedRange変更通知プロパティ
        private DispatcherCollection<TimeRange> _BufferedRange = new DispatcherCollection<TimeRange>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<TimeRange> BufferedRange {
            get { return _BufferedRange; }
            set { 
                if(_BufferedRange == value)
                    return;
                _BufferedRange = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region ShowFullScreenDescription変更通知プロパティ
        private bool _ShowFullScreenDescription = false;

        public bool ShowFullScreenDescription {
            get { return _ShowFullScreenDescription; }
            set {
                if(ShowFullScreenDescription == value)
                    return;
                _ShowFullScreenDescription = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public VideoViewModel(string url) : base(url) {

            var query = new GetRequestQuery(url);
            query.AddQuery("watch_harmful", 1);

            VideoUrl = query.TargetUrl;
            WatchiApiInstance = new NicoNicoWatchApi(this);
            StoryBoardInstance = new NicoNicoStoryBoard(this);

        }

        public async void Initialize() {

            IsActive = true;

             ApiData = await WatchiApiInstance.GetWatchApiDataAsync();
            
            if(ApiData == null) {

                return;
            }

            Name = ApiData.Video.Title;

            //タグ情報を初期化して追加
            TagList.Clear();
            foreach(var tag in ApiData.Tags) {

                TagList.Add(new VideoTagViewModel(tag, this));
            }

            IsActive = false;
            //コメントを取得
            Comment = new VideoCommentViewModel(this);

            Mylist = new VideoMylistViewModel(this);


            if(IsFullScreen) {

                FullScreenController = new Views.VideoController() {

                    DataContext = this
                };
                FullScreenWebBrowser = new WebBrowser();
                FullScreenWebBrowser.Focusable = false;

                Handler?.Dispose();
                Handler = new VideoHtml5Handler(FullScreenWebBrowser);
                Handler.Initialize(this);
            } else {

                Controller = new Views.VideoController() {

                    DataContext = this
                };
                WebBrowser = new WebBrowser();
                WebBrowser.Focusable = false;

                Handler?.Dispose();
                Handler = new VideoHtml5Handler(WebBrowser);
                Handler.Initialize(this);
            }
            
            await Comment.Initialize();

            //画像処理をUIスレッドでやられると重いので
            await Task.Run(async () => StoryBoardList = await StoryBoardInstance.GetVideoStoryBoardAsync(ApiData.Video.SmileInfo.Url));

        }

        public void Refresh() {

            Initialize();
        }

        public void Restart() {

            Handler?.Seek(0);
        }

        public void TogglePlay() {

            if(IsPlaying) {

                Handler?.Pause();
            } else {

                Handler?.Play();
            }
        }
        public void ToggleComment() {

            if(Comment != null) {

                Comment.CommentVisibility ^= true;
            }
        }

        public void ToggleRepeat() {

            IsRepeat ^= true;
        }
        
        public void ToggleMute() {

            IsMute ^= true;
        }

        public void EnterFullScreen() {

            if(IsFullScreen) {

                Window.GetWindow(FullScreenWebBrowser)?.Close();
                return;
            }
            Window.GetWindow(WebBrowser).Visibility = Visibility.Hidden;

            IsFullScreen = true;

            var temp = WebBrowser;
            WebBrowser = null;
            FullScreenWebBrowser = temp;

            var temp2 = Controller;
            Controller = null;
            FullScreenController = temp2;
            App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.VideoFullScreenView), this, TransitionMode.Normal, "Video"));

        }
        public void EnterWindowFullScreen() {

            if(IsFullScreen) {

                Window.GetWindow(FullScreenWebBrowser)?.Close();
                return;
            }

            if(IsPlayList()) {

                Window.GetWindow(WebBrowser).Visibility = Visibility.Hidden;
            } else {

                //VideoViewから削除
                App.ViewModelRoot.MainContent.RemoveVideoView(this);
            }


            IsFullScreen = true;

            var temp = WebBrowser;
            WebBrowser = null;
            FullScreenWebBrowser = temp;

            var temp2 = Controller;
            Controller = null;
            FullScreenController = temp2;
            //App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.WindowFullScreenView), this, TransitionMode.NewOrActive, "Video"));
            new Views.WindowFullScreenView() {

                DataContext = this,
                Visibility = Visibility.Visible
            };
        }

        public async void ToggleFollow() {

            var ret = await WatchiApiInstance?.ToggleFollowOwnerAsync(ApiData);

            if(ret) {

                ApiData.IsUploaderFollowed ^= true;
            }
            
        }


        #region TweetPopupOpen変更通知プロパティ
        private bool _TweetPopupOpen;

        public bool TweetPopupOpen {
            get { return _TweetPopupOpen; }
            set { 
                if(_TweetPopupOpen == value)
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
                if(_TweetBrowser == value)
                    return;
                _TweetBrowser = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public void OpenTweetView() {

            var query = new GetRequestQuery("https://twitter.com/intent/tweet");
            query.AddQuery("url", Uri.EscapeDataString("http://www.nicovideo.jp" + new Uri(VideoUrl).AbsolutePath));
            query.AddQuery("hashtags", ApiData.Video.Id);
            query.AddQuery("original_referer", Uri.EscapeDataString(VideoUrl));
            query.AddQuery("text", Uri.EscapeDataString(ApiData.Video.Title + "(" + NicoNicoUtil.ConvertTime(ApiData.Video.Duration) + ")"));

            TweetBrowser = new WebBrowser();
            TweetBrowser.Navigating += (obj, e) => {

                if(e.Uri.Query == "") {

                    TweetPopupOpen = false;
                }
            };

            TweetPopupOpen = true;
            TweetBrowser.Navigate(query.TargetUrl);
        }


        public void Close() {

            Handler?.Dispose();
            Dispose();

            if(IsPlayList()) {

                CloseRequest?.Invoke(this, EventArgs.Empty);
            } else {

                //VideoViewから削除 VideoViewに無かったらスルーされる
                App.ViewModelRoot.MainContent.RemoveVideoView(this);
            }


        }

        private void SetVolumeIcon() {

            if(IsMute) {

                VolumeIcon = "Mute";
                return;
            }
            if(Volume == 0) {

                VolumeIcon = "s0";
            } else if(Volume < 30) {

                VolumeIcon = "s30";
            } else if(Volume < 80) {

                VolumeIcon = "s80";
            } else if(Volume <= 100) {

                VolumeIcon = "s100";
            }
        }

        protected internal void ApplyVolume() {

            SetVolumeIcon();
            if(IsMute) {

                Handler?.InvokeScript("VideoViewModel$setvolume", (0).ToString());
            } else {

                Handler?.InvokeScript("VideoViewModel$setvolume", (Volume / 100.0).ToString());
            }
        }

        public override void KeyUp(KeyEventArgs e) {

            if(e.Key == Key.D) {

                ShowFullScreenDescription = false;
            }
        }

        public override void KeyDown(KeyEventArgs e) {

            if(IsActive) {

                return;
            }

            if(TweetPopupOpen) {

                if(e.Key == Key.Escape) {

                    TweetPopupOpen = false;
                }
                e.Handled = false;
                return;
            }


            if(Comment.Post.IsCommentPopupOpen) {

                if(e.Key == Key.Escape) {

                    Comment.Post.IsCommentPopupOpen = false;
                    e.Handled = true;
                }

                if(e.Key == Key.Enter) {

                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift) {

                        //Shift+Enterで改行ということにする
                    } else {

                        Comment.Post.Post();
                        Comment.Post.IsCommentPopupOpen = false;
                        e.Handled = true;
                    }
                }
                return;
            }

            //IMEを無効にする
            if(e.Key == Key.ImeProcessed) {

                InputMethod.Current.ImeState = InputMethodState.Off;
                return;
            }

            if(IsFullScreen) {

                if(e.Key == Key.D) {

                    ShowFullScreenDescription = true;
                    return;
                }
            }

            switch(e.Key) {
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
                    if(e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control)) {

                        EnterWindowFullScreen();
                    } else {

                        EnterFullScreen();
                    }
                    break;
                case Key.S:
                    Restart();
                    break;
                case Key.Down:
                    Volume -= 5;
                    break;
                case Key.Up:
                    Volume += 5;
                    break;
                case Key.Right:
                    Handler?.Seek(CurrentTime + 10);
                    break;
                case Key.Left:
                    Handler?.Seek(CurrentTime - 10);
                    break;
                case Key.C:
                    ToggleComment();
                    break;
                case Key.Enter:
                    Comment.Post.IsCommentPopupOpen = true;
                    e.Handled = true;
                    break;
            }
            if(e.KeyboardDevice.Modifiers == ModifierKeys.Control) {

                switch(e.Key) {
                    case Key.R:
                        Refresh();
                        break;
                    case Key.W:
                        Close();
                        break;
                }
            }
        }
    }
}
