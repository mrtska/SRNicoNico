using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Threading.Tasks;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models.NicoNicoViewer;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace SRNicoNico.ViewModels {
    public class DownloadViewModel : TabItemViewModel {


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

        private string Url;

        public DownloadViewModel(string downloadUrl) {

            Url = downloadUrl;
        }


        public void Update() {

            Task.Run(() => {

                Status = "準備中";
                Directory.CreateDirectory("./tmp");

                
                var wc = new WebClient();

                Status = "ダウンロード中";
                wc.DownloadFile(Url, "./tmp/data");
                wc.Dispose();

                Progress = 20;
                Status = "バックアップ中";

                Directory.CreateDirectory("./backup");
                var files = Directory.GetFiles("./");

                foreach(var path in files) {

                    var file = new FileInfo(path).Name;
                    File.Copy(file, "backup/" + file, true);
                }

                Directory.CreateDirectory("./backup/Flash");

                var flash = Directory.GetFiles(Environment.CurrentDirectory + "/Flash");
                foreach(var path in flash) {

                    var file = new FileInfo(path).Name;
                    File.Copy("./Flash/" + file, "./backup/Flash/" + file, true);

                }

                Progress = 40;

                //プログレスバーを少しだけ見せてあげる優しさ
                Thread.Sleep(200);

                //直接起動されたら困っちゃうのでコマンドライン引数にいれちゃう
                Process.Start(Environment.CurrentDirectory + "/backup/Updater.exe", "iris");
                Environment.Exit(0);
            });
        }
    }
}
