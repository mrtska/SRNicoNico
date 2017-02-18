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

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class VideoCommentEntryViewModel : ViewModel {

        #region Item変更通知プロパティ
        private NicoNicoCommentEntry _Item;

        public NicoNicoCommentEntry Item {
            get { return _Item; }
            set { 
                if(_Item == value)
                    return;
                _Item = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private readonly VideoCommentViewModel Owner;

        public VideoCommentEntryViewModel(VideoCommentViewModel owner, NicoNicoCommentEntry vm) {

            Owner = owner;
            Item = vm;
        }

        public void JumpTo() {

            Owner.Owner.Handler.Seek(Item.Vpos / 100.0D);
        }

        public void AddCommentIntoNGFilter() {

            App.ViewModelRoot.Setting.NGFilter.AddNGEntry(NGType.Word, Item.Content);
        }

        public void AddUserIntoNGFilter() {

            App.ViewModelRoot.Setting.NGFilter.AddNGEntry(NGType.UserId, Item.UserId);
        }



    }
}
