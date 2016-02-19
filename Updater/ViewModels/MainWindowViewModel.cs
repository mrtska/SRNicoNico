using System;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using System.Net;

namespace Updater.ViewModels {
    public class MainWindowViewModel : ViewModel {
        

        #region Status変更通知プロパティ
        private string _Status;

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


        #region Progress変更通知プロパティ
        private int _Progress;

        public int Progress {
            get { return _Progress; }
            set { 
                if(_Progress == value)
                    return;
                _Progress = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        public void Update() {

            Task.Run(() => {

                if(App.ProductionMode) {

                    Status = "ダウンロード中・・・";

                    Directory.CreateDirectory("./tmp");

                    var client = new WebClient();

                    client.DownloadFile(App.DownloadUrl, "./tmp/data");

                    Progress = 10;
                    var files = Directory.GetFiles(Environment.CurrentDirectory);

                    Status = "削除中・・・";
                    foreach(var path in files) {

                        var file = new FileInfo(path).Name;

                        if(File.Exists(path) && file != "session") {

                            File.Delete(path);
                        }
                        
                    }
                    Directory.Delete("Flash", true);
                    Progress = 40;

                    Status = Environment.CurrentDirectory;
                    ZipFile.ExtractToDirectory("tmp/data", ".");

                    GC.Collect();
                    Progress = 80;

                    Status = "テンポラリを削除中・・・";

                    File.Delete("./tmp/data");
                    Directory.Delete("./tmp");
                    Progress = 100;
                    Process.Start("SRNicoNico.exe");

                    Environment.Exit(0);
                } else {

                    Status = "バックアップ中";
                    Directory.CreateDirectory("./backup");
                        
                    var files = Directory.GetFiles(Environment.CurrentDirectory);
                    var offset = 100 / files.Length;

                    foreach(var path in files) {

                        var file = new FileInfo(path).Name;
                        File.Copy(file, "backup/" + file, true);

                        Progress += offset;
                    }
                    Directory.CreateDirectory("./backup/Flash");

                    var flash = Directory.GetFiles(Environment.CurrentDirectory + "/Flash");
                    foreach(var path in flash) {

                        var file = new FileInfo(path).Name;
                        File.Copy("./Flash/" + file, "./backup/Flash/" + file, true);

                    }
                    Progress = 100;

                    Process.Start(Environment.CurrentDirectory + "/backup/Updater.exe", Process.GetCurrentProcess().Id + " " + App.DownloadUrl);
                    Environment.Exit(0);
                }


            });

        }

        public void Initialize() {

            Update();
        }


    }
}
