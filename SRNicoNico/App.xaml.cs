using System.Windows;
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

            var vm = container.Resolve<MainWindowViewModel>();
            vm.Initialize(container);

            MainWindow = new MainWindow { DataContext = vm };
            MainWindow.Visibility = Visibility.Visible;
            MainWindow.Activate();
        }

        /// <summary>
        /// 各サービスをDIコンテナに登録する
        /// </summary>
        /// <param name="container">コンテナエンジン</param>
        private void RegisterServices(IUnityContainer container) {

            container.RegisterType<MainWindowViewModel>(TypeLifetime.Singleton);

            container.RegisterType<NicoNicoSessionService>(TypeLifetime.Singleton);
        }
    }
}
