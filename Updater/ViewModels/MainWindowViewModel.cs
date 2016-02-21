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

            Status = "前バージョンを削除中";
            Progress = 60;
            var files = Directory.GetFiles(Environment.CurrentDirectory);

            foreach(var path in files) {

                var file = new FileInfo(path).Name;


                if(File.Exists(path) && file != "session") {

                    File.Delete(path);
                }
            }
            Directory.Delete("Flash", true);

            Status = "新バージョンを解凍中";
            Progress = 80;

            ZipFile.ExtractToDirectory("tmp/data", ".");

            Status = "テンポラリフォルダを削除中";
            Directory.Delete("tmp", true);

            Progress = 100;

            Status = "新バージョンを起動中";

            //ステータスを少しだけ読ましてあげる優しさ
            Thread.Sleep(200);

            Process.Start(Environment.CurrentDirectory +  "/SRNicoNico.exe");
            Environment.Exit(0);
        }

        public void Initialize() {

            Update();
        }


    }
}
