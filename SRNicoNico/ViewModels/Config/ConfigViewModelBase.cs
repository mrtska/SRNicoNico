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

using SRNicoNico.Models;

namespace SRNicoNico.ViewModels {

    //コンフィグのベースViewModel
    public abstract class ConfigViewModelBase : TabItemViewModel {


        public ConfigViewModelBase(string title) : base(title) {

        }

    }
}
