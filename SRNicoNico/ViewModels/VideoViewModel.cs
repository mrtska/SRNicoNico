using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System;

using Livet;

using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using Livet.Messaging;
using SRNicoNico.Views.Contents.Video;

using Codeplex.Data;
using System.Collections.Generic;

namespace SRNicoNico.ViewModels {

	public class VideoViewModel : TabItemViewModel {

        
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


		#region SeekCursor変更通知プロパティ
		private Thickness _SeekCursor;

		public Thickness SeekCursor {
			get { return _SeekCursor; }
			set {
				if(_SeekCursor == value)
					return;
				_SeekCursor = value;
				RaisePropertyChanged();
			}
		}
		#endregion


		#region LoadStatus変更通知プロパティ
		private string _LoadStatus;

		public string LoadStatus {
			get { return _LoadStatus; }
			set {
				if(_LoadStatus == value)
					return;
				_LoadStatus = value;
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


        #region IsSeekChanged変更通知プロパティ
        private bool _IsSeekChanged;

        public bool IsSeekChanged {
            get { return _IsSeekChanged; }
            set { 
                if(_IsSeekChanged == value)
                    return;
                _IsSeekChanged = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        #region WebBrowser変更通知プロパティ
        private WebBrowser _WebBrowser;

        public WebBrowser WebBrowser {
            get { return _WebBrowser; }
            set { 
                if(_WebBrowser == value)
                    return;
                _WebBrowser = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        //NicoNicoPlayerへのパス
        public string Address {

            get {

                var cur = System.IO.Directory.GetCurrentDirectory();
                return cur + "./Flash/NicoNicoPlayer.html";
            }
        }


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


        #region Video変更通知プロパティ UI要素 仕方ないんや・・・
        private Video _Video;

        public Video Video {
            get { return _Video; }
            set { 
                if(_Video == value)
                    return;
                _Video = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region FullScreenWindow変更通知プロパティ UI要素 仕方ないんです
        private FullScreenWindow _FullScreenWindow;

        public FullScreenWindow FullScreenWindow {
            get { return _FullScreenWindow; }
            set { 
                if(_FullScreenWindow == value)
                    return;
                _FullScreenWindow = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        public VideoViewModel(string videoUrl) : base("動画") {

            App.ViewModelRoot.TabItems.Add(this);

            Initialize(videoUrl);
		}

        private void Initialize(string videoUrl) {

            Task.Run(() => {

                VideoData = new VideoData();


                App.ViewModelRoot.StatusBar.Status = "動画情報取得中";
                //動画情報取得
                VideoData.ApiData = NicoNicoWatchApi.GetWatchApiData(videoUrl);

                Time = new VideoTime();

                //動画時間
                Time.VideoTimeString = NicoNicoUtil.GetTimeFromLong(VideoData.ApiData.Length);

                App.ViewModelRoot.StatusBar.Status = "ストーリーボード取得中";

                NicoNicoStoryBoard sb = new NicoNicoStoryBoard(VideoData.ApiData.GetFlv.VideoUrl);
                VideoData.StoryBoardData = sb.GetStoryBoardData();

                NicoNicoComment comment = new NicoNicoComment(VideoData.ApiData.GetFlv);

                List<NicoNicoCommentEntry> list = comment.GetComment();


                if(list != null) {

                    foreach(NicoNicoCommentEntry entry in list) {

                        VideoData.CommentData.Add(new CommentEntryViewModel(entry));
                    }

                    dynamic json = new DynamicJson();
                    json.array = list;

                    DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => InjectComment(json.ToString())));
                }

                //App.ViewModelRoot.StatusBar.Status = "動画取得完了";
            });
        }

        public void OpenLink(Uri uri) {

            //ウェブブラウザ開放
            WebBrowser.IsEnabled = false;
            WebBrowser.Dispose();

            string cmsid = uri.OriginalString;

            if(cmsid.Contains("sm")) {

                new VideoViewModel("http://www.nicovideo.jp/watch/" + cmsid);
                Dispose();
            }
        }




        //フルスクリーンにする
        public void GoToFullScreen() {

            if(IsFullScreen) {

                return;
            }
            IsFullScreen = true;

            //リソースに登録
            Application.Current.Resources["VideoFlashKey"] = VideoFlash;
            var message = new TransitionMessage(typeof(FullScreenWindow), this, TransitionMode.Modal);

            //ウィンドウからFlash部分を消去
            Video.Grid.Children.Remove(VideoFlash);

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
            FullScreenWindow.ContentControl.Content = null;

            //ウィンドウを閉じる
            FullScreenWindow.Close();


            //ウィンドウにFlash部分を追加
            Video.Grid.Children.Add(VideoFlash);

            //フォーカス
            Video.Focus();

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

                IsPlaying = false;
                Pause();
            } else {

                IsPlaying = true;
                Resume();
            }

        }

        //最初から
        public void Restart() {

            Seek(0);
        }

        //1フレーム毎に呼ばれる
        public void CsFrame(float time, float buffer, long bps) {


            double vpos = time * 100;
            vpos = Math.Floor(vpos);

            
            BPS = (bps / 1024).ToString() + "KiB/秒";

            Time.BufferedTimeWidth = buffer * WebBrowser.ActualWidth;

            Console.WriteLine(VideoData.ApiData.Cmsid + " time:" + time + " buffer:" + buffer + " bps:" + bps);

            SetSeekCursor(time);
        }

        //指定した時間でシークバーを移動する
        private void SetSeekCursor(float time) {

            Time.CurrentTime = (int)time;
            Time.CurrentTimeMilis = (int)(time * 100);
            Time.CurrentTimeString = NicoNicoUtil.GetTimeFromLong(Time.CurrentTime);
            Time.CurrentTimeWidth = WebBrowser.ActualWidth / VideoData.ApiData.Length * Time.CurrentTime;
            SeekCursor = new Thickness(Time.CurrentTimeWidth - 10, 0, 0, 0);

        }

        //このメソッド以降はWebBrowserプロパティはnullではない
        public void OpenVideo() {

			while(VideoData.ApiData == null) {

				Thread.Sleep(50);
			}

            WebBrowser.ObjectForScripting = new ObjectForScriptingHelper(this);
            IsPlaying = true;
            InvokeScript("JsOpenVideo", VideoData.ApiData.GetFlv.VideoUrl);
            
            
        }


        //Flashに一時停止命令を送る
        public void Pause() {

            InvokeScript("JsPause");
        }

        //Flashに再生再開命令を送る
        public void Resume() {

            InvokeScript("JsResume");
        }

        //Flashにシーク命令を送る
        public void Seek(float pos) {

            InvokeScript("JsSeek", pos.ToString());
        }

        //Flashにコメントリストを送る
        public void InjectComment(string json) {
            
           InvokeScript("JsInjectComment", json);
        }

        //JSを呼ぶ
        private void InvokeScript(string func, params string[] args) {

            
            if(WebBrowser != null && WebBrowser.IsEnabled) {

                WebBrowser.InvokeScript(func, args);
            }
            
        }








    }
}
