using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Web;
using System;
using System.Linq;
using Livet;

using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using Livet.Messaging;
using SRNicoNico.Views.Contents.Video;

using Codeplex.Data;
using Flash.External;
using AxShockwaveFlashObjects;
using Livet.Messaging.Windows;

namespace SRNicoNico.ViewModels {

	public class VideoViewModel : TabItemViewModel {

        //APIバックエンドインスタンス
        public NicoNicoWatchApi WatchApi;

        //コメントバックエンドインスタンス
        public NicoNicoComment CommentInstance;


        #region CommentStatus変更通知プロパティ
        private string _CommentStatus;

        public string CommentStatus {
            get { return _CommentStatus; }
            set { 
                if(_CommentStatus == value)
                    return;
                _CommentStatus = value;
                UpdateStatus();
            }
        }
        #endregion


        #region StoryBoardStatus変更通知プロパティ
        private string _StoryBoardStatus;

        public string StoryBoardStatus {
            get { return _StoryBoardStatus; }
            set { 
                if(_StoryBoardStatus == value)
                    return;
                _StoryBoardStatus = value;
                UpdateStatus();
            }
        }
        #endregion

        private void UpdateStatus() {
            
            Status = "コメント:" + CommentStatus + " ストーリーボード:" + StoryBoardStatus;
        }

        //API結果
        #region VideoData変更通知プロパティ
        private VideoData _VideoData;

		public VideoData VideoData {
			get { return _VideoData; }
			set { 
				if(_VideoData == value)
					return;
				_VideoData = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		//各種シークバー関連の時間管理
		#region Time変更通知プロパティ
		private VideoTime _Time;

		public VideoTime Time {
			get { return _Time; }
			set { 
				if(_Time == value)
					return;
				_Time = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region BPS変更通知プロパティ
		private string _BPS;

		public string BPS {
			get { return _BPS; }
			set {
				if(_BPS == value)
					return;
				_BPS = value;
				RaisePropertyChanged();
			}
		}
        #endregion

        #region IsInitialized変更通知プロパティ
        private bool _IsInitialized;

        public bool IsInitialized {
            get { return _IsInitialized; }
            set { 
                if(_IsInitialized == value)
                    return;
                _IsInitialized = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region IsPlaying変更通知プロパティ
        private bool _IsPlaying;

        public bool IsPlaying {
            get { return _IsPlaying; }
            set { 
                if(_IsPlaying == value)
                    return;
                _IsPlaying = value;
                RaisePropertyChanged();
            }
        }
        #endregion

       
        #region CommentVisibility変更通知プロパティ
        private bool _CommentVisibility = false;

        public bool CommentVisibility {
            get { return _CommentVisibility; }
            set { 
                if(_CommentVisibility == value)
                    return;
                _CommentVisibility = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        //フルスクリーンかどうか
        #region IsFullScreen変更通知プロパティ
        private bool _IsFullScreen;

        public bool IsFullScreen {
            get { return _IsFullScreen; }
            set { 
                if(_IsFullScreen == value)
                    return;
                _IsFullScreen = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        //リピートするかどうか
        #region IsRepeat変更通知プロパティ
        private bool _IsRepeat;

        public bool IsRepeat {
            get { return _IsRepeat; }
            set { 
                if(_IsRepeat == value)
                    return;
                _IsRepeat = value;
                RaisePropertyChanged();
            }
        }
        #endregion
        

        #region Volume変更通知プロパティ
        public int Volume {
            get { return Properties.Settings.Default.Volume; }
            set { 
                Properties.Settings.Default.Volume = value;
                RaisePropertyChanged();
                if(value != 0) {

                    IsMute = false;
                }
                Properties.Settings.Default.Save();
                InvokeScript("AsChangeVolume", (value / 100.0).ToString());
            }
        }
        #endregion

        #region IsMute変更通知プロパティ
        private bool _IsMute;

        public bool IsMute {
            get { return _IsMute; }
            set { 
                if(_IsMute == value)
                    return;
                _IsMute = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region VideoFlash変更通知プロパティ UI要素だけどこればっかりは仕方ない
        private VideoFlash _VideoFlash;

        public VideoFlash VideoFlash {
            get { return _VideoFlash; }
            set { 
                if(_VideoFlash == value)
                    return;
                _VideoFlash = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region FullScreenVideoFlash変更通知プロパティ
        private VideoFlash _FullScreenVideoFlash;

        public VideoFlash FullScreenVideoFlash {
            get { return _FullScreenVideoFlash; }
            set { 
                if(_FullScreenVideoFlash == value)
                    return;
                _FullScreenVideoFlash = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Controller変更通知プロパティ
        private VideoController _Controller;

        public VideoController Controller {
            get { return _Controller; }
            set {
                if(_Controller == value)
                    return;
                _Controller = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region FullScreenContoller変更通知プロパティ
        private VideoController _FullScreenContoller;

        public VideoController FullScreenContoller {
            get { return _FullScreenContoller; }
            set { 
                if(_FullScreenContoller == value)
                    return;
                _FullScreenContoller = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        //Flashの関数を呼ぶためのもの
        public ExternalInterfaceProxy Proxy;


        #region AxShockwaveFlash変更通知プロパティ
        private AxShockwaveFlash _AxShockwaveFlash;

        public AxShockwaveFlash ShockwaveFlash {
            get { return _AxShockwaveFlash; }
            set {
                if(_AxShockwaveFlash == value)
                    return;
                _AxShockwaveFlash = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public string VideoInfoPlacement {
            get {
                return App.ViewModelRoot.Config.Video.VideoPlacement;
            }
            set {
                RaisePropertyChanged();
            }
        }

        #region SplitterHeight変更通知プロパティ

        public GridLength SplitterHeight {
            get { return Properties.Settings.Default.SplitterHeight; }
            set { 
                if(Properties.Settings.Default.SplitterHeight.Value == value.Value)
                    return;
                Properties.Settings.Default.SplitterHeight = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }
        #endregion


        #region VideoUrl変更通知プロパティ
        private string _VideoUrl;

        public string VideoUrl {
            get { return _VideoUrl; }
            set { 
                if(_VideoUrl == value)
                    return;
                _VideoUrl = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region LoadFailed変更通知プロパティ
        private bool _LoadFailed;

        public bool LoadFailed {
            get { return _LoadFailed; }
            set { 
                if(_LoadFailed == value)
                    return;
                _LoadFailed = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        //マイリスト関連
        public VideoMylistViewModel Mylist { get; set; }

        //コメント関連
        public VideoCommentViewModel Comment { get; set; }

        #region Cmsid変更通知プロパティ
        private string _Cmsid;

        public string Cmsid {
            get { return _Cmsid; }
            set {
                if(_Cmsid == value)
                    return;
                _Cmsid = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region FullScreenPopup変更通知プロパティ
        private bool _FullScreenPopup = true;

        public bool FullScreenPopup {
            get { return _FullScreenPopup; }
            private set { 
                if(_FullScreenPopup == value)
                    return;
                _FullScreenPopup = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public VideoViewModel(string videoUrl) : base(videoUrl.Substring(30)) {

            App.ViewModelRoot.AddTabAndSetCurrent(this);

            VideoUrl = videoUrl;
            Cmsid = Name;
            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                VideoFlash = new VideoFlash() { DataContext = this };
                Controller = new VideoController() { DataContext = this };
            }));

            Initialize(videoUrl + "?watch_harmful=1");
        }

        private void Initialize(string videoUrl) {

            IsActive = true;
            Task.Run(() => {

                Mylist = new VideoMylistViewModel(this);
                Comment = new VideoCommentViewModel(this);
                VideoData = new VideoData();


                Status = "動画情報取得中";
                //動画情報取得
                WatchApi = new NicoNicoWatchApi(videoUrl, this);
                VideoData.ApiData = WatchApi.GetWatchApiData();

                //ロードに失敗したら
                if(VideoData.ApiData == null) {

                    LoadFailed = true;
                    IsActive = false;
                    Status = "動画の読み込みに失敗しました。";
                    return;
                }

                //有料動画なら
                if(VideoData.ApiData.IsPaidVideo) {

                    App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(PaidVideoDialog), this, TransitionMode.Modal));
                    return;
                }

                DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                    if(VideoData.ApiData.Cmsid.Contains("nm")) {

                        VideoData.VideoType = NicoNicoVideoType.SWF;
                        ShockwaveFlash.LoadMovie(0, GetNMPlayerPath());

                    } else if(VideoData.ApiData.GetFlv.VideoUrl.StartsWith("rtmp")) {

                        VideoData.VideoType = NicoNicoVideoType.RTMP;
                        ShockwaveFlash.LoadMovie(0, GetRTMPPlayerPath());
                    } else {

                        if(VideoData.ApiData.MovieType == "flv") {

                            VideoData.VideoType = NicoNicoVideoType.FLV;
                        } else {

                            VideoData.VideoType = NicoNicoVideoType.MP4;
                        }
                        ShockwaveFlash.LoadMovie(0, GetPlayerPath());
                    }
                    Proxy.ExternalInterfaceCall += new ExternalInterfaceCallEventHandler(ExternalInterfaceHandler);
                    IsActive = false;

                    Task.Run(() => {

                        OpenVideo();
                    });

                }));
                Time = new VideoTime();
                //動画時間
                Time.VideoTimeString = NicoNicoUtil.ConvertTime(VideoData.ApiData.Length);

                if(VideoData.ApiData.GetFlv.IsPremium && !VideoData.ApiData.GetFlv.VideoUrl.StartsWith("rtmp")) {

                    Task.Run(() => {

                        StoryBoardStatus = "取得中";

                        var sb = new NicoNicoStoryBoard(VideoData.ApiData.GetFlv.VideoUrl);
                        VideoData.StoryBoardData = sb.GetStoryBoardData();

                        if(VideoData.StoryBoardData == null) {

                            StoryBoardStatus = "データ無し";
                        } else {

                            StoryBoardStatus = "取得完了";
                        }
                    });
                } else {

                    StoryBoardStatus = "データ無し";
                }

                CommentInstance = new NicoNicoComment(VideoData.ApiData, this);
                var list = CommentInstance.GetComment();
                if(list != null) {

                    foreach(var entry in list) {

                        VideoData.CommentData.Add(new CommentEntryViewModel(entry));
                    }
                    dynamic json = new DynamicJson();
                    json.array = list;

                    InjectComment(json.ToString());
                    Comment.CanComment = true;

                    //投稿者コメントがあったら取得する
                    if(VideoData.ApiData.HasOwnerThread) {

                        var ulist = CommentInstance.GetUploaderComment();
                        dynamic ujson = new DynamicJson();
                        json.array = ulist;

                        InjectUploaderComment(json.ToString());
                    }
                }

                if(!Properties.Settings.Default.CommentVisibility) {

                    InvokeScript("AsToggleComment");
                } else {

                    CommentVisibility = true;
                }
            });
        }

        //ツイートダイアログ表示
        public void OpenTweetDialog() {

            TweetDialogViewModel vm = new TweetDialogViewModel();
            string url = "https://twitter.com/intent/tweet?hashtags=" + VideoData.ApiData.Cmsid
                            + "&text=" + HttpUtility.UrlEncode(VideoData.ApiData.Title) + "(" + Time.VideoTimeString + ")&ref_src=twsrc%5Etfw&url=" + VideoUrl;
            url = url.Replace(" ", "%20");
            System.Diagnostics.Process.Start(url);
            vm.OriginalUri = new Uri(url);
            //App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.Contents.Misc.TweetDialog), vm, TransitionMode.Modal));
        }

        //リピート
        public void ToggleRepeat() {

            IsRepeat ^= true;
            Properties.Settings.Default.IsRepeat = IsRepeat;
            Properties.Settings.Default.Save();
        }

        public void ToggleComment() {

            CommentVisibility ^= true;
            Properties.Settings.Default.CommentVisibility = CommentVisibility;
            Properties.Settings.Default.Save();
            InvokeScript("AsToggleComment");
        }

        private int PrevVolume;

        public void ToggleMute() {

            IsMute ^= true;
            if(IsMute) {

                PrevVolume = Volume;
                Volume = 0;
            } else {

                Volume = PrevVolume;
            }
        }

        //フルスクリーンにする
        public void GoToFullScreen() {

            if(IsFullScreen) {

                return;
            }
            IsFullScreen = true;

            //リソースに登録
            var message = new TransitionMessage(typeof(FullScreenWindow), this, TransitionMode.NewOrActive);

            //ウィンドウからFlash部分を消去
            var temp = VideoFlash;
            VideoFlash = null;
            FullScreenVideoFlash = temp;
            var temp2 = Controller;
            Controller = null;
            FullScreenContoller = temp2;

            App.ViewModelRoot.Visibility = Visibility.Hidden;
            //フルスクリーンウィンドウ表示
            Messenger.Raise(message);

        }

        //ウィンドウモードに戻す
        public void ReturnFromFullScreen() {

            if(!IsFullScreen) {
                
                return;
            }

            IsFullScreen = false;

            //Flash部分をフルスクリーンウィンドウから消去
            Messenger.Raise(new WindowActionMessage(WindowAction.Close));

            //ウィンドウを閉じる
            App.ViewModelRoot.Visibility = Visibility.Visible;

            //ウィンドウにFlash部分を追加
            var temp = FullScreenVideoFlash;
            FullScreenVideoFlash = null;
            VideoFlash = temp;
            var temp2 = FullScreenContoller;
            FullScreenContoller = null;
            Controller = temp2;

        }

        //フルスクリーン切り替え
        public void ToggleFullScreen() {

            if(IsFullScreen) {

                ReturnFromFullScreen();
            } else {

                GoToFullScreen();
            }
        }

        //一時停止切り替え
        public void PlayOrPauseOrResume() {

            if(IsPlaying) {

                Pause();
            } else {

                Resume();
            }
        }
        
        //最初から
        public void Restart() {
            
            Seek(0);
        }
        
        private int prevTime;
        private double sumBPS;
        
        //1フレーム毎に呼ばれる
        public void CsFrame(float time, float buffer, long bps, string vpos) {

            if(prevTime != (int)time) {
               
                double comp = sumBPS / 1024;

                //大きいから単位を変える
                if(comp > 1000) {

                    BPS = Math.Floor((comp / 1024) * 100) / 100 + "MiB/秒";
                } else {

                    BPS = Math.Floor(comp * 100) / 100 + "KiB/秒";
                }
                sumBPS = 0;
            } else {

                sumBPS += bps;
            }
            prevTime = (int)time;

            Time.BufferedTime = buffer;
            Comment.Vpos = vpos;
            
            //Console.WriteLine(VideoData.ApiData.Cmsid + " time:" + time + " buffer:" + buffer + " vpos:" + vpos);

            SetSeekCursor(time);
        }

        //指定した時間でシークバーを移動する
        private void SetSeekCursor(float time) {

            Time.CurrentTime = (int)time;
            Time.CurrentTimeString = NicoNicoUtil.ConvertTime(Time.CurrentTime);
        }
        
        public void OpenVideo() {

            //ここからInvoke可能
            IsPlaying = true;
            IsInitialized = true;
            Mylist.EnableButtons();

            if(VideoData.VideoType == NicoNicoVideoType.RTMP) {

                InvokeScript("AsOpenVideo", VideoData.ApiData.GetFlv.VideoUrl + "^" + VideoData.ApiData.GetFlv.FmsToken);
            } else {

                InvokeScript("AsOpenVideo", VideoData.ApiData.GetFlv.VideoUrl);
            }
            
            IsRepeat = Properties.Settings.Default.IsRepeat;
        }
        
        private object ExternalInterfaceHandler(object sender, ExternalInterfaceCallEventArgs e) {

            InvokeFromActionScript(e.FunctionCall.FunctionName, e.FunctionCall.Arguments);
            return false;
        }
        //ExternalIntarface.callでActionscriptから呼ばれる
        public void InvokeFromActionScript(string func, params string[] args) {

            switch(func) {
                case "CsFrame": //毎フレーム呼ばれる
                    CsFrame(float.Parse(args[0]), float.Parse(args[1]), long.Parse(args[2]), args[3]);
                    break;
                case "NetConnection.Connect.Closed":    //RTMP動画再生時にタイムアウトになったら
                    RTMPTimeOut();
                    break;
                case "ShowController":  //マウスを動かしたら呼ばれる
                    ShowFullScreenPopup();
                    break;
                case "HideController":  //マウスを数秒動画の上で静止させたら呼ばれる
                    HideFullScreenPopup();
                    break;
                case "Initialized": //動画が再生される直前に呼ばれる
                    Volume = Properties.Settings.Default.Volume;    //保存された値をFlash側に伝える
                    break;
                case "Stop": //動画が最後まで行ったらリピートしたりフルスクリーンから復帰したりする
                    if(IsRepeat) {
                        
                        Restart();
                    } else if(IsFullScreen) {

                        ReturnFromFullScreen();
                    }
                    break;
                default:
                    Console.WriteLine("Invoked From Actionscript:" + func);
                    break;
            }
        }

        //Flashに一時停止命令を送る
        public void Pause() {

            IsPlaying = false;
            InvokeScript("AsPause");
        }

        //Flashに再生再開命令を送る
        public void Resume() {

            IsPlaying = true;
            InvokeScript("AsResume");
        }
        
        //Flashにシーク命令を送る
        public void Seek(float pos) {
            
            InvokeScript("AsSeek", pos.ToString());
        }

        //Flashにコメントリストを送る
        public void InjectComment(string json) {
            
           InvokeScript("AsInjectComment", json);
        }
        //Flashに投稿者コメントリストを送る
        public void InjectUploaderComment(string json) {

            InvokeScript("AsInjectUploaderComment", json);
        }


        //JSを呼ぶ
        private void InvokeScript(string func, params object[] args) {
            
            //読み込み前にボタンを押しても大丈夫なように メモリ解放されたあとに呼ばれないように
            if(ShockwaveFlash != null && !ShockwaveFlash.IsDisposed) {
                
                try {
                    
                    Proxy.Call(func, args);
                } catch(COMException) {

                    Console.WriteLine("COMException：" + func);
                }
            }
        }

        public string GetPlayerPath() {

            var cur = System.IO.Directory.GetCurrentDirectory();
            return cur + "./Flash/NicoNicoPlayer.swf";
        }

        public string GetNMPlayerPath() {

            var cur = System.IO.Directory.GetCurrentDirectory();
            return cur + "./Flash/NicoNicoNMPlayer.swf";
        }

        private string GetRTMPPlayerPath() {

            var cur = System.IO.Directory.GetCurrentDirectory();
            return cur + "./Flash/NicoNicoRTMPPlayer.swf";
        }

        //RTMP動画でタイムアウトになった時又は予期せぬ理由でエラーになった時
        public void RTMPTimeOut() {

            App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(VideoTimeOutDialog), this, TransitionMode.Modal));
        }
        

        public void Reflesh() {

            DisposeViewModel();
            new VideoViewModel(VideoUrl);
        }
        
        public void DisposeViewModel() {

            //ウェブブラウザ開放
            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                //UIスレッドじゃないとAccessViolationになる時がある
                ShockwaveFlash.Dispose();

                App.ViewModelRoot.RemoveTabAndLastSet(this);
                Dispose();
            }));
        }

        public override void KeyDown(KeyEventArgs e) {

            Console.WriteLine("KeyDown:" + e.Key);
            if(Comment.IsPopupOpen) {

                if(e.Key == Key.Enter) {

                    if(e.KeyboardDevice.Modifiers == ModifierKeys.Shift) {

                        Comment.AcceptEnter = true;
                    } else {

                        Comment.AcceptEnter = false;
                        Comment.Post();
                    }
                }
                return;
            }
            if(IsFullScreen) {

                switch(e.Key) {
                    case Key.Space:
                        PlayOrPauseOrResume();
                        break;
                    case Key.Escape:
                        ToggleFullScreen();
                        break;
                    case Key.S:
                        Restart();
                        break;
                    case Key.C:
                        ToggleComment();
                        break;
                    case Key.R:
                        ToggleRepeat();
                        break;
                    case Key.M:
                        ToggleMute();
                        break;
                }
            } else {
                switch(e.Key) {
                    case Key.Space:
                        PlayOrPauseOrResume();
                        break;
                    case Key.F:
                        ToggleFullScreen();
                        break;
                    case Key.S:
                        Restart();
                        break;
                    case Key.C:
                        ToggleComment();
                        break;
                    case Key.R:
                        ToggleRepeat();
                        break;
                    case Key.M:
                        ToggleMute();
                        break;
                    case Key.F5:
                        Reflesh();
                        break;
                }
                //Ctrl+Wで閉じる
                if(e.KeyboardDevice.Modifiers == ModifierKeys.Control) {

                    if(e.Key == Key.W) {

                        DisposeViewModel();
                    }
                }
            }
        }
       
        public void ToggleFavorite() {

            Task.Run(() => {

                if(!VideoData.ApiData.UploaderIsFavorited) {

                    var status = WatchApi.AddFavorite(VideoData.ApiData.UploaderId, VideoData.ApiData.Token);
                    if(status == Models.NicoNicoWrapper.Status.Success) {

                        VideoData.ApiData.UploaderIsFavorited = true;
                    }
                } else {

                    var status = WatchApi.DeleteFavorite(VideoData.ApiData.UploaderId, VideoData.ApiData.Token);
                    if(status == Models.NicoNicoWrapper.Status.Success) {

                        VideoData.ApiData.UploaderIsFavorited = false;
                    }
                }
            });
        }

        public void HideFullScreenPopup() {

            if(Properties.Settings.Default.AlwaysShowSeekBar) {

                return;
            }
            FullScreenPopup = false;
        }

        public void ShowFullScreenPopup() {

            FullScreenPopup = true;
        }
    }
}
