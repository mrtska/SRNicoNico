using Livet;
using System.Windows;

namespace Updater {
    public partial class App : Application {
        private void Application_Startup(object sender, StartupEventArgs e) {
            DispatcherHelper.UIDispatcher = Dispatcher;
        }
    }
}
