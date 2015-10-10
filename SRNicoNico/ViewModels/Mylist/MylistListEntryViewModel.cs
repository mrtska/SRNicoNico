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

namespace SRNicoNico.ViewModels {
    public class MylistListEntryViewModel : ViewModel {



        public NicoNicoMylistData Entry { get; set; }


        public MylistListViewModel Owner { get; set; }


        public MylistListEntryViewModel(MylistListViewModel vm, NicoNicoMylistData data) {

            Owner = vm;
            Entry = data;
        }



    }
}
