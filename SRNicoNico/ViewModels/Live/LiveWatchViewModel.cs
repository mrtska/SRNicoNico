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
    public class LiveWatchViewModel : TabItemViewModel {

        private NicoNicoLive LiveInstance;

        public LiveFlashHandler Handler;


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


        #region ReservationDialog変更通知プロパティ
        private ConfirmReservation _ReservationDialog;

        public ConfirmReservation ReservationDialog {
            get { return _ReservationDialog; }
            set { 
                if(_ReservationDialog == value)
                    return;
                _ReservationDialog = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        #region DescriptionBrowser変更通知プロパティ
        private WebBrowser _DescriptionBrowser;

        public WebBrowser DescriptionBrowser {
            get { return _DescriptionBrowser; }
            set { 
                if(_DescriptionBrowser == value)
                    return;
                _DescriptionBrowser = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region LiveFlash変更通知プロパティ
        private LiveFlash _LiveFlash;

        public LiveFlash LiveFlash {
            get { return _LiveFlash; }
            set { 
                if(_LiveFlash == value)
                    return;
                _LiveFlash = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region FullScreenLiveFlash変更通知プロパティ
        private LiveFlash _FullScreenLiveFlash;

        public LiveFlash FullScreenLiveFlash {
            get { return _FullScreenLiveFlash; }
            set { 
                if(_FullScreenLiveFlash == value)
                    return;
                _FullScreenLiveFlash = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private LiveViewModel Owner;


        public LiveWatchViewModel(LiveViewModel vm, NicoNicoLive instance, NicoNicoLiveContent content) {

            Owner = vm;
            LiveInstance = instance;
            Content = content;

            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => LiveFlash = new LiveFlash() { DataContext = this }));

            Task.Run(() => Initialize());
        }

        public async void Initialize() {

            await DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => {

                Handler.LoadMovie();
                
            }));
           OpenVideo();
        }
        
        public void OpenVideo() {

            var json = Content.GetPlayerStatus.ToJson();
            Handler.InvokeScript("AsOpenVideo", Content.GetPlayerStatus.RtmpUrl, json);
        }

        public void Refresh() {

            Task.Run(() => Initialize());
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            if(disposing) {

                Handler.DisposeHandler();
                DescriptionBrowser.Dispose();
                App.ViewModelRoot.RemoveTabAndLastSet(this);
            }
        }

        public void Close() {

            Dispose();
        }

        public override void KeyDown(KeyEventArgs e) {

            if(e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.W) {

                Dispose();
            } else if(e.Key == Key.F5) {

                Refresh();
            }
        }


    }
}
