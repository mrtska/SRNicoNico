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

            var settings = new CefSettings();
            settings.LogSeverity = LogSeverity.Verbose;
            settings.AcceptLanguageList = "ja-JP";
            settings.UserAgent = "SRNicoNico/1.0";

            //Flashプラグインを指定
            settings.CefCommandLineArgs.Add("ppapi-flash-path", "./Flash/pepflashplayer32_21_0_0_242.dll");

            //サブプロセスを指定
            settings.BrowserSubprocessPath = "./SRNicoNicoRenderingProcess.exe";

            //ローカルファイルにアクセスするために使うスキーム
            var bridge = new CefCustomScheme();
            bridge.IsLocal = false;
            bridge.SchemeName = "http";
            bridge.DomainName = "localbridge";
            bridge.SchemeHandlerFactory = new LocalBridgeSchemeHandler();
            settings.RegisterScheme(bridge);

            //日本語に
            settings.Locale = "ja";
            
            Cef.Initialize(settings, true, true);

            DispatcherHelper.UIDispatcher = Dispatcher;
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", "SRNicoNico.exe", 0x00002AF9, RegistryValueKind.DWord);

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
