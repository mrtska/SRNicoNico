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
using System.Windows;
using Codeplex.Data;

namespace SRNicoNico.ViewModels {
    public class ConfigCommentViewModel : TabItemViewModel {


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

        #region CommentAlpha変更通知プロパティ
        public float CommentAlpha {
            get { return Properties.Settings.Default.CommentAlpha; }
            set { 
                if(Properties.Settings.Default.CommentAlpha == value)
                    return;
                Properties.Settings.Default.CommentAlpha = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
                ApplyConfig();
            }
        }
        #endregion


        #region DefaultCommentSize変更通知プロパティ
        public string DefaultCommentSize {
            get { return Properties.Settings.Default.CommentSize; }
            set { 
                if(Properties.Settings.Default.CommentSize == value)
                    return;
                Properties.Settings.Default.CommentSize = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
                ApplyConfig();
            }
        }
        #endregion

        public ConfigCommentViewModel() : base("コメント") {

            //一応通知を飛ばしておく
        }

        //開いてる動画があれば設定を反映させる
        public void ApplyConfig() {
            
            foreach(var tab in App.ViewModelRoot.TabItems) {

                if(tab is VideoViewModel) {

                    var vm = (VideoViewModel) tab;
                    vm.ApplyChanges();
                }
            }
        }


        public string ToJson() {

            dynamic root = new DynamicJson();

            root.Hide3DSComment = Hide3DSComment;
            root.HideWiiUComment = HideWiiUComment;
            root.NGSharedLevel = NGSharedLevel;
            root.CommentAlpha = CommentAlpha / 100; //%から小数点に
            root.DefaultCommentSize = DefaultCommentSize;

            return root.ToString();
        }
    }
}
