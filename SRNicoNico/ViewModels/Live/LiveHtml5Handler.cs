using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SRNicoNico.ViewModels {
    public class LiveHtml5Handler : NotificationObject, IObjectForScriptable, IDisposable {

        [DllImport("urlmon.dll")]
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Error)]
        static extern int CoInternetSetFeatureEnabled(int featureEntry, [MarshalAs(UnmanagedType.U4)] int dwFlags, bool fEnable);

        public const int FEATURE_LOCALMACHINE_LOCKDOWN = 8;
        public const int SET_FEATURE_ON_PROCESS = 0x00000002;

        #region WebBrowser変更通知プロパティ
        private WebBrowser _WebBrowser;

        public WebBrowser WebBrowser {
            get { return _WebBrowser; }
            set {
                if (_WebBrowser == value)
                    return;
                _WebBrowser = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region ContentControl変更通知プロパティ
        private ContentControl _ContentControl = new ContentControl();

        public ContentControl ContentControl {
            get { return _ContentControl; }
            set {
                if (_ContentControl == value)
                    return;
                _ContentControl = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region FullScreenContentControl変更通知プロパティ
        private ContentControl _FullScreenContentControl = new ContentControl();

        public ContentControl FullScreenContentControl {
            get { return _FullScreenContentControl; }
            set {
                if (_FullScreenContentControl == value)
                    return;
                _FullScreenContentControl = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region IsPlaying変更通知プロパティ
        private bool _IsPlaying;
        public bool IsPlaying {
            get { return _IsPlaying; }
            set {
                if (_IsPlaying == value)
                    return;
                _IsPlaying = value;
                RaisePropertyChanged();
                if (value) {

                    Resume();
                } else {

                    Pause();
                }
            }
        }
        #endregion

        public void Resume() {

            InvokeScript("LIve$Resume");
        }
        public void Pause() {

            InvokeScript("Live$Pause");
        }

        #region VolumeIcon変更通知プロパティ
        private string _VolumeIcon;

        public string VolumeIcon {
            get { return _VolumeIcon; }
            set {
                if (_VolumeIcon == value)
                    return;
                _VolumeIcon = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region IsMute変更通知プロパティ
        public bool IsMute {
            get { return Settings.Instance.IsMute; }
            set {
                Settings.Instance.IsMute = value;
                ApplyVolume();
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Volume変更通知プロパティ
        private int _Volume = 0;

        public int Volume {
            get { return _Volume; }
            set {
                if (value > 100) {

                    value = 100;
                } else if (value < 0) {

                    value = 0;
                }
                _Volume = value;
                Settings.Instance.Volume = value;
                RaisePropertyChanged();
                ApplyVolume();
            }
        }
        #endregion

        #region IsFullScreen変更通知プロパティ
        private bool _IsFullScreen = false;

        public bool IsFullScreen {
            get { return _IsFullScreen; }
            set {
                if (_IsFullScreen == value)
                    return;
                _IsFullScreen = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region CurrentTime変更通知プロパティ
        private double _CurrentTime;

        public double CurrentTime {
            get { return _CurrentTime; }
            set {
                if (_CurrentTime == value)
                    return;
                _CurrentTime = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private LiveViewModel Owner;
        private NicoNicoLiveApi ApiData;

        public LiveHtml5Handler() {

        }

        public void Initialize(LiveViewModel vm, NicoNicoLiveApi api) {

            Owner = vm ?? throw new ArgumentNullException(nameof(vm));
            ApiData = api ?? throw new ArgumentNullException(nameof(api));

            WebBrowser = new WebBrowser();
            CoInternetSetFeatureEnabled(FEATURE_LOCALMACHINE_LOCKDOWN, SET_FEATURE_ON_PROCESS, false);

            if (IsFullScreen) {

                FullScreenContentControl.Content = WebBrowser;
            } else {

                ContentControl.Content = WebBrowser;
            }

            WebBrowser.ObjectForScripting = new ObjectForScripting(this);
            WebBrowser.Navigate(new Uri(GetHtml5PlayerPath()));
        }

        public void InvokeScript(string func, params object[] args) {

            //読み込み前にボタンを押しても大丈夫なように メモリ解放されたあとに呼ばれないように
            if (WebBrowser != null && WebBrowser.IsLoaded) {
                try {

                    if (args.Length == 0) {

                        WebBrowser.Dispatcher.BeginInvoke((Action)(() => WebBrowser.InvokeScript(func)));
                    } else {

                        WebBrowser.Dispatcher.BeginInvoke((Action)(() => WebBrowser.InvokeScript(func, args)));
                    }
                } catch (Exception e) when (e is COMException || e is ObjectDisposedException) {

                    Console.WriteLine("COMException：" + func);
                    return;
                }
            }
        }

        public void Invoked(string cmd, string args) {

            Console.WriteLine("Invoked " + cmd);

            switch (cmd) {
                case "ready": // ブラウザ側の準備が出来た

                    while (ApiData.HlsUrl == null) {

                        Thread.Sleep(100);
                    }

                    InvokeScript("Live$Initialize", ApiData.HlsUrl, 0, true);
                    Volume = Settings.Instance.Volume;
                    // 再生速度をUIと同期
                    //InvokeScript("Video$SetRate", PlayRate);
                    //Owner.PostInitialize();
                    break;
                case "widtheight":
                    break;
                case "playstate":
                    IsPlaying = bool.Parse(args);
                    break;
                case "click":
                    if (Settings.Instance.ClickOnPause) {

                        IsPlaying ^= true;
                    }
                    break;
                case "mousewheel":
                    if (!string.IsNullOrEmpty(args)) {

                        var vol = int.Parse(args);

                        if (vol >= 0) {

                            Volume += 2;
                        } else {

                            Volume -= 2;
                        }
                    }
                    break;
                //case "alldataloaded":
                //    if (ApiData.DmcHeartbeatRequired) {
                //        DmcHeartBeatTimer.Enabled = false;
                //    }
                //    break;
                //case "ended": {

                //        if (IsRepeat) {

                //            Seek(0);
                //            Resume();
                //        } else {

                //            if (IsFullScreen && !Owner.IsPlayList) {

                //                ReturnToNormalScreen();
                //            }
                //        }
                //        if (ApiData != null) {

                //            //現在再生位置を初期化する
                //            //WatchiApiInstance.RecordPlaybackPositionAsync(ApiData, 0);
                //        }
                //        Owner.VideoEnd();
                //        break;
                //    }
                //case "showcontroller": {

                //        ShowFullScreenController = true;
                //        ShowFullScreenCommentBar = true;
                //        break;
                //    }
                //case "hidecontroller": {

                //        if (!Settings.Instance.AlwaysShowSeekBar) {

                //            ShowFullScreenController = false;
                //        }
                //        if (!Owner.Comment.Post.IsFocused) {
                //            ShowFullScreenCommentBar = false;
                //        }
                //        break;
                //    }
                default:
                    Console.WriteLine("cmd: " + cmd + " args: " + args);
                    break;
            }
        }

        private void SetVolumeIcon() {

            if (IsMute) {

                VolumeIcon = "Mute";
                return;
            }
            if (Volume == 0) {

                VolumeIcon = "s0";
            } else if (Volume < 30) {

                VolumeIcon = "s30";
            } else if (Volume < 80) {

                VolumeIcon = "s80";
            } else if (Volume <= 100) {

                VolumeIcon = "s100";
            }
        }

        private void ApplyVolume() {

            SetVolumeIcon();
            if (IsMute) {

                InvokeScript("Live$SetVolume", "0");
            } else {

                InvokeScript("Live$SetVolume", (Volume / 100.0).ToString());
            }
        }



        private string GetHtml5PlayerPath() {

            var cur = NicoNicoUtil.CurrentDirectory;
            return Path.Combine(cur, "Html/livehtml5.html");
        }

        public void Dispose() {

            WebBrowser?.Dispose();
            WebBrowser = null;
        }
    }
}
