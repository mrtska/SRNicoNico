using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Livet;
using System.Windows;

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


        #region End変更通知プロパティ
        private double _End;

        public double End {
            get { return _End; }
            set { 
                if(_End == value)
                    return;
                _End = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region Position変更通知プロパティ
        private Thickness _Position;

        public Thickness Position {
            get { return _Position; }
            set { 
                if(_Position == value)
                    return;
                _Position = value;
                RaisePropertyChanged();
            }
        }
        #endregion


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
