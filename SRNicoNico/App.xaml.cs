using System.Windows;
using SRNicoNico.Models;
using SRNicoNico.Services;
using SRNicoNico.ViewModels;
using SRNicoNico.Views;
using Unity;

namespace SRNicoNico {
    /// <summary>
    /// NicoNicoViewerのエントリポイント
    /// </summary>
    public partial class App : Application {

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            var container = new UnityContainer();
            RegisterServices(container);

            MainWindow = new MainWindow { DataContext = container.Resolve<MainWindowViewModel>() };
            MainWindow.Visibility = Visibility.Visible;
            MainWindow.Activate();
        }

        /// <summary>
        /// 各サービスをDIコンテナに登録する
        /// </summary>
        /// <param name="container">コンテナエンジン</param>
        private void RegisterServices(IUnityContainer container) {

            container.RegisterType<ISettings, Settings>(TypeLifetime.Singleton);
            
            container.RegisterType<INicoNicoViewer, NicoNicoViewer>(TypeLifetime.Singleton);

            container.RegisterType<ISessionService, NicoNicoSessionService>(TypeLifetime.Singleton);
        }
    }
}
