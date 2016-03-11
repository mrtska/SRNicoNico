using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Controls;
using SRNicoNico.Views.Contents.Live;
using AxShockwaveFlashObjects;
using Flash.External;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace SRNicoNico.ViewModels {
    public class LiveViewModel : TabItemViewModel {

        #region Content変更通知プロパティ
        private NicoNicoLiveContent _Content;

        public NicoNicoLiveContent Content {
            get { return _Content; }
            set { 
                if(_Content == value)
                    return;
                _Content = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region ContentViewModel変更通知プロパティ
        private TabItemViewModel _ContentViewModel;

        public TabItemViewModel ContentViewModel {
            get { return _ContentViewModel; }
            set { 
                if(_ContentViewModel == value)
                    return;
                _ContentViewModel = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public string LiveUrl { get; set; }

        public LiveViewModel(string url) : base("読込中") {

            LiveUrl = url;

            Task.Run(() => Initialize());
        }

        public void Initialize() {

            Status = "読込中";
            IsActive = true;

            var LiveInstance = new NicoNicoLive(LiveUrl);

            Content = LiveInstance.GetPage();
            Name = Content.Title;

            if(Content.Type == LivePageType.Gate || Content.Type == LivePageType.TimeShiftGate) {

                ContentViewModel = new LiveGateViewModel(this, LiveInstance, Content);
            } else {

                ContentViewModel = new LiveWatchViewModel(this, LiveInstance, Content);
            }

            IsActive = false;
            Status = "";
        }

        public void Refresh() {

            Task.Run(() => {

                ContentViewModel?.Dispose();
                Initialize();
            });
        }

        public void Close() {

            ContentViewModel?.Dispose();
            Dispose();
        }

        public override void KeyDown(KeyEventArgs e) {

            ContentViewModel?.KeyDown(e);
        }


    }
}
