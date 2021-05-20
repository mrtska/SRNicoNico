using System;
using System.Collections.Generic;
using System.Text;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// 動画再生用のViewModel
    /// </summary>
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


        public void Reload() {


        }

    }
}
