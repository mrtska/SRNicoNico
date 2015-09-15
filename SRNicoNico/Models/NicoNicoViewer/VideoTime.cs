using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

namespace SRNicoNico.Models.NicoNicoViewer {
	public class VideoTime : NotificationObject {
		/*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */



		#region CurrentTime変更通知プロパティ
		private long _CurrentTime;

		public long CurrentTime {
			get { return _CurrentTime; }
			set {
				if (_CurrentTime == value)
					return;
				_CurrentTime = value;
				RaisePropertyChanged();
			}
		}
        #endregion

        #region CurrentTimeString変更通知プロパティ
        private string _CurrentTimeString = "0:00";

		public string CurrentTimeString {
			get { return _CurrentTimeString; }
			set { 
				if(_CurrentTimeString == value)
					return;
				_CurrentTimeString = value;
				RaisePropertyChanged();
			}
		}
		#endregion



        #region VideoTimeString変更通知プロパティ
        private string _VideoTimeString;

        public string VideoTimeString {
            get { return _VideoTimeString; }
            set { 
                if(_VideoTimeString == value)
                    return;
                _VideoTimeString = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region BufferedTime変更通知プロパティ
        private double _BufferedTime;

        public double BufferedTime {
            get { return _BufferedTime; }
            set { 
                if(_BufferedTime == value)
                    return;
                _BufferedTime = value;
                RaisePropertyChanged();
            }
        }
        #endregion

    }
}
