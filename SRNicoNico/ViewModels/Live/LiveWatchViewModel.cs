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
using System.Windows;

namespace SRNicoNico.ViewModels {
    public class LiveWatchViewModel : TabItemViewModel {

        private NicoNicoLive LiveInstance;

        internal NicoNicoLiveComment LiveCommentInstance;

        public LiveFlashHandler Handler;


        #region PermAria変更通知プロパティ
        private string _PermAria = "ニコニコ生放送";

        public string PermAria {
            get { return _PermAria; }
            set { 
                if(_PermAria == value)
                    return;
                _PermAria = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region CommentList変更通知プロパティ
        private DispatcherCollection<NicoNicoCommentEntry> _CommentList = new DispatcherCollection<NicoNicoCommentEntry>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<NicoNicoCommentEntry> CommentList {
            get { return _CommentList; }
            set { 
                if(_CommentList == value)
                    return;
                _CommentList = value;
                RaisePropertyChanged();
            }
        }
        #endregion


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

        #region Content変更通知プロパティ
        private NicoNicoLiveContent _Content;

        public NicoNicoLiveContent Content {
            get { return _Content; }
            set { 
                if(_Content == value)
                    return;
                _Content = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        #region DescriptionBrowser変更通知プロパティ
        private WebBrowser _DescriptionBrowser;

        public WebBrowser DescriptionBrowser {
            get { return _DescriptionBrowser; }
            set { 
                if(_DescriptionBrowser == value)
                    return;
                _DescriptionBrowser = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region LiveFlash変更通知プロパティ
        private LiveFlash _LiveFlash;

        public LiveFlash LiveFlash {
            get { return _LiveFlash; }
            set { 
                if(_LiveFlash == value)
                    return;
                _LiveFlash = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region FullScreenLiveFlash変更通知プロパティ
        private LiveFlash _FullScreenLiveFlash;

        public LiveFlash FullScreenLiveFlash {
            get { return _FullScreenLiveFlash; }
            set { 
                if(_FullScreenLiveFlash == value)
                    return;
                _FullScreenLiveFlash = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Controller変更通知プロパティ
        private UserControl _Controller;

        public UserControl Controller {
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
        private UserControl _FullScreenContoller;

        public UserControl FullScreenContoller {
            get { return _FullScreenContoller; }
            set {
                if(_FullScreenContoller == value)
                    return;
                _FullScreenContoller = value;
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

        #region IsFullScreen変更通知プロパティ
        private bool _IsFullScreen = false;

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

        #region Volume変更通知プロパティ
        public int Volume {
            get { return Settings.Instance.Volume; }
            set {
                Settings.Instance.Volume = value;
                RaisePropertyChanged();
                if(value != 0) {

                    IsMute = false;
                }
                Handler.InvokeScript("AsChangeVolume", (value / 100.0).ToString());
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


        private LiveViewModel Owner;

        #region Comment変更通知プロパティ
        private LiveCommentViewModel _Comment;

        public LiveCommentViewModel Comment {
            get { return _Comment; }
            set { 
                if(_Comment == value)
                    return;
                _Comment = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public new string Status {

            get {
                return Owner.Status;
            }
            set {

                Owner.Status = value;
            }

        }


        public LiveWatchViewModel(LiveViewModel vm, NicoNicoLive instance, NicoNicoLiveContent content) {

            Owner = vm;
            LiveInstance = instance;
            Content = content;

            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                LiveFlash = new LiveFlash() { DataContext = this };

                if(Content.GetPlayerStatus.Archive) {

                    Controller = new TimeShiftController() { DataContext = this };
                } else {

                    Controller = new LiveController() { DataContext = this };
                }


            }));

            Task.Run(() => Initialize());
        }

        public async void Initialize() {

            while(DescriptionBrowser == null) {

                Thread.Sleep(1);
            }
            await DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                DescriptionBrowser.NavigateToString(Content.Description);
                Handler.LoadMovie();
                
            }));
            LiveCommentInstance = new NicoNicoLiveComment(Content.GetPlayerStatus.MesseageServerUrl, Content.GetPlayerStatus.MesseageServerPort, this);

            Time = new VideoTime();


            Time.VideoTimeString = NicoNicoUtil.GetTimeFromVpos(Content.GetPlayerStatus, (int.Parse(Content.GetPlayerStatus.EndTime) - int.Parse(Content.GetPlayerStatus.BaseTime)) * 100);
            Time.Length = int.Parse(Content.GetPlayerStatus.EndTime) - int.Parse(Content.GetPlayerStatus.BaseTime);
            Time.BufferedTime = Time.Length;

            Comment = new LiveCommentViewModel(this);


            OpenVideo();

            //タイムシフトじゃなかったらすぐに再生
            if(!Content.GetPlayerStatus.Archive) {

                foreach(var content in Content.GetPlayerStatus.ContentsList) {

                    Handler.InvokeScript("AsCommandExcute", "/liveplay", "0", content.Content);
                }

  

            }
        }
        
        public void OpenVideo() {

            var json = Content.GetPlayerStatus.ToJson();
            Handler.InvokeScript("AsOpenVideo", Content.GetPlayerStatus.RtmpUrl, json);
        }
    
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            if(disposing) {


                DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                    LiveCommentInstance.Dispose();
                    Handler.DisposeHandler();
                   
                }));
            }
        }


        //フルスクリーンにする
        public void GoToFullScreen() {

            if(IsFullScreen) {

                return;
            }
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
            var temp = LiveFlash;
            LiveFlash = null;
            FullScreenLiveFlash = temp;
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
            //Messenger.Raise(new WindowActionMessage(WindowAction.Close));
            Window.GetWindow(FullScreenLiveFlash).Close(); //消えない時があるから強引に

            //ウィンドウを閉じる
            App.ViewModelRoot.Visibility = Visibility.Visible;

            //ウィンドウにFlash部分を追加
            var temp = FullScreenLiveFlash;
            FullScreenLiveFlash = null;
            LiveFlash = temp;
            var temp2 = FullScreenContoller;
            FullScreenContoller = null;
            Controller = temp2;

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
        //フルスクリーン切り替え
        public void ToggleFullScreen() {

            if(IsFullScreen) {

                ReturnFromFullScreen();
            } else {

                GoToFullScreen();
            }
        }
        public void FocusComment() {

            if(Comment.CanComment) {

                if(IsFullScreen) {

                    Comment.IsPopupOpen = true;

                    if(FullScreenContoller is LiveController) {

                        ((LiveController) FullScreenContoller).FocusComment();
                    } 
                  
                } else {

                    Comment.IsPopupOpen = true;

                    if(Controller is LiveController) {

                        ((LiveController) Controller).FocusComment();
                    }
                   
                }
            }
        }


        //一時停止切り替え
        public void TogglePlay() {

            if(IsPlaying) {

                Handler.Pause();
            } else {

                Handler.Resume();
            }
        }
        
        
        public void ToggleComment() {

            CommentVisibility ^= true;
            Settings.Instance.CommentVisibility = CommentVisibility;
            Handler.InvokeScript("AsToggleComment");
        }
        //最初から
        public void Restart() {

            Handler.Seek(0);
        }
        public void Close() {
            
            App.ViewModelRoot.RemoveTabAndLastSet(Owner);
            Dispose();
        }
        public override void KeyDown(KeyEventArgs e) {

            Console.WriteLine("KeyDown:" + e.Key);
            if(Content.GetPlayerStatus.Archive) {

                if(IsFullScreen) {

                    switch(e.Key) {
                        case Key.Space:
                            TogglePlay();
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
                        case Key.M:
                            ToggleMute();
                            break;
                    }
                } else {
                    switch(e.Key) {
                        case Key.Space:
                            TogglePlay();
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
                        case Key.M:
                            ToggleMute();
                            break;
                        case Key.F5:
                            Owner.Refresh();
                            break;
                    }
                }
            } else {

                if(IsFullScreen) {

                    switch(e.Key) {
                        case Key.Escape:
                            ToggleFullScreen();
                            break;
                        case Key.C:
                            ToggleComment();
                            break;
                        case Key.M:
                            ToggleMute();
                            break;
                        case Key.Enter:
                            FocusComment();
                            break;
                    }
                } else {
                    switch(e.Key) {
                        case Key.F:
                            ToggleFullScreen();
                            break;
                        case Key.C:
                            ToggleComment();
                            break;
                        case Key.M:
                            ToggleMute();
                            break;
                        case Key.F5:
                            Owner.Refresh();
                            break;
                        case Key.Enter:
                            FocusComment();
                            break;
                    }
                }
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
            }

            //Ctrl+Wで閉じる
            if(e.KeyboardDevice.Modifiers == ModifierKeys.Control) {

                if(e.Key == Key.W) {

                    Close();
                }
            }
        }
    }
}
