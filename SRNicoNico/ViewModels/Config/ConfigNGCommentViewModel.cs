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

using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows;
using Codeplex.Data;

namespace SRNicoNico.ViewModels {
    public class ConfigNGCommentViewModel : ConfigViewModelBase {


        #region Hide3DSComment変更通知プロパティ
        public bool Hide3DSComment {
            get { return Properties.Settings.Default.Hide3DSComment; }
            set {
                if(Properties.Settings.Default.Hide3DSComment == value)
                    return;
                Properties.Settings.Default.Hide3DSComment = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
                ApplyConfig();
            }
        }
        #endregion


        #region HideWiiUComment変更通知プロパティ
        public bool HideWiiUComment {
            get { return Properties.Settings.Default.HideWiiUComment; }
            set {
                if(Properties.Settings.Default.HideWiiUComment == value)
                    return;
                Properties.Settings.Default.HideWiiUComment = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
                ApplyConfig();
            }
        }
        #endregion


        #region NGSharedLevel変更通知プロパティ
        public string NGSharedLevel {
            get { return Properties.Settings.Default.NGSharedLevel; }
            set {
                if(Properties.Settings.Default.NGSharedLevel == value)
                    return;
                Properties.Settings.Default.NGSharedLevel = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
                ApplyConfig();
            }
        }
        #endregion


        //開いてる動画があれば設定を反映させる
        public void ApplyConfig() {

            foreach(var tab in App.ViewModelRoot.TabItems) {

                if(tab is VideoViewModel) {

                    var vm = (VideoViewModel)tab;
                    vm.Handler.ApplyChanges();
                }
            }
        }


        #region NGList変更通知プロパティ
        private DispatcherCollection<NGCommentEntry> _NGList = new DispatcherCollection<NGCommentEntry>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<NGCommentEntry> NGList {
            get { return _NGList; }
            set { 
                if(_NGList == value)
                    return;
                _NGList = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public ConfigNGCommentViewModel() : base("コメントNG機能") {

            var entry = new NGCommentEntry();
            entry.IsEnabled = true;
            entry.Type = NGType.RegEx;
            entry.Content = "\\d+";

            NGList.Add(entry);
        }

        public void AddEmptyNGEntry() {

            var entry = new NGCommentEntry();
            entry.IsEnabled = true;
            entry.Type = NGType.Word;
            entry.Content = "";

            NGList.Add(entry);
        }

        public void AddNGEntry(NGType type, string content) {

            var entry = new NGCommentEntry();
            entry.IsEnabled = true;
            entry.Type = type;
            entry.Content = content;

            NGList.Add(entry);
        }

        public override void Reset() {

            RaisePropertyChanged(nameof(Hide3DSComment));
            RaisePropertyChanged(nameof(HideWiiUComment));
            RaisePropertyChanged(nameof(NGSharedLevel));
        }

    }
}
