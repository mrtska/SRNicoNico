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
        /// <param name="vm">親ViewModel</param>
        /// <param name="apiData">APIから取得したコメントデータ</param>
        public async void Initialize(VideoViewModel vm, WatchApiDataComment apiData) {

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

                var layers = new List<object>();

                foreach (var layer in apiData.Layers!) {

                    if (layer == null) {
                        continue;
                    }

                    var entries = new List<VideoCommentEntry>();

                    foreach (var threadId in layer.ThreadIds!) {

                        if (threadId == null) {
                            continue;
                        }
                        var thread = CommentThreads.SingleOrDefault(s => s.Fork == threadId.Fork);
                        if (thread != null) {
                            entries.AddRange(thread.Entries!);
                        }
                    }
                    layers.Add(new {
                        index = layer.Index,
                        entries = entries.OrderBy(o => o.Vpos).ThenBy(o => o.Number).Select(s => {

                            var mails = s.Mail.Split(' ');

                            var fontSize = "middle";
                            if (mails.Contains("small")) {
                                fontSize = "small";
                            } else if (mails.Contains("big")) {
                                fontSize = "big";
                            }
                            var position = "naka";
                            if (mails.Contains("ue")) {
                                position = "ue";
                            } else if (mails.Contains("shita")) {
                                position = "shita";
                            }

                            var color = "#FFFFFF";
                            if (mails.Contains("red")) {
                                color = "#FF0000";
                            } else if (mails.Contains("pink")) {
                                color = "#FF8080";
                            } else if (mails.Contains("orange")) {
                                color = "#FFC000";
                            } else if (mails.Contains("yellow")) {
                                color = "#FFFF00";
                            } else if (mails.Contains("green")) {
                                color = "#00FF00";
                            } else if (mails.Contains("cyan")) {
                                color = "#00FFFF";
                            } else if (mails.Contains("blue")) {
                                color = "#0000FF";
                            } else if (mails.Contains("purple")) {
                                color = "#C000FF";
                            } else if (mails.Contains("black")) {
                                color = "#000000";
                            }

                            return new {
                                no = s.Number,
                                vpos = s.Vpos,
                                mail = s.Mail!,
                                content = s.Content!,
                                fontSize,
                                position,
                                color
                            };
                        })
                    });
                }
                // WebViewにコメントデータを飛ばす
                vm.Html5Handler?.DispatchComment(new { layers });
            } catch (StatusErrorException) {

                throw;
            } finally {

                IsActive = false;
            }
        }
    }
}
