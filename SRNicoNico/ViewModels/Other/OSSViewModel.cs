using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;


namespace SRNicoNico.ViewModels {
    public class OSSViewModel : TabItemViewModel {



        #region OSSList変更通知プロパティ
        private DispatcherCollection<TabItemViewModel> _OSSList = new DispatcherCollection<TabItemViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<TabItemViewModel> OSSList {
            get { return _OSSList; }
            set {
                if(_OSSList == value)
                    return;
                _OSSList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public OSSViewModel() : base("オープンソースソフトウェア") {

            OSSList.Add(new OSSEntryViewModel("MetroRadiance"));
            OSSList.Add(new OSSEntryViewModel("MetroTrilithon"));
            OSSList.Add(new OSSEntryViewModel("StatefulModel"));
            OSSList.Add(new OSSEntryViewModel("ModernUI"));
            OSSList.Add(new OSSEntryViewModel("Livet"));
            OSSList.Add(new OSSEntryViewModel("Newtonsoft.Json"));
            OSSList.Add(new OSSEntryViewModel("DynamicJson"));
            OSSList.Add(new OSSEntryViewModel("Fizzler"));
            OSSList.Add(new OSSEntryViewModel("HtmlAgilityPack"));
            OSSList.Add(new OSSEntryViewModel("gong-wpf-dragdrop"));
            OSSList.Add(new OSSEntryViewModel("ForcibleLoader"));
        }



    }
}
