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
    public class ConfigGeneralViewModel : TabItemViewModel {




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



        public ConfigGeneralViewModel() : base("一般") {

            UserSelectedFont = Properties.Settings.Default.UserSelectedFont;
        }
    }
}
