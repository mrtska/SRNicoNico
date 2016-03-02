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
using SRNicoNico.Models.NicoNicoViewer;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class NotifyLiveViewModel : TabItemViewModel {


        #region NowLiveList変更通知プロパティ
        private DispatcherCollection<NicoNicoFavoriteLiveContent> _NowLiveList = new DispatcherCollection<NicoNicoFavoriteLiveContent>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<NicoNicoFavoriteLiveContent> NowLiveList {
            get { return _NowLiveList; }
            set { 
                if(_NowLiveList == value)
                    return;
                _NowLiveList = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region SelectedLive変更通知プロパティ
        private NicoNicoFavoriteLiveContent _SelectedLive;

        public NicoNicoFavoriteLiveContent SelectedLive {
            get { return _SelectedLive; }
            set { 
                if(_SelectedLive == value)
                    return;
                _SelectedLive = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        private NicoNicoFavoriteLive LiveInstance;



        public NotifyLiveViewModel() : base("生放送通知") {

            LiveInstance = new NicoNicoFavoriteLive();

            Reflesh();

        }

        public void  Reflesh() {

            Task.Run(() => {

                IsActive = true;
                Status = "更新中";
                NowLiveList.Clear();

                var list = LiveInstance.GetLiveInformation();

                if(list != null) {

                    foreach(var entry in list) {

                        NowLiveList.Add(entry);
                    }

                    Badge = list.Count;
                } else {

                    Badge = null;
                }

                IsActive = false;
                Status = "";
            });
        }

        public void Open() {

            if(SelectedLive != null) {

                NicoNicoOpener.Open(SelectedLive.LiveUrl);
                SelectedLive = null;
            }

        }

        public override void KeyDown(KeyEventArgs e) {

            if(e.Key == Key.F5) {

                Reflesh();
            }
        }


        public void Initialize() {
        }
    }
}
