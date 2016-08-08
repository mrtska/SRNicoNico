using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using Livet;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace Updater {
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application {

        private void Application_Startup(object sender, StartupEventArgs e) {

            if(!Environment.CommandLine.Contains("iris")) {

                Environment.Exit(0);
            }



            DispatcherHelper.UIDispatcher = Dispatcher;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            var pid = int.Parse(Regex.Match(Environment.CommandLine, @"\d+$").Value);

            var process = Process.GetProcessById(pid);
            if(process != null) {

                process.Kill();
            }



        }

        //集約エラーハンドラ
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //TODO:ロギング処理など
            MessageBox.Show(
                "不明なエラーが発生しました。アプリケーションを終了します。" + e.ExceptionObject,
                "エラー",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        
            Environment.Exit(1);
        }
    }
}
