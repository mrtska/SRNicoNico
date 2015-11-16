using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class VideoDataViewModel : ViewModel {

        #region Data変更通知プロパティ
        private NicoNicoVitaApiVideoData _Data;

        public NicoNicoVitaApiVideoData Data {
            get { return _Data; }
            set { 
                if(_Data == value)
                    return;
                _Data = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region IsActive変更通知プロパティ
        private bool _IsActive;

        public bool IsActive {
            get { return _IsActive; }
            set { 
                if(_IsActive == value)
                    return;
                _IsActive = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public  VideoDataViewModel(string cmsid) {

            Task.Run(() => {

                IsActive = true;

                Data = NicoNicoVitaApi.GetVideoData(cmsid);
                IsActive = false;
            });
        }

    }
}
