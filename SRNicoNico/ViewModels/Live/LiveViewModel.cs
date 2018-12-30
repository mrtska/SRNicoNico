using SRNicoNico.Models.NicoNicoWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRNicoNico.ViewModels {
    public class LiveViewModel : TabItemViewModel {


        public NicoNicoLive Model { get; set; }

        #region Html5Handler変更通知プロパティ
        private LiveHtml5Handler _Html5Handler;

        public LiveHtml5Handler Html5Handler {
            get { return _Html5Handler; }
            set {
                if (_Html5Handler == value)
                    return;
                _Html5Handler = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public LiveViewModel(string url) : base("生放送") {

            Model = new NicoNicoLive(url);
            Html5Handler = new LiveHtml5Handler();
        }

        public async void Initialize() {

            Status = await Model.GetLiveData();

            if(Model.ApiData == null) {


            } else {

                Html5Handler.Initialize(this, Model.ApiData);
            }

        }


        public void Refresh() {


        }

        public void Close() {



        }

    }
}
