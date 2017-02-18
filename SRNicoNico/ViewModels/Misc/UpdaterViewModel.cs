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
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
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

                    await wc.DownloadFileTaskAsync(await UpdateChecker.GetUpdaterBinaryUrl(), updater);
                }

                var process = new Process();
                process.StartInfo.FileName = updater;
                process.StartInfo.Arguments = "iris \"" + AssemblyDirectory + "\"";
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
