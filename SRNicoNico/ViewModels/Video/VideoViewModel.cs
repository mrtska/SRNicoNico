using System;
using System.Collections.Generic;
using System.Text;
using Livet;
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


        public VideoViewModel(IVideoService videoService, string videoId) {

            VideoService = videoService;
            VideoId = videoId;
        }

        public void Loaded() {


        }


    }
}
