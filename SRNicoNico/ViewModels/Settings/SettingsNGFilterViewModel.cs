using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Text.RegularExpressions;

namespace SRNicoNico.ViewModels {
    public class SettingsNGFilterViewModel : TabItemViewModel {

        #region NGFilter変更通知プロパティ
        private NicoNicoNGFilter _NGFilter;

        public NicoNicoNGFilter NGFilter {
            get { return _NGFilter; }
            set { 
                if(_NGFilter == value)
                    return;
                _NGFilter = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public SettingsNGFilterViewModel() : base("コメントNGフィルター") {

            NGFilter = new NicoNicoNGFilter();
        }

        public void AddEmptyNGEntry() {

            var entry = new NGCommentEntry();
            entry.IsEnabled = false;
            entry.Type = NGType.Word;
            entry.Content = "";
            NGFilter.AddNGEntry(entry);
        }

        public void AddNGEntry(NGType type, string content) {

            var entry = new NGCommentEntry();
            entry.IsEnabled = true;
            entry.Type = type;
            entry.Content = content;
            NGFilter.AddNGEntry(entry);
        }

        public void DeleteNGEntry(NGCommentEntry target) {

            NGFilter.NGList.Remove(target);
            NGFilter.Save();
        }

        public void Filter(NicoNicoCommentEntry entry) {

            

            if(Settings.Instance.HideWiiUComment) {

                if(entry.Mail.Contains("device:WiiU")) {

                    entry.Rejected = true;
                    return;
                }
            }
            if(Settings.Instance.Hide3DSComment) {
                
                if(entry.Mail.Contains("device:3DS")) {

                    entry.Rejected = true;
                    return;
                }
            }

            switch(Settings.Instance.NGSharedLevel) {
                case "無":
                    break;
                case "弱":
                    if(entry.Score <= -10000) {

                        entry.Rejected = true;
                        return;
                    }
                    break;
                case "中":
                    if(entry.Score <= -4800) {

                        entry.Rejected = true;
                        return;
                    }
                    break;
                case "強":
                    if(entry.Score <= -1000) {

                        entry.Rejected = true;
                        return;
                    }
                    break;
                case "最強":
                    if(entry.Score < 0) {

                        entry.Rejected = true;
                        return;
                    }
                    break;
            }

            foreach(var ng in NGFilter.NGList) {

                if(!ng.IsEnabled) {

                    continue;
                }
                switch(ng.Type) {
                    case NGType.RegEx:

                        if(Regex.Match(entry.Content, ng.Content).Success) {

                            entry.Rejected = true;
                            return;
                        }
                        break;
                    case NGType.Command:

                        if(entry.Mail.Contains(ng.Content)) {

                            entry.Rejected = true;
                            return;
                        }

                        break;
                    case NGType.UserId:

                        if(entry.UserId == ng.Content) {

                            entry.Rejected = true;
                            return;
                        }
                        break;
                    case NGType.Word:

                        if(entry.Content == ng.Content) {

                            entry.Rejected = true;
                            return;
                        }
                        break;
                    case NGType.WordContains:

                        if(entry.Content.Contains(ng.Content)) {

                            entry.Rejected = true;
                            return;
                        }
                        break;
                }
            }
        }
    }
}
