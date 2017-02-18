using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using System.IO;
using Microsoft.Win32;
using System.Net;
using Updater.Models;
using System.Threading;
using System.Diagnostics;
using System.Windows;

namespace Updater.ViewModels {
    public class ProgressViewModel : ViewModel {



        #region Status変更通知プロパティ
        private string _Status = string.Empty;
        
        public string Status {
            get { return _Status; }
            set { 
                if(_Status == value)
                    return;
                _Status = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private MainWindowViewModel Owner;

        public ProgressViewModel(MainWindowViewModel vm) {

            Owner = vm;
        }

        public async void Initialize() {

            Owner.NextButtonAvailable = false;
            Owner.PrevButtonAvailable = false;

            Status = "レジストリ登録中";
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", "SRNicoNico.exe", 0x00002AF8, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_GPU_RENDERING", "SRNicoNico.exe", 0x00000001, RegistryValueKind.DWord);

            var location = Owner.InstallLocation + @"\";

            var settings = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SRNicoNico\";


            if(Directory.Exists(location)) {

                Status = "元ファイルバックアップ中";
                if(File.Exists(settings + @"backup.zip")) {

                    Status = "古いバックアップファイルを削除中";
                    File.Delete(settings + @"backup.zip");
                }

                ZipFile.CreateFromDirectory(location, settings + @"backup.zip", CompressionLevel.Optimal, true);

                retry:
                try {
                    var files = Directory.GetFiles(location);

                    foreach(var file in files) {

                        File.Delete(file);
                    }

                    var directories = Directory.GetDirectories(location);
                    foreach(var directory in directories) {

                        Directory.Delete(directory, true);
                    }

                    bool exists = true;
                    do {
                        // 
                        Thread.Sleep(1);

                        if(Directory.GetFiles(location).Length == 0 && Directory.GetDirectories(location).Length == 0) {

                            exists = false;
                        }
                    } while(exists);
                } catch(Exception) {

                    Thread.Sleep(1);
                    goto retry;
                }
            }

            //もとからファイルがあるとディレクトリもろとも削除するからもう一回
            Status = "ディレクトリ作成中";
            Directory.CreateDirectory(location);
            Directory.CreateDirectory(location + @"tmp");
            Directory.CreateDirectory(settings);

            using(var wc = new WebClient()) {

                Status = "本体ダウンロード中";
                await wc.DownloadFileTaskAsync(await UpdateChecker.GetLatestBinaryUrl(wc), location + @"tmp\data");
            }

            Status = "解凍中";
            ZipFile.ExtractToDirectory(location + @"tmp\data", location);

            Status = "OKOK";

            Owner.NextButtonAvailable = true;
            EndOfLife();
        }

        private void EndOfLife() {

            if(File.Exists(Owner.InstallLocation + @"\SRNicoNico.exe")) {

                Process.Start(Owner.InstallLocation + @"\SRNicoNico.exe");
            }
            Environment.Exit(0);
        }
    }
}
