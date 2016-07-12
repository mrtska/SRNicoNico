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
using System.Windows;
using Codeplex.Data;
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.ViewModels {
    public class ConfigNGFilterViewModel : ConfigViewModelBase {



        public ConfigNGFilterViewModel() : base("NGフィルター") {

        }

        public void AddEmptyNGEntry() {

            var entry = new NGCommentEntry();
            entry.IsEnabled = false;
            entry.Type = NGType.Word;
            entry.Content = "";

            Settings.Instance.NGList.Add(entry);
        }

        public void AddNGEntry(NGType type, string content) {

            var entry = new NGCommentEntry();
            entry.IsEnabled = true;
            entry.Type = type;
            entry.Content = content;

            Settings.Instance.NGList.Add(entry);
        }

        public void DeleteNGEntry(NGCommentEntry target) {

            Settings.Instance.NGList.Remove(target);
        }
    }
}
