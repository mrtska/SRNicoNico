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

using SRNicoNico.Models;
using System.Windows.Media;

namespace SRNicoNico.ViewModels {
    public class ConfigGeneralViewModel : ConfigViewModelBase {




        #region UserSelecetedFont変更通知プロパティ

        public FontFamily UserSelectedFont {
            get { return Properties.Settings.Default.UserSelectedFont; }
            set { 
                if(Properties.Settings.Default.UserSelectedFont == value)
                    return;
                Properties.Settings.Default.UserSelectedFont = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
                App.ViewModelRoot.ApplyFont();
            }
        }
        #endregion


        #region EnableTwitterLink変更通知プロパティ

        public bool EnableTwitterLink {
            get { return Properties.Settings.Default.EnableTwitterLink; }
            set {
                if(Properties.Settings.Default.EnableTwitterLink == value)
                    return;
                Properties.Settings.Default.EnableTwitterLink = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }
        #endregion


        #region EnableUrlLink変更通知プロパティ

        public bool EnableUrlLink {
            get { return Properties.Settings.Default.EnableUrlLink; }
            set {
                if(Properties.Settings.Default.EnableUrlLink == value)
                    return;
                Properties.Settings.Default.EnableUrlLink = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }
        #endregion

        private ConfigViewModel Owner;

        public ConfigGeneralViewModel(ConfigViewModel vm) : base("一般") {

            Owner = vm;
        }

        public void OpenResetConfig() {

            App.ViewModelRoot.Messenger.Raise(new TransitionMessage(typeof(Views.Contents.Config.ConfigResetDialog), this, TransitionMode.Modal));
        }

        public override void Reset() {

            Properties.Settings.Default.Reset();
            RaisePropertyChanged(nameof(EnableUrlLink));
            RaisePropertyChanged(nameof(EnableTwitterLink));
            RaisePropertyChanged(nameof(UserSelectedFont));
            App.ViewModelRoot.ApplyFont();
            Close();

            Owner.Reset();
        }
        public void Close() {

            Messenger.Raise(new WindowActionMessage(WindowAction.Close));
        }


    }
}
