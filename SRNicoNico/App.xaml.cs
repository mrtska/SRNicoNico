using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using Livet;
using Microsoft.Win32;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.ViewModels;
using SRNicoNico.Views;
using System.Text;

namespace SRNicoNico {
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application {

        //メインウィンドウのViewModel
        public static MainWindowViewModel ViewModelRoot { get; private set; }

#if !DEBUG
        private MultipleLaunchingMonitor Monitor;
#endif

        private void Application_Startup(object sender, StartupEventArgs e) {

            //UIスレッドのディスパッチャを登録しておく
            DispatcherHelper.UIDispatcher = Dispatcher;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

#if !DEBUG
            Monitor = new MultipleLaunchingMonitor();


            //多重起動をしようとしていたら最初に起動したプロセスにコマンドラインなどを送って終了
            if(Monitor.IsMultipleLaunching()) {
            
                Monitor.SendCommandLine();
                Environment.Exit(0);
                return;
            } else {

                Monitor.StartMonitoring();
        }
#endif

            //WebBrowserコントロールのIEバージョンを最新にする 古いとUI崩れるからね インストーラーでもこの処理はされてるけど一応
            //レジストリを弄るのはここだけ
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", "SRNicoNico.exe", 0x00002AFA, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_GPU_RENDERING", "SRNicoNico.exe", 0x00000001, RegistryValueKind.DWord);


            ViewModelRoot = new MainWindowViewModel();
            MainWindow = new MainWindow() { DataContext = ViewModelRoot };

            MainWindow.Show();
        }


        //集約エラーハンドラ
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            //ハンドルされない例外の処理 このメソッドが終わると
            MessageBox.Show(
                "不明なエラーが発生しました。可能であれば、この文章をコピーして作者に報告していただれば幸いです。Ctrl+Cでコピーできます。\n動画再生時に起きた場合は動画IDを添えてください。\n ExceptionObject:" + e.ExceptionObject,
                "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            
        }
    }
}
