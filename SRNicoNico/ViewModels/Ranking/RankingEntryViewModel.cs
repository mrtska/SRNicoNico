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

namespace SRNicoNico.ViewModels {
    public class RankingEntryViewModel : TabItemViewModel {

        private readonly NicoNicoRanking RankingInstance;


        private readonly string Category;

        #region RankingItemList変更通知プロパティ
        private DispatcherCollection<RankingItem> _RankingItemList = new DispatcherCollection<RankingItem>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<RankingItem> RankingItemList {
            get { return _RankingItemList; }
            set { 
                if(_RankingItemList == value)
                    return;
                _RankingItemList = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        public RankingEntryViewModel(string title, string category, NicoNicoRanking instance) : base(title) {

            Category = category;
            RankingInstance = instance;
        }

        public async void Initialize() {

            
            var a = await RankingInstance.GetRankingAsync(Category, 1);
            ;
            foreach(var item in a.ItemList) {

                RankingItemList.Add(item);
            }


        }

        public void Refresh() {

            Initialize();
        }

    }
}
