using Livet;
using SRNicoNico.Models.NicoNicoWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRNicoNico.ViewModels {
    public class VideoCommentEntryViewModel : ViewModel {


        #region Item変更通知プロパティ
        private NicoNicoCommentEntry _Item;

        public NicoNicoCommentEntry Item {
            get { return _Item; }
            set { 
                if (_Item == value)
                    return;
                _Item = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private readonly VideoHtml5Handler Handler;

        public VideoCommentEntryViewModel(NicoNicoCommentEntry entry, VideoHtml5Handler handler) {

            Item = entry;
            Handler = handler;
        }
        public void JumpTo() {

            Handler.Seek(Item.Vpos / 100.0D);
        }

        public void AddCommentIntoNGFilter() {

            App.ViewModelRoot.Setting.NGFilter.AddNGEntry(NGType.Word, Item.Content);
        }

        public void AddUserIntoNGFilter() {

            App.ViewModelRoot.Setting.NGFilter.AddNGEntry(NGType.UserId, Item.UserId);
        }
    }
}
