using Livet;
using Livet.Messaging.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRNicoNico.ViewModels {
    public class VideoPaymentViewModel : ViewModel {

        private readonly VideoViewModel Owner;

        #region VideoUrl変更通知プロパティ
        private string _VideoUrl;

        public string VideoUrl {
            get { return _VideoUrl; }
            set {
                if (_VideoUrl == value)
                    return;
                _VideoUrl = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public VideoPaymentViewModel(VideoViewModel vm) {

            Owner = vm;
            VideoUrl = vm.VideoUrl;
        }

        public void Refresh() {

            Owner.Refresh();
        }

        public void Close() {

            Messenger.Raise(new WindowActionMessage(WindowAction.Close));
        }
    }
}
