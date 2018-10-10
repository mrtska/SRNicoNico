using Codeplex.Data;
using Livet;
using Livet.Messaging;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Views.Controls;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace SRNicoNico.ViewModels {
    public class VideoHtml5Handler : NotificationObject, IObjectForScriptable, IDisposable {

        [DllImport("urlmon.dll")]
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Error)]
        static extern int CoInternetSetFeatureEnabled(int featureEntry, [MarshalAs(UnmanagedType.U4)] int dwFlags, bool fEnable);

        public const int FEATURE_LOCALMACHINE_LOCKDOWN = 8;
        public const int SET_FEATURE_ON_PROCESS = 0x00000002;

        #region WebBrowser変更通知プロパティ
        private WebBrowser _WebBrowser;

        public WebBrowser WebBrowser {
            get { return _WebBrowser; }
            set { 
                if (_WebBrowser == value)
                    return;
                _WebBrowser = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region ContentControl変更通知プロパティ
        private ContentControl _ContentControl = new ContentControl();

        public ContentControl ContentControl {
            get { return _ContentControl; }
            set { 
                if (_ContentControl == value)
                    return;
                _ContentControl = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region FullScreenContentControl変更通知プロパティ
        private ContentControl _FullScreenContentControl = new ContentControl();

        public ContentControl FullScreenContentControl {
            get { return _FullScreenContentControl; }
            set { 
                if (_FullScreenContentControl == value)
                    return;
                _FullScreenContentControl = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region IsPlaying変更通知プロパティ
        private bool _IsPlaying;
        public bool IsPlaying {
            get { return _IsPlaying; }
            set { 
                if (_IsPlaying == value)
                    return;
                _IsPlaying = value;
                RaisePropertyChanged();
                if(value) {

                    Resume();
                } else {

                    Pause();
                }
            }
        }
        #endregion

        #region IsFullScreen変更通知プロパティ
        private bool _IsFullScreen = false;

        public bool IsFullScreen {
            get { return _IsFullScreen; }
            set {
                if (_IsFullScreen == value)
                    return;
                _IsFullScreen = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CurrentTime変更通知プロパティ
        private double _CurrentTime;

        public double CurrentTime {
            get { return _CurrentTime; }
            set { 
                if (_CurrentTime == value)
                    return;
                _CurrentTime = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region PlayedRange変更通知プロパティ
        private ObservableSynchronizedCollection<TimeRange> _PlayedRange = new ObservableSynchronizedCollection<TimeRange>();

        public ObservableSynchronizedCollection<TimeRange> PlayedRange {
            get { return _PlayedRange; }
            set {
                if (_PlayedRange == value)
                    return;
                _PlayedRange = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region BufferedRange変更通知プロパティ
        private ObservableSynchronizedCollection<TimeRange> _BufferedRange = new ObservableSynchronizedCollection<TimeRange>();

        public ObservableSynchronizedCollection<TimeRange> BufferedRange {
            get { return _BufferedRange; }
            set {
                if (_BufferedRange == value)
                    return;
                _BufferedRange = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region VolumeIcon変更通知プロパティ
        private string _VolumeIcon;

        public string VolumeIcon {
            get { return _VolumeIcon; }
            set { 
                if (_VolumeIcon == value)
                    return;
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

        #region Volume変更通知プロパティ
        private int _Volume = 0;

        public int Volume {
            get { return _Volume; }
            set {
                if (value > 100) {

                    value = 100;
                } else if (value < 0) {

                    value = 0;
                }
                _Volume = value;
                Settings.Instance.Volume = value;
                RaisePropertyChanged();
                ApplyVolume();
            }
        }
        #endregion

        #region IsRepeat変更通知プロパティ
        private bool _IsRepeat;

        public bool IsRepeat {
            get { return _IsRepeat; }
            set {
                if (_IsRepeat == value)
                    return;
                _IsRepeat = value;
                Settings.Instance.IsRepeat = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CurrentVideoQuality変更通知プロパティ
        private NicoNicoDmcVideoQuality _CurrentVideoQuality;

        public NicoNicoDmcVideoQuality CurrentVideoQuality {
            get { return _CurrentVideoQuality; }
            set { 
                if (_CurrentVideoQuality == value)
                    return;
                _CurrentVideoQuality = value;
                RaisePropertyChanged();
                ReestablishDmcSession();
            }
        }
        #endregion

        #region CurrentAudioQuality変更通知プロパティ
        private NicoNicoDmcAudioQuality _CurrentAudioQuality;

        public NicoNicoDmcAudioQuality CurrentAudioQuality {
            get { return _CurrentAudioQuality; }
            set { 
                if (_CurrentAudioQuality == value)
                    return;
                _CurrentAudioQuality = value;
                RaisePropertyChanged();
                ReestablishDmcSession();
            }
        }
        #endregion

        #region PlayRate変更通知プロパティ
        private double _PlayRate = 1.0D;

        public double PlayRate {
            get { return _PlayRate; }
            set { 
                if (_PlayRate == value)
                    return;
                _PlayRate = value;
                RaisePropertyChanged();
                WebBrowser?.InvokeScript("Video$SetRate", value);
            }
        }
        #endregion

        #region Resolution変更通知プロパティ
        private string _Resolution;

        public string Resolution {
            get { return _Resolution; }
            set {
                if (_Resolution == value)
                    return;
                _Resolution = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region ShowFullScreenController変更通知プロパティ
        private bool _ShowFullScreenController;

        public bool ShowFullScreenController {
            get { return _ShowFullScreenController; }
            set { 
                if (_ShowFullScreenController == value)
                    return;
                _ShowFullScreenController = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region ShowFullScreenCommentBar変更通知プロパティ
        private bool _ShowFullScreenCommentBar;

        public bool ShowFullScreenCommentBar {
            get { return _ShowFullScreenCommentBar; }
            set { 
                if (_ShowFullScreenCommentBar == value)
                    return;
                _ShowFullScreenCommentBar = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        //新仕様動画で使う
        private Timer DmcHeartBeatTimer;

        private VideoViewModel Owner;
        private NicoNicoWatchApi ApiData;
        

        private VideoCommentViewModel Comment;

        public VideoHtml5Handler() {

        }

        private async void ReestablishDmcSession() {

            if (DmcHeartBeatTimer != null) {

                // Timerが既に動いていた場合セッションを削除する
                DmcHeartBeatTimer.Stop();
                DmcHeartBeatTimer.Dispose();
                await ApiData.DmcInfo.DeleteAsync();

                // 再度Dmc用TokenとSignatureを取得する
                var video = new NicoNicoVideoWithPlaylistToken(ApiData.VideoId, ApiData.PlaylistToken);
                var str = await video.Initialize();
                if(string.IsNullOrEmpty(str)) {

                    return;
                }
                var json = DynamicJson.Parse(str);
                ApiData.DmcInfo = new NicoNicoDmc(json.video.dmcInfo.session_api);

                await EstablishDmcSession();
                // セッションを張り直したのでWebBrowser側に伝える
                WebBrowser.InvokeScript("Video$Initialize", new object[] { ApiData.VideoUrl, CurrentTime, IsPlaying });
                ApplyVolume();
            }
        }

        private async Task EstablishDmcSession() {

            // ハートビート開始
            if (ApiData.DmcHeartbeatRequired) {

                if (CurrentVideoQuality == null || CurrentAudioQuality == null) {

                    throw new NullReferenceException("Qualityをnullには出来ません");
                }

                ApiData.VideoUrl = await ApiData.DmcInfo.CreateAsync(CurrentVideoQuality, CurrentAudioQuality);

                DmcHeartBeatTimer = new Timer(ApiData.DmcInfo.HeartbeatLifeTime / 3);
                DmcHeartBeatTimer.Elapsed += async (state, e) => await ApiData.DmcInfo.HeartbeatAsync();
                DmcHeartBeatTimer.Start();
            }
        }

        internal async void Initialize(VideoViewModel vm, NicoNicoWatchApi api) {

            Owner = vm ?? throw new ArgumentNullException(nameof(vm));
            ApiData = api ?? throw new ArgumentNullException(nameof(api));
            if (ApiData.DmcHeartbeatRequired) {

                CurrentVideoQuality = ApiData.DmcInfo.Videos.First();
                CurrentAudioQuality = ApiData.DmcInfo.Audios.First();
                await EstablishDmcSession();
            }

            IsRepeat = Settings.Instance.IsRepeat;

            WebBrowser = new WebBrowser();
            CoInternetSetFeatureEnabled(FEATURE_LOCALMACHINE_LOCKDOWN, SET_FEATURE_ON_PROCESS, false);

            if(IsFullScreen) {

                FullScreenContentControl.Content = WebBrowser;
            } else {

                ContentControl.Content = WebBrowser;
            }
            WebBrowser.ObjectForScripting = new ObjectForScripting(this);
            WebBrowser.Navigate(GetHtml5PlayerPath());
        }

        public void Resume() {

            WebBrowser.InvokeScript("Video$Resume");
        }
        public void Pause() {

            WebBrowser.InvokeScript("Video$Pause");
        }

        internal void Seek(double pos) {

            if (pos < 0) {

                pos = 0;
            } else if (pos > ApiData.Duration) {

                pos = ApiData.Duration;
            }
            WebBrowser.InvokeScript("Video$Seek", new object[] { pos });
        }
        private void SetVolumeIcon() {

            if (IsMute) {

                VolumeIcon = "Mute";
                return;
            }
            if (Volume == 0) {

                VolumeIcon = "s0";
            } else if (Volume < 30) {

                VolumeIcon = "s30";
            } else if (Volume < 80) {

                VolumeIcon = "s80";
            } else if (Volume <= 100) {

                VolumeIcon = "s100";
            }
        }

        private void ApplyVolume() {

            SetVolumeIcon();
            if (IsMute) {

                WebBrowser.InvokeScript("Video$SetVolume", 0);
            } else {

                WebBrowser.InvokeScript("Video$SetVolume", Volume / 100.0);
            }
        }

        public void ToggleFullscreen() {

            if(IsFullScreen) {

                ReturnToNormalScreen();
            } else {

                if (Settings.Instance.UseWindowMode) {

                    EnterWindowFullScreen();
                } else {

                    EnterFullScreen();
                }
            }
        }

        public void EnterFullScreen() {

            if(IsFullScreen) {

                return;
            }
            IsFullScreen = true;
            ContentControl.Content = null;
            FullScreenContentControl.Content = WebBrowser;
            App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.VideoFullScreen), Owner, TransitionMode.Normal));

            Window.GetWindow(ContentControl).Visibility = Visibility.Hidden;

        }
        public void EnterWindowFullScreen() {

            if (IsFullScreen) {

                return;
            }
            IsFullScreen = true;
            ContentControl.Content = null;
            FullScreenContentControl.Content = WebBrowser;
            // App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.VideoWindowFullScreen), Owner, TransitionMode.UnKnown));
            new Views.VideoWindowFullScreen() {

                DataContext = Owner,
                Visibility = Visibility.Visible
            };
        }

        public void ReturnToNormalScreen() {

            if (!IsFullScreen) {

                return;
            }
            IsFullScreen = false;
            FullScreenContentControl.Content = null;
            ContentControl.Content = WebBrowser;
            Window.GetWindow(ContentControl).Visibility = Visibility.Visible;
            Window.GetWindow(FullScreenContentControl)?.Close();
        }



        public void AttachComment(VideoCommentViewModel comment) {

            Comment = comment;
        }

        public void Invoked(string cmd, string args) {

            switch(cmd) {
                case "ready": // ブラウザ側の準備が出来た
                    WebBrowser.InvokeScript("Video$Initialize", new object[] { ApiData.VideoUrl, 0, Settings.Instance.AutoPlay || Owner.IsPlayList });
                    Volume = Settings.Instance.Volume;
                    // 再生速度をUIと同期
                    WebBrowser?.InvokeScript("Video$SetRate", PlayRate);
                    Owner.PostInitialize();
                    break;
                case "widtheight":
                    Resolution = args;
                    break;
                case "playstate":
                    IsPlaying = bool.Parse(args);
                    break;
                case "click":
                    if (Settings.Instance.ClickOnPause) {

                        IsPlaying ^= true;
                    }
                    break;
                case "currenttime": {

                        dynamic json = DynamicJson.Parse(args);
                        CurrentTime = json.time;
                        Comment?.CommentTick((int) json.vpos);

                        if (json.played()) {

                            PlayedRange.Clear();
                            foreach (var play in json.played) {

                                var time = new TimeRange() { StartTime = play.start, EndTime = play.end };
                                var temp = ApiData.Duration / (time.EndTime - time.StartTime);
                                var width = IsFullScreen ? FullScreenContentControl.ActualWidth : ContentControl.ActualWidth;

                                time.Start = width / (ApiData.Duration / time.StartTime);
                                time.Width = width / temp;
                                PlayedRange.Add(time);
                            }
                        }
                        if (json.buffered()) {

                            BufferedRange.Clear();
                            foreach (var buffer in json.buffered) {

                                var time = new TimeRange() { StartTime = buffer.start, EndTime = buffer.end };
                                var temp = ApiData.Duration / (time.EndTime - time.StartTime);
                                var width = IsFullScreen ? FullScreenContentControl.ActualWidth : ContentControl.ActualWidth;

                                time.Start = width / (ApiData.Duration / time.StartTime);
                                time.Width = width / temp;
                                BufferedRange.Add(time);
                            }
                        }
                        break;
                    }
                case "mousewheel":
                    if (!string.IsNullOrEmpty(args)) {

                        var vol = int.Parse(args);

                        if (vol >= 0) {

                            Volume += 2;
                        } else {

                            Volume -= 2;
                        }
                    }
                    break;
                case "alldataloaded":
                    if(ApiData.DmcHeartbeatRequired) {
                        DmcHeartBeatTimer.Enabled = false;
                    }
                    break;
                case "ended": {

                        if (IsRepeat) {

                            Seek(0);
                            Resume();
                        } else {

                            if (IsFullScreen && !Owner.IsPlayList) {

                                ReturnToNormalScreen();
                            }
                        }
                        if (ApiData != null) {

                            //現在再生位置を初期化する
                            //WatchiApiInstance.RecordPlaybackPositionAsync(ApiData, 0);
                        }
                        Owner.VideoEnd();
                        break;
                    }
                case "showcontroller": {

                        ShowFullScreenController = true;
                        ShowFullScreenCommentBar = true;
                        break;
                    }
                case "hidecontroller": {

                        if (!Settings.Instance.AlwaysShowSeekBar) {

                            ShowFullScreenController = false;
                        }
                        if(!Owner.Comment.Post.IsFocused) {
                            ShowFullScreenCommentBar = false;
                        }
                        break;
                    }
                default:
                    Console.WriteLine("cmd: " + cmd + " args: " + args);
                    break;
            }
        }

        private string GetHtml5PlayerPath() {

            var cur = NicoNicoUtil.CurrentDirectory;
            return Path.Combine(cur, "Html/videohtml5.html");
        }

        public void Dispose() {

            WebBrowser.Dispose();
            WebBrowser = null;
            DmcHeartBeatTimer?.Dispose();
            DmcHeartBeatTimer = null;
        }
    }
}
