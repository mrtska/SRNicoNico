using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

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
