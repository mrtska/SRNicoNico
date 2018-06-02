using Livet;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SRNicoNico.ViewModels {
    public class StatusBarViewModel : ViewModel {

        #region Status変更通知プロパティ
        private string _Status;

        public string Status {
            get { return _Status; }
            set { 
                if(_Status == value)
                    return;
                _Status = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Time変更通知プロパティ
        private string _Time;

        public string Time {
            get { return _Time; }
            set { 
                if(_Time == value)
                    return;
                _Time = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public void TimerStart() {

            Task.Run(() => {

                for(;;) {

                    Time = DateTime.Now.ToString();
                    Thread.Sleep(1000);
                }
            });
        }
    }
}
