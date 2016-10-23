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
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.ViewModels {
    public class ConfigCommentViewModel : ConfigViewModelBase {



        #region CommentAlpha変更通知プロパティ
        public float CommentAlpha {
            get { return Settings.Instance.CommentAlpha; }
            set { 
                if(Settings.Instance.CommentAlpha == value)
                    return;
                Settings.Instance.CommentAlpha = value;
                RaisePropertyChanged();
                ApplyConfig();
            }
        }
        #endregion


        #region DefaultCommentSize変更通知プロパティ
        public string DefaultCommentSize {
            get { return Settings.Instance.CommentSize; }
            set { 
                if(Settings.Instance.CommentSize == value)
                    return;
                Settings.Instance.CommentSize = value;
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
            
            foreach(var tab in App.ViewModelRoot.VideoTabs) {

                if(tab is VideoViewModel) {

                    var vm = (VideoViewModel) tab;
                    // TODO fix
                    vm.Handler.ApplyChanges();
                }
            }
        }


        public string ToJson() {

            dynamic root = new DynamicJson();

            root.Hide3DSComment = App.ViewModelRoot.Config.NGComment.Hide3DSComment;
            root.HideWiiUComment = App.ViewModelRoot.Config.NGComment.HideWiiUComment;
            root.NGSharedLevel = App.ViewModelRoot.Config.NGComment.NGSharedLevel;
            root.CommentAlpha = CommentAlpha / 100; //%から小数点に
            root.DefaultCommentSize = DefaultCommentSize;

            return root.ToString();
        }
    }
}
