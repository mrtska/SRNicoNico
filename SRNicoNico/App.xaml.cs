using System.Windows;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;
using SRNicoNico.Models;
using SRNicoNico.Services;
using SRNicoNico.ViewModels;
using Unity;

namespace SRNicoNico {
    /// <summary>
    /// NicoNicoViewerのエントリポイント
    /// </summary>
    public partial class App : Application {

        public static Dispatcher? UIDispatcher;

        public static IUnityContainer? UnityContainer { get; private set; }

        protected override void OnStartup(StartupEventArgs e) {

            UIDispatcher = Dispatcher;

            var container = new UnityContainer();
#if DEBUG
            container.AddExtension(new Diagnostic()); // デバッグ起動時は診断拡張を追加しておく
#endif
            RegisterServices(container);

            ConfigureDbContext(container);

            base.OnStartup(e);

            UnityContainer = container;

            ApplySettings(container.Resolve<ISettings>());

            MainWindow = new Views.MainWindow { DataContext = container.Resolve<MainWindowViewModel>() };
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

            container.RegisterType<MainWindowViewModel>(TypeLifetime.Singleton);

            container.RegisterType<ViewerDbContext>(TypeLifetime.Singleton);

            container.RegisterType<ISettings, Settings>(TypeLifetime.Singleton);
            container.RegisterType<INicoNicoViewer, NicoNicoViewer>(TypeLifetime.Singleton);
            container.RegisterType<IAccountService, NicoNicoViewerAccountService>(TypeLifetime.Singleton);

            container.RegisterType<ISessionService, NicoNicoSessionService>(TypeLifetime.Singleton);
            container.RegisterType<IUserService, NicoNicoUserService>(TypeLifetime.Singleton);
            container.RegisterType<INicoRepoService, NicoNicoNicoRepoService>(TypeLifetime.Singleton);
            container.RegisterType<IMylistService, NicoNicoMylistService>(TypeLifetime.Singleton);
            container.RegisterType<ISeriesService, NicoNicoSeriesService>(TypeLifetime.Singleton);
            container.RegisterType<IRankingService, NicoNicoRankingService>(TypeLifetime.Singleton);
            container.RegisterType<IHistoryService, NicoNicoHistoryService>(TypeLifetime.Singleton);

            container.RegisterType<ISearchService, NicoNicoSearchService>(TypeLifetime.Singleton);
            container.RegisterType<IVideoService, NicoNicoVideoService>(TypeLifetime.Singleton);
        }

        /// <summary>
        /// 設定をアプリケーション全体に反映させる
        /// </summary>
        /// <param name="settings"></param>
        private void ApplySettings(ISettings settings) {

            // アクセントを変更
            settings.ChangeAccent();
            settings.ChangeFontFamily();
            settings.ChangeMutedAccount();
        }
    }
}
