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


			ViewModelRoot = new MainWindowViewModel();
			this.MainWindow = new MainWindow { DataContext = ViewModelRoot };
			this.MainWindow.Show();


        }

        //集約エラーハンドラ
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
		    //TODO:ロギング処理など
		    MessageBox.Show(
		        "不明なエラーが発生しました。ExceptionObject:" + e.ExceptionObject,
		        "エラー",
		        MessageBoxButton.OK,
		        MessageBoxImage.Error);

			this.Shutdown(-1);
		}
	}
}
