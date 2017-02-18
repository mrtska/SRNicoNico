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
using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows.Input;



namespace SRNicoNico.ViewModels {
    public class NicoRepoNextButtonEntryViewModel : ViewModel {

        public TabItemViewModel Owner { get; set; }
        //ニコレポの一番下のボタン
        public NicoRepoNextButtonEntryViewModel(NicoRepoResultViewModel vm) {

            Owner = vm;
        }
        public NicoRepoNextButtonEntryViewModel(UserNicoRepoViewModel vm) {

            Owner = vm;
        }


    }
}
