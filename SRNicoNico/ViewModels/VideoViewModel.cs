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

namespace SRNicoNico.ViewModels {

	public class VideoViewModel : ViewModel {

        
		//キャッシュパス
		public string Path { get; set; }
        

		//インラインシークバー画像プレビュー
		public NicoNicoStoryBoard StoryBoard { get; set; }


        public CommentViewModel Comment { get; set; }


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




        public VideoViewModel(string videoUrl) {

            //ViewをVideoに変える
            App.ViewModelRoot.RightContent = this;
            App.ViewModelRoot.CurrentVideo = this;

            Comment = new CommentViewModel(this);

            Initialize(videoUrl);
		}

        public void Initialize(string videoUrl) {

            Task.Run(() => {

                VideoData = new VideoData();
                VideoData.ApiData = NicoNicoWatchApi.GetWatchApiData(videoUrl);

                Time = new VideoTime();
                Time.VideoTimeString = NicoNicoUtil.GetTimeFromLong(VideoData.ApiData.Length);

                NicoNicoStoryBoard sb = new NicoNicoStoryBoard(VideoData.ApiData.GetFlv.VideoUrl);
                VideoData.StoryBoardData = sb.GetStoryBoardData();

                NicoNicoComment comment = new NicoNicoComment(VideoData.ApiData.GetFlv);

                foreach(NicoNicoCommentEntry entry in comment.GetComment()) {

                    VideoData.CommentData.Add(new CommentEntryViewModel(entry));
                }

                DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                    if(Comment.View != null) {

                        //コメントのViewを初期化する
                        Comment.View.Initialize(VideoData.CommentData);
                        CommentVisibility = true;

                    }
                }));
               

            });

            
        }

        public void GoToFullScreen() {

            
            Application.Current.Resources["VideoFlashKey"] = VideoFlash;
            var message = new TransitionMessage(typeof(FullScreenWindow), this, TransitionMode.Modal);

            Video.Grid.Children.Remove(VideoFlash);
            Messenger.Raise(message);


        }

        public void PlayOrPauseOrResume() {

            if(IsPlaying) {

                IsPlaying = false;
                Pause();
            } else {

                IsPlaying = true;
                Resume();
            }

        }

        private double PrevVpos = 0;

        //1フレーム毎に呼ばれる
        public void CsFrame(float time, float buffer) {


            double vpos = time * 100;
            vpos = Math.Floor(vpos);

            //Console.WriteLine(VideoData.ApiData.Cmsid + " " + time + ":" + buffer + " vpos:" + vpos);


            Time.CurrentTime = (int) time;
            Time.CurrentTimeMilis = (int) (time * 100);
            Time.CurrentTimeString = NicoNicoUtil.GetTimeFromLong(Time.CurrentTime);
            Time.CurrentTimeWidth = WebBrowser.ActualWidth / VideoData.ApiData.Length * Time.CurrentTime;
            SeekCursor = new Thickness(Time.CurrentTimeWidth, 0, 0, 0);


            

            int kBps = BPSCounter.Bps / 1024;
            //数字がデカかったら単位を変えましょう
            if(kBps > 1024) {

                BPS = (Math.Truncate((float)kBps / 1024 * 100) / 100) + "MiB/秒";
            } else {

                BPS = kBps + "KiB/秒";
            }

            if(VideoData.ApiData.Length == time) {

                Seek(0);
            }

            if(PrevVpos == vpos) {
                Thread.Sleep(1);
                return;
            }
            PrevVpos = vpos;
            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                if(CommentVisibility && Comment.View != null) {

                    Comment.View.RenderComment(vpos);
                }
            }));


        }

        //このメソッド以降はWebBrowserプロパティはnullではない
        public void OpenVideo() {

			while(VideoData.ApiData == null && !CommentVisibility) {

				Thread.Sleep(50);
			}

            WebBrowser.ObjectForScripting = new ObjectForScriptingHelper(this);
            IsPlaying = true;
            InvokeScript("JsOpenVideo", VideoData.ApiData.GetFlv.VideoUrl);
            

        }



        public void Pause() {

            Comment.View.Pause();
            InvokeScript("JsPause");
        }

        public void Resume() {

            Comment.View.Resume();
            InvokeScript("JsResume");
        }

        public void Seek(float pos) {

            Comment.View.Seek();
            InvokeScript("JsSeek", pos.ToString());
            IsSeekChanged = true;

        }

        //JSを呼ぶ
        private void InvokeScript(string func, params string[] args) {



            WebBrowser.InvokeScript(func, args);
        }








    }
}
