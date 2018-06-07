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
using SRNicoNico.Models.NicoNicoViewer;
using Microsoft.Win32;

namespace SRNicoNico.ViewModels {

    public class DeployViewModel : TabItemViewModel {

        public DeployModel Model { get; set; }

        public DeployViewModel() : base("デプロイ") {
#if DEBUG
            Model = new DeployModel();
#endif
        }

        public void Initialize() {


        }

        public void FileSelect() {

            var dialog = new OpenFileDialog();
            dialog.ShowDialog();

            Model.FilePath = dialog.FileName;
        }

        public async void Upload() {

            Status = "アップロード中";
            Status = await Model.UploadAsync();

        }
    }
}
