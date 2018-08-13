﻿using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class MylistResultEntryViewModel : ViewModel {

        #region IsDescriptionEditMode変更通知プロパティ
        private bool _IsDescriptionEditMode = false;

        public bool IsDescriptionEditMode {
            get { return _IsDescriptionEditMode; }
            set { 
                if(_IsDescriptionEditMode == value)
                    return;
                _IsDescriptionEditMode = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public NicoNicoMylistEntry Item { get; set; }

        internal MylistResultViewModel Owner;

        public MylistResultEntryViewModel(MylistResultViewModel owner, NicoNicoMylistEntry item) {

            Owner = owner;
            Item = item;
        }

        public void Open() {

            //編集モードの時は開かない
            if(!Owner.IsEditMode) {

                NicoNicoOpener.Open(Item.ContentUrl);
            }
        }
        
        public async void UpdateDescription() {

            if(await Owner.MylistInstance.Item.UpdateDescriptionAsync(Owner.Group, Item, await Owner.MylistInstance.GetMylistTokenAsync())) {
                //何か マイリストコメントの更新に成功したときにしたい処理とかあれば
            }
            IsDescriptionEditMode = false;
        }


    }
}
