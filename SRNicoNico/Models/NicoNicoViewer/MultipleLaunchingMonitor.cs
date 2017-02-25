using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using Livet;
using System.Windows;
using System.Text.RegularExpressions;
using System.Runtime.Remoting.Lifetime;

namespace SRNicoNico.Models.NicoNicoViewer {
    public class MultipleLaunchingMonitor : IDisposable {

        private IpcServerChannel Server;

        private static readonly Mutex MutexInstance = new Mutex(false, "NicoNicoViewer");

        //多重起動かどうかMutexを使って確認
        public bool IsMultipleLaunching() {

            try {

                return !MutexInstance.WaitOne(0, false);
                
            } catch(AbandonedMutexException) {

                return false;
            }
        }

        //最初に起動したインスタンスがIPCで複数起動したインスタンスからいろいろを取得する
        public void StartMonitoring() {

            LifetimeServices.LeaseTime = TimeSpan.Zero;
            LifetimeServices.RenewOnCallTime = TimeSpan.Zero;

            Server = new IpcServerChannel("NicoNicoViewer");

            ChannelServices.RegisterChannel(Server, true);

            var shared = new IpcRemoteobject();
            shared.CommandLineRecieved += (o, e) => {

                DispatcherHelper.UIDispatcher.BeginInvoke((Action) (() => {
                    App.Current.MainWindow.Activate();
                }));

                var commandline = Convert.ToString(o);

                NicoNicoOpener.TryOpen(commandline);
            };

            RemotingServices.Marshal(shared, "command", typeof(IpcRemoteobject));
        }

        public void SendCommandLine() {

            var client = new IpcClientChannel();

            ChannelServices.RegisterChannel(client, true);

            var shared = Activator.GetObject(typeof(IpcRemoteobject), "ipc://NicoNicoViewer/command") as IpcRemoteobject;

            var args = Environment.GetCommandLineArgs();

            if(args.Length == 2) {

                shared.CommandLine = Environment.GetCommandLineArgs()[1];
            } else {

                shared.CommandLine = "";
            }
        }


        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing) {
            if(!disposedValue) {
                if(disposing) {
                    // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                    MutexInstance.Dispose();
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~MultipleLaunchingMonitor() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        void IDisposable.Dispose() {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    //
    public class IpcRemoteobject : MarshalByRefObject {

        #region CommandLine変更通知プロパティ
        private string _CommandLine;

        public string CommandLine {
            get { return _CommandLine;
            }
            set { 
                if(_CommandLine == value)
                    return;
                _CommandLine = value;
                CommandLineRecieved(value, new RoutedEventArgs());
            }
        }
        #endregion

        public event RoutedEventHandler CommandLineRecieved;

    }

}
