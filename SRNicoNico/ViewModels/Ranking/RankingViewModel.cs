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
using System.Windows.Controls;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using SRNicoNico.Models.NicoNicoViewer;

namespace SRNicoNico.ViewModels {
    public class RankingViewModel : TabItemViewModel {

        private NicoNicoRanking RankingInstance;



        #region Period変更通知プロパティ
        private string _Period = "24時間";

        public string Period {
            get { return _Period; }
            set { 
                if(_Period == value)
                    return;
                _Period = value;
                RaisePropertyChanged();
                Initialize();
            }
        }
        #endregion


        #region Target変更通知プロパティ
        private string _Target = "総合";

        public string Target {
            get { return _Target; }
            set { 
                if(_Target == value)
                    return;
                _Target = value;
                RaisePropertyChanged();
                Initialize();
            }
        }
        #endregion



        #region RankingList変更通知プロパティ
        private DispatcherCollection<RankingEntryViewModel> _RankingList = new DispatcherCollection<RankingEntryViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<RankingEntryViewModel> RankingList {
            get { return _RankingList; }
            set { 
                if(_RankingList == value)
                    return;
                _RankingList = value;
                RaisePropertyChanged();
            }
        }
        #endregion



        public RankingViewModel() : base("ランキング") {

        }

        public void Initialize() {

            RankingInstance = new NicoNicoRanking(Period, Target);

            RankingList.Clear();

            RankingList.Add(new RankingEntryViewModel("カテゴリ合算", "all", RankingInstance));
            RankingList.Add(new RankingEntryViewModel("エンタメ・音楽", "g_ent2", RankingInstance));
            RankingList.Add(new RankingEntryViewModel("生活・一般・スポ", "g_life2", RankingInstance));
            RankingList.Add(new RankingEntryViewModel("政治", "g_politics", RankingInstance));
            RankingList.Add(new RankingEntryViewModel("科学・技術", "g_tech", RankingInstance));
            RankingList.Add(new RankingEntryViewModel("アニメ・ゲーム・絵", "g_culture2", RankingInstance));
            RankingList.Add(new RankingEntryViewModel("その他", "g_other", RankingInstance));

        }

        public void Refresh() {



        }




    }
}
