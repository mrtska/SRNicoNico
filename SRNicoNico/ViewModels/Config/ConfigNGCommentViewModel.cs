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
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.ViewModels {
    public class ConfigNGCommentViewModel : ConfigViewModelBase {


        #region Hide3DSComment変更通知プロパティ
        public bool Hide3DSComment {
            get { return Settings.Instance.Hide3DSComment; }
            set {
                if(Settings.Instance.Hide3DSComment == value)
                    return;
                Settings.Instance.Hide3DSComment = value;
                RaisePropertyChanged();
                ApplyConfig();
            }
        }
        #endregion


        #region HideWiiUComment変更通知プロパティ
        public bool HideWiiUComment {
            get { return Settings.Instance.HideWiiUComment; }
            set {
                if(Settings.Instance.HideWiiUComment == value)
                    return;
                Settings.Instance.HideWiiUComment = value;
                RaisePropertyChanged();
                ApplyConfig();
            }
        }
        #endregion


        #region NGSharedLevel変更通知プロパティ
        public string NGSharedLevel {
            get { return Settings.Instance.NGSharedLevel; }
            set {
                if(Settings.Instance.NGSharedLevel == value)
                    return;
                Settings.Instance.NGSharedLevel = value;
                RaisePropertyChanged();
                ApplyConfig();
            }
        }
        #endregion


        //開いてる動画があれば設定を反映させる
        public void ApplyConfig() {

            foreach(var tab in App.ViewModelRoot.VideoTabs) {

                if(tab is VideoViewModel) {

                    var vm = (VideoViewModel)tab;
                    vm.Handler.ApplyChanges();
                }
            }
        }


        public ConfigNGCommentViewModel() : base("コメントNG機能") {

        }

        public void AddEmptyNGEntry() {

            var entry = new NGCommentEntry();
            entry.IsEnabled = false;
            entry.Type = NGType.Word;
            entry.Content = "";

            Settings.Instance.NGList.Add(entry);
        }

        public void AddNGEntry(NGType type, string content) {

            var entry = new NGCommentEntry();
            entry.IsEnabled = true;
            entry.Type = type;
            entry.Content = content;

            Settings.Instance.NGList.Add(entry);
        }

        public void DeleteNGEntry(NGCommentEntry target) {

            Settings.Instance.NGList.Remove(target);
        }
    }
}
