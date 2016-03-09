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

namespace SRNicoNico.ViewModels {
    public class LiveViewModel : TabItemViewModel {

        private NicoNicoLive LiveInstance;


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


        public string LiveUrl { get; set; }

        public LiveViewModel(string url) : base("読込中") {

            LiveUrl = url;

            Task.Run(() => Initialize());
        }

        public void Initialize() {

            Status = "読込中";
            IsActive = true;
            LiveInstance = new NicoNicoLive(LiveUrl);


            Content = LiveInstance.GetPage();
            Name = Content.Title;

            
            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => DescriptionBrowser.NavigateToString(Content.Description)));
            ;

            IsActive = false;
            Status = "";

        }

        public void ShowReservationDialog() {

            ReservationDialog = new ConfirmReservation();
            ReservationDialog.IsActive = true;
            Task.Run(() => {

                LiveInstance.ConfirmReservation(Content.Id, ReservationDialog);
                ReservationDialog.IsActive = false;

                App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.Contents.Live.ReservationDialog), this, TransitionMode.Modal));

            });

        }


        public void MakeReservation() {

            Messenger.Raise(new WindowActionMessage(WindowAction.Close));
            Task.Run(() => {

                LiveInstance.MakeReservation(ReservationDialog);
                Refresh();
            });
        }


        public void Refresh() {

            Task.Run(() => Initialize());
        }

        public void DisposeViewModel() {

            DescriptionBrowser.Dispose();
            Dispose();
            App.ViewModelRoot.RemoveTabAndLastSet(this);
        }

        public void Close() {

            DisposeViewModel();
        }

        public override void KeyDown(KeyEventArgs e) {

            if(e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.W) {

                DisposeViewModel();
            } else if(e.Key == Key.F5) {

                Refresh();
            }
        }


    }
}
