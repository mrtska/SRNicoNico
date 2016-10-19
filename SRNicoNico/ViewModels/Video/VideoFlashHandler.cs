using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Controls;
using SRNicoNico.Views.Contents.Live;
using AxShockwaveFlashObjects;
using Flash.External;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Views.Contents.Video;
using Codeplex.Data;
using SRNicoNico.Views.Controls;
using System.Windows;


namespace SRNicoNico.ViewModels {
    public class VideoFlashHandler : IObjectForScriptable, IDisposable {




        private VideoViewModel Owner;

        private WebBrowser Browser;

        private VideoData VideoData;


        public VideoFlashHandler(VideoViewModel vm) {

            Owner = vm;
        }

        protected internal async void Initialize(WebBrowser browser, VideoData videoData) {

            Browser = browser;

            //取得に失敗
            if(videoData.ApiData == null) {

                Owner.LoadFailed = true;
                Owner.IsActive = false;
                Owner.Status = "動画の読み込みに失敗しました。";
                return;

            }
            VideoData = videoData;

            //タブの名前を設定
            Owner.Name = videoData.ApiData.Title;

            Owner.Time.Length = videoData.ApiData.Length;

            //有料動画なら
            if(VideoData.ApiData.IsPaidVideo) {

                App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(PaidVideoDialog), Owner, TransitionMode.Modal));
                return;
            }


            await DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {


                browser.LoadCompleted += (sender, e) => {

                    browser.ObjectForScripting = new ObjectForScripting(this);
                    browser.InvokeScript("VideoViewModel$initialize", VideoData.ApiData.GetFlv.VideoUrl);

                    //ここからInvoke可能
                    Owner.IsPlaying = true;
                    Owner.IsInitialized = true;
                    Owner.Mylist.EnableButtons();

                    Owner.Volume = Settings.Instance.Volume;

                    //RTMPの時はサーバートークンも一緒にFlashに渡す
                    if(VideoData.VideoType == NicoNicoVideoType.RTMP) {

                        //InvokeScript("AsOpenVideo", VideoData.ApiData.GetFlv.VideoUrl + "^" + VideoData.ApiData.GetFlv.FmsToken, App.ViewModelRoot.Config.Comment.ToJson());
                    } else {

                        //InvokeScript("AsOpenVideo", VideoData.ApiData.GetFlv.VideoUrl, App.ViewModelRoot.Config.Comment.ToJson());
                    }

                    Owner.IsRepeat = Settings.Instance.IsRepeat;
                    InitializeComment();
                };

                if(VideoData.ApiData.Cmsid.Contains("nm")) {

                    VideoData.VideoType = NicoNicoVideoType.SWF;

                } else if(VideoData.ApiData.GetFlv.VideoUrl.StartsWith("rtmp")) {

                    VideoData.VideoType = NicoNicoVideoType.RTMP;
                } else {

                    if(VideoData.ApiData.MovieType == "flv") {

                        VideoData.VideoType = NicoNicoVideoType.FLV;
                    } else {

                        VideoData.VideoType = NicoNicoVideoType.MP4;
                    }
                }

                if(VideoData.VideoType == NicoNicoVideoType.MP4) {

                    browser.Source = new Uri(GetHtml5PlayerPath());
                } else {

                    browser.Source = new Uri(GetFlashPlayerPath());
                }



                Owner.IsActive = false;


            }));


        }

        private async void InitializeComment() {

            Owner.Comment.IsCommentLoading = true;

            var list = await Owner.CommentInstance.GetCommentAsync();
            if(list != null) {

                foreach(var entry in list) {

                    VideoData.CommentData.Add(new CommentEntryViewModel(entry, Owner));
                }

                dynamic json = new DynamicJson();
                json.array = list;

                InjectComment(json.ToString());
                Owner.Comment.CanComment = true;
                Owner.Comment.IsCommentLoading = false;

                //投稿者コメントがあったら取得する
                if(VideoData.ApiData.HasOwnerThread) {

                    var ulist = await Owner.CommentInstance.GetUploaderCommentAsync();
                    dynamic ujson = new DynamicJson();
                    json.array = ulist;

                    InjectUploaderComment(json.ToString());
                }

            }

            if(!Settings.Instance.CommentVisibility) {

                //InvokeScript("AsToggleComment");
            } else {

                Owner.CommentVisibility = true;
            }
        }



        protected internal void Restart() {


        }

        protected internal void ReloadComment() {


        }

        protected internal void ShowComment() {

            InvokeScript("CommentViewModel$show_comment");

        }
        protected internal void HideComment() {

            InvokeScript("CommentViewModel$hide_comment");
        }



        protected internal void InjectComment(string json) {


            InvokeScript("CommentViewModel$initialize", json);
        }

        protected internal void InjectUploaderComment(string json) {


        }

        protected internal void InjectPostedComment(string json) {


        }

        protected internal void Pause() {

            InvokeScript("VideoViewModel$pause");
        }

        protected internal void Resume() {

            InvokeScript("VideoViewModel$resume");
        }

        protected internal void TogglePlay() {

            if(Owner.IsPlaying) {

                Pause();
            } else {

                Resume();
            }
        }

        protected internal void SetVolume(int volume) {

            InvokeScript("VideoViewModel$setvolume", (volume / 100.0).ToString());
        }

        protected internal void Seek(int pos) {

            InvokeScript("VideoViewModel$seek", pos.ToString());
        }

        private double GetFixedPos(double pos, long videoTime) {

            double ret = pos / videoTime;

            return (Browser.ActualWidth * ret);
        }


        //1フレーム毎に呼ばれる
        private void CsFrame(double time, TimeRange[] played, TimeRange[] buffer, int vpos) {


            var videoTime = VideoData.ApiData.Length;

            Owner.Time.PlayedRange.Clear();
            foreach(var range in played) {

                range.Width = GetFixedPos(range.End - range.Start, VideoData.ApiData.Length);
                range.Position = new Thickness(GetFixedPos(range.Start, VideoData.ApiData.Length), 0, 0, 0);

                Owner.Time.PlayedRange.Add(range);
            }

            Owner.Time.BufferedRange.Clear();
            foreach(var range in buffer) {

                range.Width = GetFixedPos(range.End - range.Start, VideoData.ApiData.Length);
                range.Position = new Thickness(GetFixedPos(range.Start, VideoData.ApiData.Length), 0, 0, 0);

                Owner.Time.BufferedRange.Add(range);
            }


            Owner.Comment.Vpos = vpos.ToString();

            //Console.WriteLine(VideoData.ApiData.Cmsid + " time:" + time + " buffer:" + buffer + " vpos:" + vpos);

            Owner.Time.CurrentTime = time;
            //Time.CurrentTimeString = NicoNicoUtil.ConvertTime((long)Time.CurrentTime);
        }


        //ASを呼ぶ
        public void InvokeScript(string func, params string[] args) {

            //読み込み前にボタンを押しても大丈夫なように メモリ解放されたあとに呼ばれないように
            if(Browser != null) {

                try {

                    if(args.Length == 0) {

                        Browser.Dispatcher.BeginInvoke((Action)(() => Browser.InvokeScript(func)));
                    } else {

                        Browser.Dispatcher.BeginInvoke((Action)(() => Browser.InvokeScript(func, args)));
                    }

                } catch(COMException) {

                    Console.WriteLine("COMException：" + func);
                }
            }
        }

        public void Invoked(string cmd, string args) {

            switch(cmd) {
                case "currenttime": {

                        dynamic json = DynamicJson.Parse(args);

                        var played = new List<TimeRange>();

                        foreach(var play in json.played) {

                            played.Add(new TimeRange() { Start = play.start, End = play.end });
                        }

                        var buffered = new List<TimeRange>();

                        foreach(var buffer in json.buffered) {

                            buffered.Add(new TimeRange() { Start = buffer.start, End = buffer.end });
                        }

                        CsFrame((double)json.time, played.ToArray(), buffered.ToArray(), (int)json.vpos);
                        break;
                    }
                case "playstate":

                    Owner.IsPlaying = bool.Parse(args);
                    break;

                case "widtheight":
                    Owner.VideoData.Resolution = args;
                    break;
                case "log":
                    Console.WriteLine("Log: " + args);
                    break;
                default: {


                        Console.WriteLine("Invoked:" + cmd);
                        break;
                    }
            }
        }

        public void Dispose() {

            Browser?.Dispose();


        }

        private string GetHtml5PlayerPath() {

            var cur = NicoNicoUtil.CurrentDirectory.Replace('\\', '/').Split(':');

            return "file://127.0.0.1/" + cur[0] + "$" + cur[1] + "Html/videohtml5.html";
        }

        private string GetFlashPlayerPath() {

            return "file://127.0.0.1/e$/Development/MVVM/WPF/SRNicoNico/SRNicoNico/Html5/videoflash.html";
        }



    }
}
