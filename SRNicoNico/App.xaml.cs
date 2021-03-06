using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;
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

        public static Dispatcher? UIDispatcher;

        protected override void OnStartup(StartupEventArgs e) {

            UIDispatcher = Dispatcher;

            var container = new UnityContainer();
#if DEBUG
            container.AddExtension(new Diagnostic()); // デバッグ起動時は診断拡張を追加しておく
#endif
            RegisterServices(container);

            ConfigureDbContext(container);

            base.OnStartup(e);

            MainWindow = new MainWindow { DataContext = container.Resolve<MainWindowViewModel>() };
            MainWindow.Visibility = Visibility.Visible;
            MainWindow.Activate();
        }

        /// <summary>
        /// DbContextを初期化する
        /// </summary>
        /// <param name="container">DIコンテナ</param>
        private void ConfigureDbContext(IUnityContainer container) {

            var dbContext = container.Resolve<ViewerDbContext>();
            dbContext.Database.Migrate();
        }

        /// <summary>
        /// 各サービスをDIコンテナに登録する
        /// </summary>
        /// <param name="container">DIコンテナ</param>
        private void RegisterServices(IUnityContainer container) {

            container.RegisterType<ViewerDbContext>(TypeLifetime.Singleton);

            container.RegisterType<ISettings, Settings>(TypeLifetime.Singleton);
            container.RegisterType<INicoNicoViewer, NicoNicoViewer>(TypeLifetime.Singleton);

            container.RegisterType<ISessionService, NicoNicoSessionService>(TypeLifetime.Singleton);
            container.RegisterType<IUserService, NicoNicoUserService>(TypeLifetime.Singleton);
            container.RegisterType<IHistoryService, NicoNicoHistoryService>(TypeLifetime.Singleton);
        }
    }
}
