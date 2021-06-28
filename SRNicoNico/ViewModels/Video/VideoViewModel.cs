using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Input;
using Livet;
using Livet.Messaging;
using Livet.Messaging.Windows;
using Microsoft.Web.WebView2.Wpf;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;
using SRNicoNico.Views.Controls;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// 動画再生用のViewModel
    /// </summary>
    [ComVisible(true)]
    public class VideoViewModel : TabItemViewModel {


        private WatchApiData? _ApiData;
        /// <summary>
        /// 動画詳細データ
        /// </summary>
        public WatchApiData? ApiData {
            get { return _ApiData; }
            set { 
                if (_ApiData == value)
                    return;
                _ApiData = value;
                RaisePropertyChanged();
            }
        }

        private VideoCommentViewModel? _Comment;
        /// <summary>
        /// コメント部分のViewModel
        /// </summary>
        public VideoCommentViewModel? Comment {
            get { return _Comment; }
            set { 
                if (_Comment == value)
                    return;
                _Comment = value;
                RaisePropertyChanged();
            }
        }

        private VideoStoryBoard? _StoryBoard;
        /// <summary>
        /// ストーリーボード情報
        /// </summary>
        public VideoStoryBoard? StoryBoard {
            get { return _StoryBoard; }
            set { 
                if (_StoryBoard == value)
                    return;
                _StoryBoard = value;
                RaisePropertyChanged();
            }
        }

        private VideoHtml5Handler? _Html5Handler;

        public VideoHtml5Handler? Html5Handler {
            get { return _Html5Handler; }
            set { 
                if (_Html5Handler == value)
                    return;
                _Html5Handler = value;
                RaisePropertyChanged();
            }
        }

        private double _CurrentTime;
        /// <summary>
        /// 現在の再生時間
        /// </summary>
        public double CurrentTime {
            get { return _CurrentTime; }
            set { 
                if (_CurrentTime == value)
                    return;
                _CurrentTime = value;
                RaisePropertyChanged();
            }
        }

        private float _Volume;
        /// <summary>
        /// 音量
        /// </summary>
        public float Volume {
            get { return _Volume; }
            set { 
                if (_Volume == value)
                    return;
                if (value > 1) {
                    value = 1;
                } else if (value < 0) {
                    value = 0;
                }
                _Volume = value;
                RaisePropertyChanged();
                Settings.CurrentVolume = value;

                if (!IsMuted) {
                    Html5Handler?.SetVolume(value);
                }
            }
        }

        private bool _IsMuted = false;
        /// <summary>
        /// ミュート状態
        /// </summary>
        public bool IsMuted {
            get { return _IsMuted; }
            set { 
                if (_IsMuted == value)
                    return;
                _IsMuted = value;
                RaisePropertyChanged();
                Settings.CurrentIsMute = value;

                if (value) {
                    Html5Handler?.SetVolume(0);
                } else {
                    Html5Handler?.SetVolume(Volume);
                }
            }
        }

        private RepeatBehavior _RepeatBehavior;
        /// <summary>
        /// リピート時の動作
        /// </summary>
        public RepeatBehavior RepeatBehavior {
            get { return _RepeatBehavior; }
            set { 
                if (_RepeatBehavior == value)
                    return;
                _RepeatBehavior = value;
                RaisePropertyChanged();
                Settings.CurrentRepeatBehavior = value;
            }
        }

        private CommentVisibility _CommentVisibility;
        /// <summary>
        /// コメントの表示状態
        /// </summary>
        public CommentVisibility CommentVisibility {
            get { return _CommentVisibility; }
            set { 
                if (_CommentVisibility == value)
                    return;
                _CommentVisibility = value;
                Html5Handler?.SetVisibility(value);
                RaisePropertyChanged();
                Settings.CurrentCommentVisibility = value;
            }
        }

        private double _RepeatA;
        /// <summary>
        /// ABリピートのA地点
        /// </summary>
        public double RepeatA {
            get { return _RepeatA; }
            set { 
                if (_RepeatA == value)
                    return;
                _RepeatA = value;
                RaisePropertyChanged();
                SaveABRepeatPosition();
            }
        }

        private double _RepeatB;
        /// <summary>
        /// ABリピートのB地点
        /// </summary>
        public double RepeatB {
            get { return _RepeatB; }
            set { 
                if (_RepeatB == value)
                    return;
                _RepeatB = value;
                RaisePropertyChanged();
                SaveABRepeatPosition();
            }
        }

        private bool _PlayState;
        /// <summary>
        /// Trueの時は再生中
        /// Falseの時は一時停止
        /// </summary>
        public bool PlayState {
            get { return _PlayState; }
            set { 
                if (_PlayState == value)
                    return;
                _PlayState = value;
                RaisePropertyChanged();
            }
        }


        private bool _IsFullScreen = false;
        /// <summary>
        /// フルスクリーン状態かどうか
        /// </summary>
        public bool IsFullScreen {
            get { return _IsFullScreen; }
            set { 
                if (_IsFullScreen == value)
                    return;
                _IsFullScreen = value;
                RaisePropertyChanged();
            }
        }


        private int? _ActualVideoWidth;
        /// <summary>
        /// 再生している動画の実際の横幅
        /// </summary>
        public int? ActualVideoWidth {
            get { return _ActualVideoWidth; }
            set { 
                if (_ActualVideoWidth == value)
                    return;
                _ActualVideoWidth = value;
                RaisePropertyChanged();
            }
        }

        private int? _ActualVideoHeight;
        /// <summary>
        /// 再生している動画の実際の縦幅
        /// </summary>
        public int? ActualVideoHeight {
            get { return _ActualVideoHeight; }
            set { 
                if (_ActualVideoHeight == value)
                    return;
                _ActualVideoHeight = value;
                RaisePropertyChanged();
            }
        }

        private double _ActualVideoDuration = 0;
        /// <summary>
        /// 実際の動画の長さ
        /// </summary>
        public double ActualVideoDuration {
            get { return _ActualVideoDuration; }
            set { 
                if (_ActualVideoDuration == value)
                    return;
                _ActualVideoDuration = value;
                RaisePropertyChanged();
                // 実際の動画の長さが分かったらABリピートの初期値をロードする
                LoadABRepeatPosition();
            }
        }

        private WebView2? _WebViewControl;
        /// <summary>
        /// 通常時のWebView要素
        /// </summary>
        public WebView2? WebViewControl {
            get { return _WebViewControl; }
            set { 
                if (_WebViewControl == value)
                    return;
                _WebViewControl = value;
                RaisePropertyChanged();
            }
        }

        private WebView2? _FullScreenWebViewControl;
        /// <summary>
        /// 通常時のWebView要素
        /// </summary>
        public WebView2? FullScreenWebViewControl {
            get { return _FullScreenWebViewControl; }
            set {
                if (_FullScreenWebViewControl == value)
                    return;
                _FullScreenWebViewControl = value;
                RaisePropertyChanged();
            }
        }

        private ObservableSynchronizedCollection<TimeRange> _PlayedRange = new ObservableSynchronizedCollection<TimeRange>();
        /// <summary>
        /// 再生済みの時間幅のリスト
        /// </summary>
        public ObservableSynchronizedCollection<TimeRange> PlayedRange {
            get { return _PlayedRange; }
            set { 
                if (_PlayedRange == value)
                    return;
                _PlayedRange = value;
                RaisePropertyChanged();
            }
        }

        private ObservableSynchronizedCollection<TimeRange> _BufferedRange = new ObservableSynchronizedCollection<TimeRange>();
        /// <summary>
        /// バッファ済みの時間幅のリスト
        /// </summary>
        public ObservableSynchronizedCollection<TimeRange> BufferedRange {
            get { return _BufferedRange; }
            set { 
                if (_BufferedRange == value)
                    return;
                _BufferedRange = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// シークバー操作時に呼ばれるアクション
        /// </summary>
        public Action<double> SeekAction { get; set; }

        private DmcSession? DmcSession;
        private Timer? HeartbeatTimer;

        private readonly ISettings Settings;
        private readonly IVideoService VideoService;
        private readonly InteractionMessenger RootMessenger;
        private readonly string VideoId;

        public VideoViewModel(ISettings settings, IVideoService videoService, InteractionMessenger messenger, string videoId) : base(videoId) {

            Settings = settings;
            VideoService = videoService;
            RootMessenger = messenger;
            VideoId = videoId;

            SeekAction = pos => Seek(pos);

            // 設定から初期値を設定しておく
            _Volume = Settings.CurrentVolume;
            _IsMuted = Settings.CurrentIsMute;
            _RepeatBehavior = Settings.CurrentRepeatBehavior;
            _CommentVisibility = Settings.CurrentCommentVisibility;
        }

        /// <summary>
        /// 動画情報をロードする
        /// </summary>
        public async void Loaded() {

            IsActive = true;
            Status = "動画を読込中";
            try {

                ApiData = await VideoService.WatchAsync(VideoId);

                // タブ名を動画IDから動画タイトルに書き換える
                Name = ApiData.Video!.Title;

                // WebViewが既にある場合は破棄する
                if (Html5Handler != null) {

                    CompositeDisposable.Remove(Html5Handler);
                    Html5Handler.Dispose();
                }
                Html5Handler = new VideoHtml5Handler();
                CompositeDisposable.Add(Html5Handler);
                WebViewControl = Html5Handler.WebView;

                DmcSession = await VideoService.CreateSessionAsync(ApiData.Media!.Movie!.Session!);
                await Html5Handler.InitializeAsync(this, DmcSession.ContentUri!);

                // タイマーが既に動いている場合は止める
                if (HeartbeatTimer != null) {
                    CompositeDisposable.Remove(HeartbeatTimer);
                    HeartbeatTimer.Dispose();
                }

                HeartbeatTimer = new Timer(async (_) => {
                    DmcSession = await VideoService.HeartbeatAsync(DmcSession);
                }, null, ApiData.Media.Movie!.Session!.HeartbeatLifetime / 3, ApiData.Media.Movie!.Session!.HeartbeatLifetime / 3);
                CompositeDisposable.Add(HeartbeatTimer);

                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"動画 {VideoId} の再生に失敗しました ステータスコード: {e.StatusCode}";
                return;
            } finally {

                IsActive = false;
            }
            try {

                // コメント部分を初期化する
                Comment = new VideoCommentViewModel(VideoService);
                Comment.Initialize(this, ApiData.Comment!);

            } catch (StatusErrorException e) {

                Status = $"コメントの取得に失敗しました ステータスコード: {e.StatusCode}";
            }
            try {

                // ストーリーボードを取得する
                StoryBoard = await VideoService.GetStoryBoardAsync(ApiData.Media.StoryBoard!.Session!);

            } catch (StatusErrorException e) {

                Status = $"ストーリーボードの取得に失敗しました ステータスコード: {e.StatusCode}";
            }
        }

        private async void LoadABRepeatPosition() {

            var pos = await VideoService.GetABRepeatPositionAsync(VideoId);

            // プロパティに代入するとSaveABRepeatPositionが呼ばれてしまうのでバッキングストアに代入して手動でPropertyChangedイベントを呼ぶ
            if (pos == null) {
                _RepeatA = 0;
                _RepeatB = ActualVideoDuration;
            } else {
                _RepeatA = pos.RepeatA;
                _RepeatB = pos.RepeatB;
            }
            RaisePropertyChanged(nameof(RepeatA));
            RaisePropertyChanged(nameof(RepeatB));
        }

        private async void SaveABRepeatPosition() {

            await VideoService.SaveABRepeatPositionAsync(VideoId, RepeatA, RepeatB);
        }

        /// <summary>
        /// 再生と一時停止を切り替える
        /// </summary>
        public void TogglePlay() {

            Html5Handler?.TogglePlay();
        }

        /// <summary>
        /// 再生していたら一時停止する
        /// </summary>
        public void Pause() {

            if (PlayState) {
                Html5Handler?.TogglePlay();
            }
        }

        /// <summary>
        /// 一時停止していたら再生する
        /// </summary>
        public void Resume() {

            if (!PlayState) {
                Html5Handler?.TogglePlay();
            }
        }

        /// <summary>
        ///  ミュートを切り替える
        /// </summary>
        public void ToggleMute() {

            IsMuted ^= true;
        }

        /// <summary>
        /// リピートを切り替える
        /// </summary>
        public void ToggleRepeat() {

            if (RepeatBehavior == RepeatBehavior.None) {
                RepeatBehavior = RepeatBehavior.Repeat;
            } else if (RepeatBehavior == RepeatBehavior.Repeat) {
                RepeatBehavior = Settings.DisableABRepeat ? RepeatBehavior.None : RepeatBehavior.ABRepeat;
            } else if (RepeatBehavior == RepeatBehavior.ABRepeat) {
                RepeatBehavior = RepeatBehavior.None;
            }
        }

        /// <summary>
        /// コメント表示の切り替え
        /// </summary>
        public void ToggleComment() {

            if (CommentVisibility == CommentVisibility.Visible) {
                CommentVisibility = CommentVisibility.Hidden;
            } else if (CommentVisibility == CommentVisibility.Hidden) {
                CommentVisibility = CommentVisibility.OnlyAuthor;
            } else if (CommentVisibility == CommentVisibility.OnlyAuthor) {
                CommentVisibility = CommentVisibility.Visible;
            }
        }

        /// <summary>
        /// 指定した位置にシークする
        /// </summary>
        /// <param name="position">シークしたい位置 秒</param>
        public void Seek(double position) {

            Html5Handler?.Seek(position);
        }

        /// <summary>
        /// 動画を最初から再生する
        /// </summary>
        public void Restart() {

            Seek(0);
        }

        /// <summary>
        /// フルスクリーン状態を切り替える
        /// </summary>
        public void ToggleFullScreen() {

            if (IsFullScreen) {
                Messenger.Raise(new WindowActionMessage(WindowAction.Close));
            } else {
                IsFullScreen = true;
                RootMessenger.Raise(new TransitionMessage(typeof(Views.VideoFullScreen), this, TransitionMode.Normal));

                FullScreenWebViewControl = Html5Handler?.WebView;
                WebViewControl = null;
            }
        }

        public void BackFromFullScreen() {

            IsFullScreen = false;

            WebViewControl = Html5Handler?.WebView;
            FullScreenWebViewControl = null;
        }

        /// <summary>
        /// 動画タブを閉じる
        /// </summary>
        public void Close() {

            Dispose();
        }

        /// <summary>
        /// 動画を再読み込みする
        /// </summary>
        public void Reload() {

            Loaded();
        }

        public override void KeyDown(KeyEventArgs e) {

            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control)) {
                // Ctrl+Wで閉じる
                if (e.Key == Key.W) {
                    Close();
                    return;
                }
            }

            if (IsFullScreen && e.Key == Key.Escape) {
                Messenger.Raise(new WindowActionMessage(WindowAction.Close));
                return;
            }

            switch (e.Key) {
                case Key.F5:
                    Reload();
                    break;
                case Key.M:
                    ToggleMute();
                    break;
                case Key.S:
                    Restart();
                    break;
                case Key.Space:
                    TogglePlay();
                    break;
                case Key.R:
                    ToggleRepeat();
                    break;
                case Key.C:
                    ToggleComment();
                    break;
                case Key.F:
                    ToggleFullScreen();
                    break;
            }
        }
    }

    public enum CommentVisibility {
        /// <summary>
        /// 表示
        /// </summary>
        Visible,
        /// <summary>
        /// 非表示
        /// </summary>
        Hidden,
        /// <summary>
        /// 投コメのみ表示
        /// </summary>
        OnlyAuthor
    }
}
