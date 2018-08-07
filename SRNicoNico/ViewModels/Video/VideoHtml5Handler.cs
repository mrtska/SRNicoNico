﻿using Codeplex.Data;
using Livet.Messaging;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace SRNicoNico.ViewModels {
    public class VideoHtml5Handler : IObjectForScriptable, IDisposable {

        [DllImport("urlmon.dll")]
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Error)]
        static extern int CoInternetSetFeatureEnabled(int featureEntry, [MarshalAs(UnmanagedType.U4)] int dwFlags, bool fEnable);

        public const int FEATURE_LOCALMACHINE_LOCKDOWN = 8;
        public const int SET_FEATURE_ON_PROCESS = 0x00000002;

        private WebBrowser Browser;

        private VideoViewModel Owner;

        //新仕様動画で使う
        private Timer DmcHeartBeatTimer = new Timer(8000.0D);

        private NicoNicoVideoType VideoType;

        public VideoHtml5Handler(WebBrowser browser) {

            Browser = browser;
            
            //忌々しいアクティブコンテンツ云々を無効にする WebBrowserの初期化時にこの値がリセットされるので
            //新しくWebBrowserのインスタンスを生成する度に呼ばないとだめっぽいね
            CoInternetSetFeatureEnabled(FEATURE_LOCALMACHINE_LOCKDOWN, SET_FEATURE_ON_PROCESS, false);
            
            Browser.ObjectForScripting = new ObjectForScripting(this);
            //
            Browser.LoadCompleted += async (obj, e) => {

                var autoPlay = Settings.Instance.AutoPlay;

                //プレイリストなら強制的にtrue
                if(Owner.IsPlayList()) {

                    autoPlay = true;
                }

                if (string.IsNullOrEmpty(Owner.ApiData.Video.SmileInfo.Url) || Owner.ApiData.Context.IsNeedPayment) {

                    App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.PaidVideoView), new PaidVideoViewModel(Owner), TransitionMode.NewOrActive));
                    return;
                }

                if (Owner.ApiData.Video.DmcInfo != null && !Owner.ApiData.Video.Title.Contains("ドットバイドット")) {

                    var dmc = new NicoNicoDmcSession(Owner.ApiData.Video.DmcInfo);
                    var session = await dmc.CreateAsync();


                    if(DmcHeartBeatTimer != null) {

                        DmcHeartBeatTimer.Elapsed += async (state, ev) => {

                            await dmc.HeartbeatAsync(session.Id);
                        };

                        DmcHeartBeatTimer.Enabled = true;
                    }

                    Browser?.InvokeScript("VideoViewModel$initialize", new object[] { session.ContentUri, Owner.ApiData.Context.InitialPlaybackPosition ?? 0, autoPlay });
                } else {

                    if(VideoType == NicoNicoVideoType.RTMP) {

                        Browser.InvokeScript("VideoViewModel$initialize", new object[] { Owner.ApiData.Video.SmileInfo.Url + "^" + Owner.ApiData.FmsToken, Owner.ApiData.Context.InitialPlaybackPosition ?? 0, autoPlay });
                    } else {

                        Browser.InvokeScript("VideoViewModel$initialize", new object[] { Owner.ApiData.Video.SmileInfo.Url, Owner.ApiData.Context.InitialPlaybackPosition ?? 0, autoPlay });
                    }

                }


                if(VideoType == NicoNicoVideoType.SWF || VideoType == NicoNicoVideoType.RTMP) {

                    Owner.ChangePlayRateAvalilable = PlayBackRateAvalilableReason.NotSupportVideo;
                } else {

                    Owner.ChangePlayRateAvalilable = PlayBackRateAvalilableReason.Available;
                }

                Owner.Volume = Owner.Volume;

            };

            //ブラウザにキーボードイベントが走らないようにする
            //走られると+とか押された時に動画の再生速度とかが変わっちゃうんだよね
            //それはちょっと困る
            Browser.PreviewKeyDown += (obj, e) => {

                e.Handled = true;
            };

        }


        //ASを呼ぶ
        public void InvokeScript(string func, params string[] args) {

            //読み込み前にボタンを押しても大丈夫なように メモリ解放されたあとに呼ばれないように
            if(Browser != null && Browser.IsLoaded) {
                try {

                    if(args.Length == 0) {

                        Browser.Dispatcher.BeginInvoke((Action)(() => Browser.InvokeScript(func)));
                    } else {

                        Browser.Dispatcher.BeginInvoke((Action)(() => Browser.InvokeScript(func, args)));
                    }
                } catch(Exception e) when(e is COMException || e is ObjectDisposedException) {

                    Console.WriteLine("COMException：" + func);
                    return;
                }
            }
        }
        public void Initialize(VideoViewModel vm) {

            Owner = vm;

            if(Owner.ApiData.Video.Id.Contains("nm")) {

                VideoType = NicoNicoVideoType.SWF;

                Browser.Source = new Uri(GetSWFPlayerPath());
            } else if(Owner.ApiData.Video.SmileInfo.Url.Contains("rtmp")) {

                VideoType = NicoNicoVideoType.RTMP;

                Browser.Source = new Uri(GetRTMPPlayerPath());
            } else {

                if(Owner.ApiData.Video.MovieType == "flv") {

                    VideoType = NicoNicoVideoType.FLV;
                } else {
                    
                    if(Owner.ApiData.Video.DmcInfo != null) {

                        VideoType = NicoNicoVideoType.New;
                    } else {

                        VideoType = NicoNicoVideoType.MP4;
                    }
                }
                Browser.Source = new Uri(GetHtml5PlayerPath());
            }

        }


        protected internal void Play() {

            
            InvokeScript("VideoViewModel$play");
        }

        protected internal void Pause() {


            InvokeScript("VideoViewModel$pause");
        }

        protected internal void Seek(double pos) {

            if(Owner.ApiData == null) {

                return;
            }

            if(pos < 0) {

                pos = 0;
            } else if(pos > Owner.ApiData.Video.Duration) {

                pos = Owner.ApiData.Video.Duration;
            }

            InvokeScript("VideoViewModel$seek", pos.ToString());
        }



        //JSから呼ばれる
        public void Invoked(string cmd, string args) {

            switch(cmd) {
                case "widtheight":
                    Owner.Resolution = args;
                    break;
                case "bufferingstart":
                    Owner.IsBuffering = true;
                    break;
                case "bufferingend":
                    Owner.IsBuffering = false;
                    break;
                case "playstate":
                    if(!string.IsNullOrEmpty(args)) {

                        Owner.IsPlaying = bool.Parse(args);
                    }
                    break;
                case "click":

                    if(Settings.Instance.ClickOnPause) {

                        Owner.TogglePlay();
                    }
                    break;
                case "mousewheel":
                    if(!string.IsNullOrEmpty(args)) {

                        var vol = int.Parse(args);

                        if(vol >= 0) {

                            Owner.Volume += 2;
                        } else {

                            Owner.Volume -= 2;
                        }
                    }
                    break;
                case "currenttime": {

                        dynamic json = DynamicJson.Parse(args);

                        if(json.played()) {

                            Owner.PlayedRange.Clear();

                            foreach(var play in json.played) {

                                Owner.PlayedRange.Add(new TimeRange() { StartTime = play.start, EndTime = play.end });
                            }
                        }

                        if(json.buffered()) {

                            Owner.BufferedRange.Clear();

                            foreach(var buffer in json.buffered) {

                                Owner.BufferedRange.Add(new TimeRange() { StartTime = buffer.start, EndTime = buffer.end });
                            }
                        }

                        if(json.time()) {

                            Owner.CurrentTime = json.time;
                        }

                        if(json.vpos()) {

                            if(Owner.Comment.CommentVisibility) {

                                Owner.Comment.CommentTick((int)json.vpos);
                            }
                        }
                        
                        break;
                    }
                case "ended": {

                        if(Owner.IsRepeat) {
                            
                            Owner.Restart();
                            Play();
                        } else {

                            if(Owner.IsFullScreen && !Owner.IsPlayList()) {

                                Window.GetWindow(Owner.FullScreenWebBrowser)?.Close();
                            }
                        }

                        if(Owner.ApiData != null) {

                            //現在再生位置を初期化する
                            Owner.WatchiApiInstance.RecordPlaybackPositionAsync(Owner.ApiData, 0);
                        }

                        Owner.VideoEnd();
                        break;
                    }
                case "showcontroller": {

                        Owner.ShowFullScreenController = true;
                        break;
                    }
                case "hidecontroller": {

                        if(!Settings.Instance.AlwaysShowSeekBar) {

                            Owner.ShowFullScreenController = false;
                        }
                        break;
                    }
                case "alldataloaded":
                    DmcHeartBeatTimer.Enabled = false;
                    break;
                default:
                    Console.WriteLine("Invoked: " + cmd);
                    break;
            }
        }
        
        private string GetHtml5PlayerPath() {

            var cur = NicoNicoUtil.CurrentDirectory;
            return cur + "Html/videohtml5.html";
        }

        private string GetFlashPlayerPath() {

            var cur = NicoNicoUtil.CurrentDirectory;
            return cur + "Html/videoflash.html";
        }
        private string GetSWFPlayerPath() {

            var cur = NicoNicoUtil.CurrentDirectory;
            return cur + "Html/videoswf.html";
        }

        private string GetRTMPPlayerPath() {

            var cur = NicoNicoUtil.CurrentDirectory;
            return cur + "Html/videortmp.html";
        }

        public void Dispose() {

            ((IDisposable)DmcHeartBeatTimer).Dispose();
            DmcHeartBeatTimer = null;
            Browser.Dispose();
            Browser = null;
        }
    }
}
