using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Livet;
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

        private float _Volume = 0.3F;
        /// <summary>
        /// 音量
        /// </summary>
        public float Volume {
            get { return _Volume; }
            set { 
                if (_Volume == value)
                    return;
                _Volume = value;
                RaisePropertyChanged();
                Html5Handler?.SetVolume(value);
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

        private DmcSession? DmcSession;
        private Timer? HeartbeatTimer;

        private readonly IVideoService VideoService;
        private readonly string VideoId;


        public VideoViewModel(IVideoService videoService, string videoId) : base(videoId) {

            VideoService = videoService;
            VideoId = videoId;
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

                // コメント部分を初期化する
                Comment = new VideoCommentViewModel(VideoService);
                Comment.Initialize(ApiData.Comment!);


                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"動画 {VideoId} の再生に失敗しました ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
            }
        }

        /// <summary>
        /// 再生と一時停止を切り替える
        /// </summary>
        public void TogglePlay() {

        }

        /// <summary>
        /// 指定した位置にシークする
        /// </summary>
        /// <param name="position">シークしたい位置 秒</param>
        public void Seek(int position) {

            Html5Handler?.Seek(position);
        }

        /// <summary>
        /// 動画を最初から再生する
        /// </summary>
        public void Restart() {

            Seek(0);
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

    }
}
