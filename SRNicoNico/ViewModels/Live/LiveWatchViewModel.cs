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
        private LiveController _Controller;

        public LiveController Controller {
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
        private LiveController _FullScreenContoller;

        public LiveController FullScreenContoller {
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
            get { return Properties.Settings.Default.Volume; }
            set {
                Properties.Settings.Default.Volume = value;
                RaisePropertyChanged();
                if(value != 0) {

                    IsMute = false;
                }
                Properties.Settings.Default.Save();
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
                Controller = new LiveController() { DataContext = this };
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


           OpenVideo();
        }
        
        public void OpenVideo() {

            var json = Content.GetPlayerStatus.ToJson();
            Handler.InvokeScript("AsOpenVideo", Content.GetPlayerStatus.RtmpUrl, json);
        }
    
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            if(disposing) {

                DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                    Handler.DisposeHandler();
                    App.ViewModelRoot.RemoveTabAndLastSet(Owner);
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
            if(App.ViewModelRoot.Config.Video.UseWindowFullScreen) {

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

        //フルスクリーン切り替え
        public void ToggleFullScreen() {

            if(IsFullScreen) {

                ReturnFromFullScreen();
            } else {

                GoToFullScreen();
            }
        }

        public void Close() {

            Dispose();
        }

        public override void KeyDown(KeyEventArgs e) {

            if(e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.W) {

                Dispose();
            } else if(e.Key == Key.F5) {

                Owner.Refresh();
            }
        }


    }
}
