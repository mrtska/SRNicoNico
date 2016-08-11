using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;

using SRNicoNico.ViewModels;
using SRNicoNico.Views;
using SRNicoNico.Models.NicoNicoViewer;

using Livet;
using Microsoft.Win32;
using CefSharp;

namespace SRNicoNico {
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : Application {


		public static MainWindowViewModel ViewModelRoot { get; private set; }


		private void Application_Startup(object sender, StartupEventArgs e) {
            
            
		}

		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);

            DispatcherHelper.UIDispatcher = Dispatcher;
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", "SRNicoNico.exe", 0x00002AF9, Microsoft.Win32.RegistryValueKind.DWord);

            var settings = new CefSettings();

            settings.LogSeverity = LogSeverity.Info;
            settings.AcceptLanguageList = "ja-JP";
            settings.UserAgent = "SRNicoNico/1.0";
            settings.CachePath = "./cache";

            settings.MultiThreadedMessageLoop = false;

            settings.Locale = "ja";

            //サブプロセスを指定
            settings.BrowserSubprocessPath = "./SRNicoNicoRenderingProcess.exe";

            Cef.Initialize(settings, true, true);


            ViewModelRoot = new MainWindowViewModel();
			MainWindow = new MainWindow { DataContext = ViewModelRoot };
			MainWindow.Show();


        }

        //集約エラーハンドラ
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
		    //TODO:ロギング処理など
		    MessageBox.Show(
		        "不明なエラーが発生しました。可能であれば、この文章をコピーして作者に報告していただれば幸いです。Ctrl+Cでコピーできます。\n ExceptionObject:" + e.ExceptionObject,
		        "エラー",
		        MessageBoxButton.OK,
		        MessageBoxImage.Error);

		}
	}
}
