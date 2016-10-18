using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using SRNicoNico.Views.Controls;

namespace SRNicoNico.Models.NicoNicoViewer {
    public class VideoTime : NotificationObject {

        //動画の現在時間
        #region CurrentTime変更通知プロパティ
        private double _CurrentTime;

        public double CurrentTime {
            get { return _CurrentTime; }
            set {
                if(_CurrentTime == value)
                    return;
                _CurrentTime = value;
                RaisePropertyChanged();
            }
        }
        #endregion
        

        #region Length変更通知プロパティ
        private int _Length;

        public int Length {
            get { return _Length; }
            set {
                if(_Length == value)
                    return;
                _Length = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        //バッファリングが終わった時間
        #region BufferedRange変更通知プロパティ
        private DispatcherCollection<TimeRange> _BufferedRange = new DispatcherCollection<TimeRange>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<TimeRange> BufferedRange {
            get { return _BufferedRange; }
            set {
                if(_BufferedRange == value)
                    return;

                _BufferedRange = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region PlayedRange変更通知プロパティ
        private DispatcherCollection<TimeRange> _PlayedRange = new DispatcherCollection<TimeRange>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<TimeRange> PlayedRange {
            get { return _PlayedRange; }
            set {
                if(_PlayedRange == value)
                    return;
                _PlayedRange = value;
                RaisePropertyChanged();
            }
        }
        #endregion

    }
}