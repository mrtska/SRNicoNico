using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using IWshRuntimeLibrary;
using System.Diagnostics;

namespace Installer.ViewModels {
    public class InstallFinishViewModel : ViewModel {



        #region RegisterStartMenu変更通知プロパティ
        private bool _RegisterStartMenu = true;

        public bool RegisterStartMenu {
            get { return _RegisterStartMenu; }
            set { 
                if(_RegisterStartMenu == value)
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
                if(_CreateDesktopShortcut == value)
                    return;
                _CreateDesktopShortcut = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        private MainWindowViewModel Owner;

        public InstallFinishViewModel(MainWindowViewModel vm) {

            Owner = vm;
        }

        public void EndOfLife() {

            var shell = new WshShell();
            var installPath = Owner.InstallLocationViewModel.InstallLocation + "SRNicoNico.exe";

            if(RegisterStartMenu) {

                var path = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);

                var shortcut = (IWshShortcut)shell.CreateShortcut(path + @"\Programs\NicoNicoViewer.lnk");

                shortcut.Description = "NicoNicoViewer";
                shortcut.TargetPath = installPath;
                shortcut.Save();
            }

            if(CreateDesktopShortcut) {

                var path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                var shortcut = (IWshShortcut)shell.CreateShortcut(path + @"\NicoNicoViewer.lnk");

                shortcut.Description = "NicoNicoViewer";
                shortcut.TargetPath = installPath;
                shortcut.Save();
            }

            Environment.Exit(0);
        }


    }
}
