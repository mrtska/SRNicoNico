using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using DynaJson;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using SRNicoNico.Views.Controls;

namespace SRNicoNico.ViewModels {
    public class VideoHtml5Handler : IDisposable {

        /// <summary>
        /// WebView
        /// ライフタイムはこのクラスと同じ
        /// </summary>
        public WebView2? WebView { get; private set; }

        private bool Initialized = false;

        private bool BrowserInitialized = false;
        private object? CommentObject;

        public VideoHtml5Handler() {

            WebView = new WebView2 { DefaultBackgroundColor = Color.Black };
        }

        /// <summary>
        /// 指定したViewModelでWebViewを初期化する
        /// </summary>
        /// <param name="vm">JavaScript環境に露出するViewModel</param>
        /// <param name="contentUri">動画URL</param>
        public async Task InitializeAsync(VideoViewModel vm, string contentUri) {

            if (WebView == null || Initialized) {
                return;
            }
            BrowserInitialized = false;

            await WebView.EnsureCoreWebView2Async();
            WebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            WebView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            WebView.CoreWebView2.SetVirtualHostNameToFolderMapping("srniconico", GetHtml5PlayerPath(), CoreWebView2HostResourceAccessKind.Allow);
            WebView.Source = new Uri("http://srniconico/player.html");
            WebView.CoreWebView2.AddHostObjectToScript("vm", vm);
            WebView.KeyDown += (o, e) => {
                vm.KeyDown(e);
            };

            WebView.CoreWebView2.WebMessageReceived += (o, e) => {

                var json = JsonObject.Parse(e.WebMessageAsJson);
                var type = json.type as string;

                switch (type) {
                    case "initialized": // ブラウザ側の初期化が終わったので動画URLをブラウザに送信する
                        WebView?.CoreWebView2.PostWebMessageAsJson(JsonObject.Serialize(new { type = "setContent", value = new { contentUri, volume = vm.IsMuted ? 0 : vm.Volume, autoplay = true } }));
                        SetVisibility(vm.CommentVisibility);
                        BrowserInitialized = true;
                        if (CommentObject != null) {
                            WebView?.CoreWebView2?.PostWebMessageAsJson(JsonObject.Serialize(new { type = "dispatchComment", value = CommentObject }));
                            CommentObject = null;
                        }
                        break;
                    case "clicked":
                        if (vm.Settings.ClickOnPause) {
                            vm.TogglePlay();
                        }
                        break;
                    case "doubleclick":
                        if (vm.Settings.DoubleClickToggleFullScreen) {
                            vm.ToggleFullScreen();
                        }
                        break;
                    case "info":
                        vm.ActualVideoWidth = (int)json.value.width;
                        vm.ActualVideoHeight = (int)json.value.height;
                        vm.ActualVideoDuration = json.value.duration;
                        break;
                    case "loop":
                        // 再生済みの位置やバッファ済みの位置をWebViewから取得する
                        vm.PlayedRange.Clear();
                        foreach (var played in json.value.played) {

                            if (played == null) {
                                continue;
                            }
                            vm.PlayedRange.Add(new TimeRange((float)played.start, (float)played.end));
                        }
                        vm.BufferedRange.Clear();
                        foreach (var buffered in json.value.buffered) {

                            if (buffered == null) {
                                continue;
                            }
                            vm.BufferedRange.Add(new TimeRange((float)buffered.start, (float)buffered.end));
                        }
                        break;
                    case "ended":
                        if (vm.RepeatBehavior != RepeatBehavior.None) {
                            vm.Resume();
                        }
                        break;
                    case "mousemove":
                        vm.IsFullScreenPopupOpen = true;
                        break;
                    case "KeyS":
                        vm.Restart();
                        break;
                    case "KeyR":
                        vm.ToggleRepeat();
                        break;
                    case "KeyC":
                        vm.ToggleComment();
                        break;
                    case "KeyF":
                        vm.ToggleFullScreen();
                        break;
                    case "KeyM":
                        vm.ToggleMute();
                        break;
                    case "Space":
                        vm.TogglePlay();
                        break;
                }
            };

            Initialized = true;
        }

        public void SetContent(string contentUri, bool clearComment) {
            WebView?.CoreWebView2.PostWebMessageAsJson(JsonObject.Serialize(new { type = "setSrc", value = new { contentUri, clearComment } }));
        }

        public void DispatchComment(object obj) {
            if (BrowserInitialized) {
                WebView?.CoreWebView2?.PostWebMessageAsJson(JsonObject.Serialize(new { type = "dispatchComment", value = obj }));
            } else {
                CommentObject = obj;
            }
        }

        public void Seek(double position) {

            WebView?.CoreWebView2?.PostWebMessageAsJson(JsonObject.Serialize(new { type = "seek", value = position }));
        }

        public void SetVolume(float volume) {

            WebView?.CoreWebView2?.PostWebMessageAsJson(JsonObject.Serialize(new { type = "setVolume", value = volume }));
        }

        public void TogglePlay() {

            WebView?.CoreWebView2?.PostWebMessageAsJson(JsonObject.Serialize(new { type = "togglePlay" }));
        }

        public void SetVisibility(CommentVisibility visibility) {

            switch (visibility) {
                case CommentVisibility.Visible:
                    WebView?.CoreWebView2?.PostWebMessageAsJson(JsonObject.Serialize(new { type = "setVisibility", value = "visible" }));
                    break;
                case CommentVisibility.Hidden:
                    WebView?.CoreWebView2?.PostWebMessageAsJson(JsonObject.Serialize(new { type = "setVisibility", value = "hidden" }));
                    break;
                case CommentVisibility.OnlyAuthor:
                    WebView?.CoreWebView2?.PostWebMessageAsJson(JsonObject.Serialize(new { type = "setVisibility", value = "onlyAuthor" }));
                    break;
            }
        }

        public void SetRate(double value) {
            WebView?.CoreWebView2?.PostWebMessageAsJson(JsonObject.Serialize(new { type = "setRate", value }));
        }

        /// <summary>
        /// 動画プレイヤーのパスを返す
        /// </summary>
        /// <returns>ファイルパス</returns>
        private string GetHtml5PlayerPath() {
#if DEBUG
            return Environment.GetEnvironmentVariable("SRNICONICO_HTML_PATH") ?? Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase!, "Html");
#else
            return Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase!, "Html");
#endif
        }

        public void Dispose() {

            WebView?.Dispose();
            WebView = null;
        }

    }
}
