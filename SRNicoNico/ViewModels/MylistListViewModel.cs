using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoWrapper;
using System.Collections.ObjectModel;

namespace SRNicoNico.ViewModels {
    public class MylistListViewModel : TabItemViewModel {





        #region Mylist変更通知プロパティ
        private ObservableCollection<NicoNicoMylistData> _Mylist;

        public ObservableCollection<NicoNicoMylistData> Mylist {
            get { return _Mylist; }
            set { 
                if(_Mylist == value)
                    return;
                _Mylist = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region SelectedItem変更通知プロパティ
        private NicoNicoMylistData _SelectedItem;

        public NicoNicoMylistData SelectedItem {
            get { return _SelectedItem; }
            set {
                if(_SelectedItem == value)
                    return;
                _SelectedItem = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public MylistListViewModel(string name, List<NicoNicoMylistData> list) : base(name) {

            Mylist = new ObservableCollection<NicoNicoMylistData>(list);
        }

        public void Open() {

            if(SelectedItem != null) {

                new VideoViewModel("http://www.nicovideo.jp/watch/" + SelectedItem.Id);
                SelectedItem = null;
            }
        }

    }
}
