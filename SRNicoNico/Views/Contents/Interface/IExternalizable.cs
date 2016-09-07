using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Livet;


using System.Windows.Controls;
using SRNicoNico.ViewModels;

namespace SRNicoNico.Views.Contents.Interface {
    interface IExternalizable {

        Control View { get; set; }

        TabItemViewModel ViewModel { get; }
    }
}
