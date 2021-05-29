using System;
using System.Collections.Generic;
using System.Linq;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// 動画再生用のViewModelのコメント部分
    /// </summary>
    public class VideoCommentViewModel : TabItemViewModel {

        private ObservableSynchronizedCollection<VideoCommentThread> _CommentThreads = new ObservableSynchronizedCollection<VideoCommentThread>();
        /// <summary>
        /// コメントのスレッド
        /// </summary>
        public ObservableSynchronizedCollection<VideoCommentThread> CommentThreads {
            get { return _CommentThreads; }
            set { 
                if (_CommentThreads == value)
                    return;
                _CommentThreads = value;
                RaisePropertyChanged();
            }
        }

        private VideoCommentThread? _SelectedThread;
        /// <summary>
        /// 現在選択中のコメントスレッド
        /// デフォルトはDefaultPostTargetがTrueのもの
        /// </summary>
        public VideoCommentThread? SelectedThread {
            get { return _SelectedThread; }
            set { 
                if (_SelectedThread == value)
                    return;
                _SelectedThread = value;
                RaisePropertyChanged();
            }
        }

        private readonly IVideoService VideoService;

        public VideoCommentViewModel(IVideoService videoService) {

            VideoService = videoService;
        }


        /// <summary>
        /// コメントを取得する
        /// </summary>
        /// <param name="apiData">APIから取得したコメントデータ</param>
        public async void Initialize(WatchApiDataComment apiData) {

            IsActive = true;
            CommentThreads.Clear();

            try {
                foreach (var thread in await VideoService.GetCommentAsync(apiData)) {

                    thread.Entries = thread.Entries.Where(w => !w.Deleted).OrderBy(o => o.Vpos).ThenBy(o => o.Number).ToList();
                    CommentThreads.Add(thread);
                }

                var defaultThread = apiData.Threads.SingleOrDefault(s => s.IsDefaultPostTarget);
                if (defaultThread != null) {

                    SelectedThread = CommentThreads.SingleOrDefault(s => s.Fork == defaultThread.Fork);
                }

            } catch (StatusErrorException e) {

                Status = $"コメントの取得に失敗しました ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
            }
        }
    }
}
