using Installer.Models;
using IWshRuntimeLibrary;
using Livet;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;

namespace Installer.ViewModels {
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

        #region InstallLocation変更通知プロパティ
        private string _InstallLocation = string.Empty;

        public string InstallLocation {
            get { return _InstallLocation; }
            set {
                if (_InstallLocation == value)
                    return;
                _InstallLocation = value;
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

        #region RegisterStartMenu変更通知プロパティ
        private bool _RegisterStartMenu = true;

        public bool RegisterStartMenu {
            get { return _RegisterStartMenu; }
            set {
                if (_RegisterStartMenu == value)
                    return;
                _RegisterStartMenu = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region CreateDesktopShortcut変更通知プロパティ
        private bool _CreateDesktopShortcut = true;

        public bool CreateDesktopShortcut {
            get { return _CreateDesktopShortcut; }
            set {
                if (_CreateDesktopShortcut == value)
                    return;
                _CreateDesktopShortcut = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private readonly InstallProcess InstallProccess;

        public MainWindowViewModel() {

            InstallProccess = new InstallProcess(this);
        }

        public void Initialize() {

            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            InstallLocation = Path.Combine(programFiles, "SRNicoNico");
        }

        public async void Install() {

            NextButtonAvailable = false;
            PrevButtonAvailable = false;

            await InstallProccess.InstallAsync();

            NextButtonAvailable = true;
            Next();
        }


        public void EndOfLife() {

            var shell = new WshShell();
            var installPath = Path.Combine(InstallLocation, "SRNicoNico.exe");

            if (RegisterStartMenu) {

                var path = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);

                var shortcut = (IWshShortcut)shell.CreateShortcut(Path.Combine(path, @"Programs\NicoNicoViewer.lnk"));

                shortcut.Description = "NicoNicoViewer";
                shortcut.TargetPath = installPath;
                shortcut.Save();
            }

            if (CreateDesktopShortcut) {

                var path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                var shortcut = (IWshShortcut)shell.CreateShortcut(Path.Combine(path, @"NicoNicoViewer.lnk"));

                shortcut.Description = "NicoNicoViewer";
                shortcut.TargetPath = installPath;
                shortcut.Save();
            }
            Environment.Exit(0);
        }

        public void OpenDirectorySelectionView() {

            using (var fbd = new CommonOpenFileDialog("インストール先を選択してください") { IsFolderPicker = true, Multiselect = false }) {

                fbd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                var result = fbd.ShowDialog();

                if (result == CommonFileDialogResult.Ok) {

                    InstallLocation = Path.Combine(fbd.FileName, "SRNicoNico");
                }
            }
        }

        public void Prev() {

            Index--;
        }

        public void Next() {

            //強制的に終わり
            if(Index == 3) {

                EndOfLife();
                return;
            }
            Index++;
        }
    }
}
