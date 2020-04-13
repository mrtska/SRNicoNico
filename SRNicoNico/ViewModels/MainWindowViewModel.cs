using System;
using System.Collections.Generic;
using System.Text;
using Livet;
using Unity;

namespace SRNicoNico.ViewModels {
    public class MainWindowViewModel : ViewModel {

        /// <summary>
        /// 現在のバージョン
        /// </summary>
        public double CurrentVersion {

            get { return 2.00; }
        }

        public MainWindowViewModel(IUnityContainer container) {

        }


    }
}
