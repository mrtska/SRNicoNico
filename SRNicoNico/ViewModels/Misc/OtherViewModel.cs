using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class OtherViewModel : TabItemViewModel {



        #region OtherCollection変更通知プロパティ
        private ObservableCollection<TabItemViewModel> _OtherCollection;

        public ObservableCollection<TabItemViewModel> OtherCollection {
            get { return _OtherCollection; }
            set { 
                if(_OtherCollection == value)
                    return;
                _OtherCollection = value;
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




        public OtherViewModel() : base("その他") {

            OtherCollection = new ObservableCollection<TabItemViewModel>();
            OtherCollection.Add(new OverViewViewModel());
            OtherCollection.Add(new OSSViewModel());

        }




       
    }
}
