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
using SRNicoNico.Views.Contents.Interface;

namespace SRNicoNico.ViewModels {

	public class VideoViewModel : TabItemViewModel, IExternalizable {

        //APIバックエンドインスタンス
        public NicoNicoWatchApi WatchApi;

        //コメントバックエンドインスタンス
        public NicoNicoComment CommentInstance;

        public VideoFlashHandler Handler;


        #region Status変更通知プロパティ
        public new string Status {
            get { return base.Status; }
            set { 
                if(base.Status == value)
                    return;
                base.Status = value;
                RaisePropertyChanged();
            }
        }
        #endregion


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
                if(IsInitialized && Time.CurrentTime == Time.Length && value) {

                    Restart();
                }
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
        private bool _IsFullScreen = false;

        public bool IsFullScreen {
            get { return _IsFullScreen; }
            set { 
                if(_IsFullScreen == value)
                    return;
                _IsFullScreen = value;
                if(IsPlayList) {

                    PlayListEntry.Owner.IsFullScreen = value;
                }
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
            get { return Settings.Instance.Volume; }
            set { 
                if(value > 100) {

                    value = 100;
                } else if(value < 0) {

                    value = 0;
                }
                Settings.Instance.Volume = value;

                if(value != 0) {

                    IsMute = false;
                }
                Handler.InvokeScript("AsChangeVolume", (value / 100.0).ToString());
                RaisePropertyChanged();
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
        private VideoController _FullScreenController;
        
        public VideoController FullScreenController {
            get { return _FullScreenController; }
            set { 
                if(_FullScreenController == value)
                    return;
                _FullScreenController = value;
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
        #region Comment変更通知プロパティ
        private VideoCommentViewModel _Comment;

        public VideoCommentViewModel Comment {
            get { return _Comment; }
            set { 
                if(_Comment == value)
                    return;
                _Comment = value;
                RaisePropertyChanged();
            }
        }
        #endregion

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
            set { 
                if(_FullScreenPopup == value)
                    return;
                _FullScreenPopup = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        Control IExternalizable.View { get; set; }
        TabItemViewModel IExternalizable.ViewModel {
            get {
                return this;
            }
        }





        //プレイリストだったら
        internal readonly bool IsPlayList = false;
        internal PlayListEntryViewModel PlayListEntry;

        public VideoViewModel(string videoUrl) : base(videoUrl.Substring(30)) {

            if(videoUrl.Contains("?")) {

                videoUrl = videoUrl.Split('?')[0];
                Name = videoUrl.Substring(30);
            }

            VideoUrl = videoUrl;
            Cmsid = Name;
        }
        public VideoViewModel(PlayListEntryViewModel entry, bool isFullScreen = false) : this(entry.VideoUrl) {

            IsPlayList = true;
            PlayListEntry = entry;
            if(isFullScreen) {

                IsFullScreen = true;
            }
        }

        public async void Initialize() {

            Mylist = new VideoMylistViewModel(this);
            Comment = new VideoCommentViewModel(this);
            Handler = new VideoFlashHandler(this);
            Time = new VideoTime();
            VideoData = new VideoData();

            await DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {


                if(IsFullScreen) {

                    FullScreenVideoFlash = new VideoFlash() { DataContext = this };
                    FullScreenController = new VideoController() { DataContext = this };
                } else {

                    VideoFlash = new VideoFlash() { DataContext = this };
                    Controller = new VideoController() { DataContext = this };
                }

            }));

            var videoUrl = VideoUrl + "?watch_harmful=1";

            IsActive = true;

            Status = "動画情報取得中";
            //動画情報取得

            await Task.Run(() => {

                WatchApi = new NicoNicoWatchApi(videoUrl, this);
                VideoData.ApiData = WatchApi.GetWatchApiData();
                Handler.Initialize(VideoData);
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


        //最初から
        public void Restart() {

            Handler.Restart();
        }

        //リピート
        public void ToggleRepeat() {

            IsRepeat ^= true;
            Settings.Instance.IsRepeat = IsRepeat;
        }

        public void ToggleComment() {

            CommentVisibility ^= true;
            Settings.Instance.CommentVisibility = CommentVisibility;
            Handler.InvokeScript("AsToggleComment");
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


            IsFullScreen = true;

            Type type;
            if(Settings.Instance.UseWindowMode) {

                type = typeof(WindowedWindow);
            } else {

                type = typeof(FullScreenWindow);
            }


            //リソースに登録
            var message = new TransitionMessage(type, this, TransitionMode.NewOrActive);

            //ウィンドウからFlash部分を消去
            var temp = VideoFlash;
            VideoFlash = null;
            FullScreenVideoFlash = temp;
            var temp2 = Controller;
            Controller = null;
            FullScreenController = temp2;


            App.ViewModelRoot.Visibility = Visibility.Hidden;

            if(IsPlayList) {

                PlayListEntry.Owner.ToFullScreen();
                return;
            }
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
            //Messenger.Raise(new WindowActionMessage(WindowAction.Close));
            Window.GetWindow(FullScreenVideoFlash).Close(); //消えない時があるから強引に

            //ウィンドウを閉じる
            App.ViewModelRoot.Visibility = Visibility.Visible;

            //ウィンドウにFlash部分を追加
            var temp = FullScreenVideoFlash;
            FullScreenVideoFlash = null;
            VideoFlash = temp;
            var temp2 = FullScreenController;
            FullScreenController = null;
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
        public void TogglePlay() {

            Handler.TogglePlay();
        }


        //指定した時間でシークバーを移動する
        public void SetSeekCursor(float time) {

            Time.CurrentTime = (int)time;
            Time.CurrentTimeString = NicoNicoUtil.ConvertTime(Time.CurrentTime);
        }
        
        public void OpenNew() {

            NicoNicoOpener.OpenNew(this);
        }

        //RTMP動画でタイムアウトになった時又は予期せぬ理由でエラーになった時
        public void RTMPTimeOut() {

            App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(VideoTimeOutDialog), this, TransitionMode.Modal));
        }
        

        public void Refresh() {


            if(IsPlayList) {

                PlayListEntry.Owner.Jump(PlayListEntry);
            } else {

                DisposeViewModel();
                NicoNicoOpener.Replace(this, VideoUrl);
            }

        }

        public void Close() {

            DisposeViewModel();
            if(IsPlayList) {

                App.ViewModelRoot.RemoveTabAndLastSet(PlayListEntry.Owner);
            }
            App.ViewModelRoot.RemoveTabAndLastSet(this);
        }
        
        public async void DisposeViewModel() {

            //ウェブブラウザ開放
            await DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                //UIスレッドじゃないとAccessViolationになる時がある
                Handler.DisposeHandler();
                VideoFlash = null;
                Controller = null;
                FullScreenVideoFlash = null;
                FullScreenController = null;

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
                } else if(e.Key == Key.Escape) {

                    Comment.IsPopupOpen = false;
                }
                return;
            }
            if(IsFullScreen) {

                switch(e.Key) {
                    case Key.Space:
                        Handler.TogglePlay();
                        break;
                    case Key.Escape:
                        ToggleFullScreen();
                        break;
                    case Key.S:
                        Handler.Restart();
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
                    case Key.Enter:
                        FocusComment();
                        break;
                    case Key.Up:
                        if(Volume <= 90) {

                            Volume += 10;
                        } else {

                            Volume = 100;
                        }
                        break;
                    case Key.Down:
                        if(Volume >= 10) {

                            Volume -= 10;
                        } else {

                            Volume = 0;
                        }
                        break;
                    case Key.N:
                        if(IsPlayList) {
                            PlayListEntry.Owner.Next();
                        }
                        break;
                    case Key.P:
                        if(IsPlayList) {
                            PlayListEntry.Owner.Prev();
                        }
                        break;

                }
            } else {
                switch(e.Key) {
                    case Key.Space:
                        Handler.TogglePlay();
                        break;
                    case Key.F:
                        ToggleFullScreen();
                        break;
                    case Key.S:
                        Handler.Restart();
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
                        Refresh();
                        break;
                    case Key.Enter:
                        FocusComment();
                        break;
                    case Key.Up:
                        if(Volume <= 90) {

                            Volume += 10;
                        } else {

                            Volume = 100;
                        }
                        break;
                    case Key.Down:
                        if(Volume >= 10) {

                            Volume -= 10;
                        } else {

                            Volume = 0;
                        }
                        break;
                }
                //Ctrl+Wで閉じる
                if(e.KeyboardDevice.Modifiers == ModifierKeys.Control) {

                    if(e.Key == Key.W) {

                        Close();
                    }
                }
            }
        }
       public void FocusComment() {

            if(Comment.CanComment) {

                if(IsFullScreen) {

                    Comment.IsPopupOpen = true;
                    FullScreenController.FocusComment();
                } else {

                    Comment.IsPopupOpen = true;
                    Controller.FocusComment();
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

            //常に表示の時は隠さない
            if(Settings.Instance.AlwaysShowSeekBar) {

                return;
            }
            FullScreenPopup = false;
        }

        public void ShowFullScreenPopup() {

            FullScreenPopup = true;
        }
    }
}
