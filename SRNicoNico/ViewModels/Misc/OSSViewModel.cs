using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models;

namespace SRNicoNico.ViewModels {
    public class OSSViewModel : TabItemViewModel {


        #region OtherCollection変更通知プロパティ
        private ObservableCollection<TabItemViewModel> _OSSCollection = new ObservableCollection<TabItemViewModel>();

        public ObservableCollection<TabItemViewModel> OSSCollection {
            get { return _OSSCollection; }
            set {
                if(_OSSCollection == value)
                    return;
                _OSSCollection = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region SelectedItem変更通知プロパティ
        private TabItemViewModel _SelectedItem;

        public TabItemViewModel SelectedItem {
            get { return _SelectedItem; }
            set {
                if(_SelectedItem == value)
                    return;
                _SelectedItem = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public OSSViewModel() : base("オープンソースライセンス") {

            OSSCollection.Add(new OSSEntryViewModel("MetroRadiance"));
            OSSCollection.Add(new OSSEntryViewModel("ModernUI"));
            OSSCollection.Add(new OSSEntryViewModel("Livet"));
            OSSCollection.Add(new OSSEntryViewModel("Newtonsoft.Json"));
            OSSCollection.Add(new OSSEntryViewModel("DynamicJson"));
            OSSCollection.Add(new OSSEntryViewModel("Fizzler"));
            OSSCollection.Add(new OSSEntryViewModel("HtmlAgilityPack"));
            OSSCollection.Add(new OSSEntryViewModel("ForcibleLoader"));


        }

    }
}
