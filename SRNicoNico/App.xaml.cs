using System.Windows;
using SRNicoNico.Services;
using SRNicoNico.ViewModels;
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

            MainWindow = new MainWindow() { DataContext = container.Resolve<MainWindowViewModel>() };
            MainWindow.Show();
        }

        /// <summary>
        /// 各サービスをDIコンテナに登録する
        /// </summary>
        /// <param name="container">コンテナエンジン</param>
        private void RegisterServices(IUnityContainer container) {

            container.RegisterType<NicoNicoSessionService>(TypeLifetime.Singleton);
        }
    }
}
