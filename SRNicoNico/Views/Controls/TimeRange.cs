using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;

namespace SRNicoNico.Views.Controls {
    public class TimeRange : NotificationObject {


        #region Start変更通知プロパティ
        private double _Start;

        public double Start {
            get { return _Start; }
            set { 
                if(_Start == value)
                    return;
                _Start = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public double StartTime;

        public double EndTime;

        

        #region Width変更通知プロパティ
        private double _Width;

        public double Width {
            get { return _Width; }
            set { 
                if(_Width == value)
                    return;
                _Width = value;
                RaisePropertyChanged();
            }
        }
        #endregion


    }
}
