using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

using Livet;



using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;


namespace SRNicoNico.ViewModels {

	public class VideoViewModel : ViewModel {

        
		//キャッシュパス
		public string Path { get; set; }



		//ストリーミング
		public NicoNicoStream Stream { get; set; }

		//インラインシークバー画像プレビュー
		public NicoNicoStoryBoard StoryBoard { get; set; }

		//API結果
		public VideoData VideoData { get; set; }


		//各種シークバー関連の時間管理
		public VideoTime Time { get; set; }



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



		#region IconData変更通知プロパティ
		private GeometryGroup _IconData = Playing;

		public GeometryGroup IconData {
			get { return _IconData; }
			set {
				if(_IconData == value)
					return;
				_IconData = value;
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





        public VideoViewModel(WatchApiData apiData) {
            
            //ViewをVideoに変える
            App.ViewModelRoot.RightContent = this;
            App.ViewModelRoot.CurrentVideo = this;


            VideoData = new VideoData();
            VideoData.ApiData = apiData;

        }



        public VideoViewModel(string videoUrl) {


            //ViewをVideoに変える
            App.ViewModelRoot.RightContent = this;
            App.ViewModelRoot.CurrentVideo = this;


            VideoData = new VideoData();
            VideoData.ApiData = NicoNicoWatchApi.GetWatchApiData(videoUrl);

		}

        public void Initialize() {

            
           


        }


        //このメソッド以降はWebBrowserプロパティはnullではない
        public void OpenVideo() {

            InvokeScript("CsOpenVideo", VideoData.ApiData.GetFlv.VideoUrl);


        }



        //JSを呼ぶ
        private void InvokeScript(string func, params string[] args) {



            WebBrowser.InvokeScript(func, args);
        }
	}
}
