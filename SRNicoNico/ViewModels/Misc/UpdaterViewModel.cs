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
using SRNicoNico.Models.NicoNicoViewer;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Net;

namespace SRNicoNico.ViewModels {
    public class UpdaterViewModel : ViewModel {

        public string AssemblyDirectory {
            get {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public double CurrentVersion {
            get { return App.ViewModelRoot.CurrentVersion; }
        }

        public async void Update() {

            try {

                var updater = Path.GetTempPath() + @"Updater.exe";

                using(var wc = new WebClient()) {

                    await wc.DownloadFileTaskAsync("https://mrtska.net/download/niconicoviewer/NicoNicoViewerUpdater.exe", updater);
                }

                var process = new Process();
                process.StartInfo.FileName = updater;
                process.StartInfo.Arguments = "iris \"" + AssemblyDirectory + "\" " + Process.GetCurrentProcess().Id;
                process.Start();
                Environment.Exit(0);

            } catch(Exception) {

                Environment.Exit(0);
            }

        }

        public void Cancel() {

            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Updater"));
        }
    }
}
