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

namespace Installer.ViewModels {
    public class InstallLocationViewModel : ViewModel {


        #region InstallLocation変更通知プロパティ
        private string _InstallLocation = string.Empty;

        public string InstallLocation {
            get { return _InstallLocation; }
            set {
                if(_InstallLocation == value)
                    return;
                _InstallLocation = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public InstallLocationViewModel() {

            Initialize();
        }

        public void Initialize() {

            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            InstallLocation = programFiles + @"\SRNicoNico\";
        }

        public void OpenDirectorySelectionView() {

            using(var fbd = new CommonOpenFileDialog("インストール先を選択してください") { IsFolderPicker = true, Multiselect = false }) {

                fbd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                var result = fbd.ShowDialog();

                if(result == CommonFileDialogResult.Ok) {

                    InstallLocation = fbd.FileName + @"\SRNicoNico";
                }
            }
        }
    }
}
