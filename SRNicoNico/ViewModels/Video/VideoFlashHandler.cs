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
using System.Web;

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
using CefSharp;

namespace SRNicoNico.ViewModels {
    public class VideoFlashHandler : ViewModel {

        public IWebBrowser Browser;

        private VideoViewModel Owner;

        public bool IsInitialized;

        public VideoFlashHandler(VideoViewModel vm)  {

            Owner = vm;
        }


        private bool IsPreIntialized = false;

        private VideoData VideoData;

        public void PreInitialize(IWebBrowser flash) {


            Browser = flash;
            Browser.RequestHandler = new RequestHandler();
            Browser.MenuHandler = new MenuHandler();
            IsPreIntialized = true;
        }

        public void Initialize(VideoData videoData) {

            VideoData = videoData;

            while(!IsPreIntialized) {

                Thread.Sleep(1);
            }

            //ロードに失敗したら
            if(VideoData.ApiData == null) {

                Owner.LoadFailed = true;
                Owner.IsActive = false;
                Owner.Status = "動画の読み込みに失敗しました。";
                return;
            }

            Owner.Name = VideoData.ApiData.Title;

            //有料動画なら
            if(VideoData.ApiData.IsPaidVideo) {

                App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(PaidVideoDialog), this, TransitionMode.Modal));
                return;
            }

            Owner.Time = new VideoTime();
            //動画時間
            Owner.Time.VideoTimeString = NicoNicoUtil.ConvertTime(VideoData.ApiData.Length);

            
            if(VideoData.ApiData.Cmsid.Contains("nm")) {

                VideoData.VideoType = NicoNicoVideoType.SWF;
                Browser.Load("http://localbridge/NicoNicoNMPlayer.html");

            } else if(VideoData.ApiData.GetFlv.VideoUrl.StartsWith("rtmp")) {

                VideoData.VideoType = NicoNicoVideoType.RTMP;
                Browser.Load("http://localbridge/NicoNicoRTMPPlayer.html");

            } else {

                if(VideoData.ApiData.MovieType == "flv") {

                    VideoData.VideoType = NicoNicoVideoType.FLV;
                } else {

                    VideoData.VideoType = NicoNicoVideoType.MP4;
                }
                Browser.Load("http://localbridge/NicoNicoPlayer.html");

            }
            Owner.IsActive = false;

        //    Browser.ShowDevTools();

            if(VideoData.ApiData.GetFlv.IsPremium && !VideoData.ApiData.GetFlv.VideoUrl.StartsWith("rtmp") && videoData.ApiData.MovieType != "swf") {

                Owner.StoryBoardStatus = "取得中";

                var sb = new NicoNicoStoryBoard(VideoData.ApiData.GetFlv.VideoUrl);
                VideoData.StoryBoardData = sb.GetStoryBoardData();

                if(VideoData.StoryBoardData == null) {

                    Owner.StoryBoardStatus = "データ無し";
                } else {

                    Owner.StoryBoardStatus = "取得完了";
                }
            } else {

                Owner.StoryBoardStatus = "データ無し";
            }

            Owner.CommentInstance = new NicoNicoComment(VideoData.ApiData, Owner);
            var list = Owner.CommentInstance.GetComment();
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

                    var ulist = Owner.CommentInstance.GetUploaderComment();
                    dynamic ujson = new DynamicJson();
                    json.array = ulist;

                    InjectUploaderComment(json.ToString());
                }

            }

            if(!Settings.Instance.CommentVisibility) {

                InvokeScript("AsToggleComment");
            } else {

                Owner.CommentVisibility = true;
            }


        }

        private void OpenVideo() {

            //ここからInvoke可能
            Owner.IsPlaying = true;
            Owner.IsInitialized = true;
            Owner.Mylist.EnableButtons();

            //RTMPの時はサーバートークンも一緒にFlashに渡す
            if(VideoData.VideoType == NicoNicoVideoType.RTMP) {

                InvokeScript("AsOpenVideo", VideoData.ApiData.GetFlv.VideoUrl + "^" + VideoData.ApiData.GetFlv.FmsToken, App.ViewModelRoot.Config.Comment.ToJson());
            } else {

                InvokeScript("AsOpenVideo", VideoData.ApiData.GetFlv.VideoUrl, App.ViewModelRoot.Config.Comment.ToJson());
            }

            Owner.IsRepeat = Settings.Instance.IsRepeat;
        }

        public void DisposeHandler() {

            Browser.Dispose();
        }


        //ASを呼ぶ
        public void InvokeScript(string func, params string[] args) {

           // Console.WriteLine(func);
            //読み込み前にボタンを押しても大丈夫なように メモリ解放されたあとに呼ばれないように
             if(Browser != null && Browser.GetBrowser() != null) {

                try {

                    if(args.Length == 0) {

                        Browser.ExecuteScriptAsync("window.AsExecuteInstruction", func);
                    } else {

                        switch(args.Length) {
                            case 1:
                                Browser.ExecuteScriptAsync("window.AsExecuteInstruction1", func, args[0]);
                                break;
                            case 2:
                                Browser.ExecuteScriptAsync("window.AsExecuteInstruction2", func, args[0], args[1]);
                                break;
                            case 3:
                                Browser.ExecuteScriptAsync("window.AsExecuteInstruction3", func, args[0], args[1], args[2]);
                                break;

                        }
                    }
                } catch(COMException) {

                    Console.WriteLine("COMException：" + func);
                }
            }
        }
        

        //ExternalIntarface.callでActionscriptから呼ばれる
        public void InvokeFromActionScript(string func, params object[] rawargs) {

            var args = rawargs.Cast<string>().ToArray();
            switch(func) {
                case "Ready":
                    OpenVideo();
                    break;

                case "CsFrame": //毎フレーム呼ばれる
                    CsFrame(float.Parse(args[0]), float.Parse(args[1]), long.Parse(args[2]), args[3]);
                    break;
                case "NetConnection.Connect.Closed":    //RTMP動画再生時にタイムアウトになったら
                    Owner.RTMPTimeOut();
                    break;
                case "ShowController":  //マウスを動かしたら呼ばれる
                    Owner.ShowFullScreenPopup();
                    break;
                case "HideController":  //マウスを数秒動画の上で静止させたら呼ばれる
                    Owner.HideFullScreenPopup();
                    break;
                case "Initialized": //動画が再生される直前に呼ばれる
                    Owner.Volume = Settings.Instance.Volume;    //保存された値をFlash側に伝える
                    break;
                case "WidthHeight":
                    Owner.VideoData.Resolution = args[0];
                    break;
                case "Bitrate":
                    //Owner.VideoData.BitRate = args[0];
                    break;
                case "Framerate":
                    var rate = double.Parse(args[0]);
                    rate *= 100;
                    rate = Math.Floor(rate) / 100;
                    Owner.VideoData.FrameRate = rate.ToString();
                    break;
                case "FileSize":
                    if(VideoData.VideoType == NicoNicoVideoType.RTMP) {

                        Owner.VideoData.FileSize = "N/A";
                    } else {

                        var size = double.Parse(args[0]);
                        size /= 100000.0;
                        size = Math.Floor(size) / 10;

                        Owner.VideoData.FileSize = size.ToString() + "MB";
                    }

                    break;
                case "Stop": //動画が最後まで行ったらリピートしたりフルスクリーンから復帰したりする
                    if(Owner.IsRepeat) {

                        Restart();
                    } else if(Owner.IsFullScreen) {

                        if(Owner.IsPlayList) {

                            Owner.PlayList.Next();
                        } else {

                            Owner.ReturnFromFullScreen();
                        }
                    } else {

                        if(Owner.IsPlayList) {

                            Owner.PlayList.Next();  
                        }
                    }

                    break;
                case "Click":   //Flash部分がクリックされた時に呼ばれる
                    if(Settings.Instance.ClickOnPause) {   //クリックしたら一時停止する設定になっていたら

                        TogglePlay();
                    }
                    break;
                case "PlayVideo":
                    Console.WriteLine("From AS:" + func + " " +args[0]);
                    break;
                default:
                    Console.WriteLine("From AS:" + func);
                    break;
            }
        }

        private int prevTime;
        private double sumBPS;
        //1フレーム毎に呼ばれる
        public void CsFrame(float time, float buffer, long bps, string vpos) {

            if(prevTime != (int)time) {

                double comp = sumBPS / 1024;

                //大きいから単位を変える
                if(comp > 1000) {

                    Owner.BPS = Math.Floor((comp / 1024) * 100) / 100 + "MiB/秒";
                } else {

                    Owner.BPS = Math.Floor(comp * 100) / 100 + "KiB/秒";
                }
                sumBPS = 0;
            } else {

                sumBPS += bps;
            }
            prevTime = (int)time;

            Owner.Time.BufferedTime = buffer;
            Owner.Comment.Vpos = vpos;

            //Console.WriteLine(VideoData.ApiData.Cmsid + " time:" + time + " buffer:" + buffer + " vpos:" + vpos);

            Owner.SetSeekCursor(time);
        }


        //最初から
        public void Restart() {

            Seek(0);
        }
        public void Pause() {
            
            Owner.IsPlaying = false;
            InvokeScript("AsPause");
        }

        public void Resume() {

            Owner.IsPlaying = true;
            InvokeScript("AsResume");
        }

        //一時停止切り替え
        public void TogglePlay() {

            if(Owner.IsPlaying) {

                Pause();
            } else {

                Resume();
            }
        }

        //コメント設定をFlashに反映させる
        public void ApplyChanges() {

            InvokeScript("AsApplyChanges", App.ViewModelRoot.Config.Comment.ToJson());
        }


        //Flashにシーク命令を送る
        public void Seek(float pos) {

            InvokeScript("AsSeek", pos.ToString());
        }

        //Flashにコメントリストを送る
        public void InjectComment(string json) {

            InvokeScript("AsInjectComment", HttpUtility.UrlEncode(json));
        }
        //Flashに投稿者コメントリストを送る
        public void InjectUploaderComment(string json) {

            InvokeScript("AsInjectUploaderComment", json);
        }
    }
}
