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
using System.Windows;
using Codeplex.Data;

namespace SRNicoNico.ViewModels {
    public class ConfigNGCommentViewModel : ConfigViewModelBase {


        public ConfigNGCommentViewModel() : base("NGコメント") {

        }

        public override void Reset() {

        }
    }
}
