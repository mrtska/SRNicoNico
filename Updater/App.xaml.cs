using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using Livet;

namespace Updater {
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application {

        public static string DownloadUrl { get; set; }

        public static bool ProductionMode;

        private void Application_Startup(object sender, StartupEventArgs e) {
            DispatcherHelper.UIDispatcher = Dispatcher;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            var lines = Environment.GetCommandLineArgs();

            if(lines.Length == 1) {

                Environment.Exit(-1);
                return;
            }

            var url = lines[1];

            if(url == "prepare") {

                ProductionMode = false;
                DownloadUrl = lines[2];
                return;
            }

            if(url.StartsWith("http://")) {

                ProductionMode = true;
                DownloadUrl = url;
                return;
            }

            Environment.Exit(-1);
        }

        //集約エラーハンドラ
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //TODO:ロギング処理など
            MessageBox.Show(
                "不明なエラーが発生しました。アプリケーションを終了します。",
                "エラー",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        
            Environment.Exit(1);
        }
    }
}
