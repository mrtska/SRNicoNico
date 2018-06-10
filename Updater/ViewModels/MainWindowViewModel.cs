using Livet;
using System;
using System.Diagnostics;
using System.IO;
using Updater.Models;

namespace Updater.ViewModels {
    public class MainWindowViewModel : ViewModel {

        #region Index変更通知プロパティ
        private int _Index = 0;

        public int Index {
            get { return _Index; }
            set {
                if(_Index == value)
                    return;
                _Index = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region PrevButtonAvailable変更通知プロパティ
        private bool _PrevButtonAvailable = false;

        public bool PrevButtonAvailable {
            get { return _PrevButtonAvailable; }
            set {
                if(_PrevButtonAvailable == value)
                    return;
                _PrevButtonAvailable = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region NextButtonAvailable変更通知プロパティ
        private bool _NextButtonAvailable = true;

        public bool NextButtonAvailable {
            get { return _NextButtonAvailable; }
            set {
                if(_NextButtonAvailable == value)
                    return;
                _NextButtonAvailable = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Status変更通知プロパティ
        private string _Status = string.Empty;

        public string Status {
            get { return _Status; }
            set {
                if (_Status == value)
                    return;
                _Status = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        internal string InstallLocation = "";

        private readonly InstallProcess InstallProcess;

        public MainWindowViewModel() {

            var args = Environment.GetCommandLineArgs();

            if(args.Length == 1 || args[1] != "iris") {

                Environment.Exit(0);
            }
            if(args.Length == 4) {

                int pid = int.Parse(args[3]);
                Process.GetProcessById(pid).WaitForExit();
            }

            InstallLocation = args[2] + @"\";
            InstallProcess = new InstallProcess(this);
        }

        public void EndOfLife() {

            Environment.Exit(0);
        }

        public async void Initialize() {

            await InstallProcess.InstallAsync();
            Next();
            Status = "アップデート完了\n手動で起動してください";
        }

        public void Prev() {

            Index--;
        }

        public void Next() {

            Index++;
            if(Index == 2) {

                EndOfLife();
            }
        }
    }
}
