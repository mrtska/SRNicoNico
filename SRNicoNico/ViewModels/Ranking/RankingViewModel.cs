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

            if(Settings.Instance.RankingCategoryTotal) {

                RankingList.Add(new RankingEntryViewModel("カテゴリ合算", "all", RankingInstance));
            }

            if(Settings.Instance.RankingEntameMusic) {

                RankingList.Add(new RankingEntryViewModel("エンタメ・音楽", "g_ent2", RankingInstance));
            }

            if(Settings.Instance.RankingEntertainment) {

                RankingList.Add(new RankingEntryViewModel("エンターテイメント", "ent", RankingInstance));
            }

            if(Settings.Instance.RankingMusic) {

                RankingList.Add(new RankingEntryViewModel("音楽", "music", RankingInstance));
            }

            if(Settings.Instance.RankingSingaSong) {

                RankingList.Add(new RankingEntryViewModel("歌ってみた", "sing", RankingInstance));
            }

            if(Settings.Instance.RankingPlayaMusic) {

                RankingList.Add(new RankingEntryViewModel("演奏してみた", "play", RankingInstance));
            }

            if(Settings.Instance.RankingDancing) {

                RankingList.Add(new RankingEntryViewModel("踊ってみた", "dance", RankingInstance));
            }

            if(Settings.Instance.RankingVOCALOID) {

                RankingList.Add(new RankingEntryViewModel("VOCALOID", "vocaloid", RankingInstance));
            }

            if(Settings.Instance.RankingIndies) {

                RankingList.Add(new RankingEntryViewModel("ニコニコインディーズ", "nicoindies", RankingInstance));
            }

            if(Settings.Instance.RankingLifeSports) {

                RankingList.Add(new RankingEntryViewModel("生活・一般・スポ", "g_life2", RankingInstance));
            }

            if(Settings.Instance.RankingAnimal) {

                RankingList.Add(new RankingEntryViewModel("動物", "animal", RankingInstance));
            }

            if(Settings.Instance.RankingCooking) {

                RankingList.Add(new RankingEntryViewModel("料理", "cooking", RankingInstance));
            }

            if(Settings.Instance.RankingNature) {

                RankingList.Add(new RankingEntryViewModel("自然", "nature", RankingInstance));
            }

            if(Settings.Instance.RankingTravel) {

                RankingList.Add(new RankingEntryViewModel("旅行", "travel", RankingInstance));
            }

            if(Settings.Instance.RankingSports) {

                RankingList.Add(new RankingEntryViewModel("スポーツ", "sport", RankingInstance));
            }

            if(Settings.Instance.RankingNicoNicoDougaLecture) {

                RankingList.Add(new RankingEntryViewModel("ニコニコ動画講座", "lecture", RankingInstance));
            }

            if(Settings.Instance.RankingDriveVideo) {

                RankingList.Add(new RankingEntryViewModel("車載動画", "drive", RankingInstance));
            }

            if(Settings.Instance.RankingHistory) {

                RankingList.Add(new RankingEntryViewModel("歴史", "history", RankingInstance));
            }

            if(Settings.Instance.RankingPolitics) {

                RankingList.Add(new RankingEntryViewModel("政治", "g_politics", RankingInstance));
            }

            if(Settings.Instance.RankingScienceTech) {

                RankingList.Add(new RankingEntryViewModel("科学・技術", "g_tech", RankingInstance));
            }

            if(Settings.Instance.RankingScience) {

                RankingList.Add(new RankingEntryViewModel("科学", "science", RankingInstance));
            }

            if(Settings.Instance.RankingNicoNicoTech) {

                RankingList.Add(new RankingEntryViewModel("ニコニコ技術部", "tech", RankingInstance));
            }

            if(Settings.Instance.RankingHandicraft) {

                RankingList.Add(new RankingEntryViewModel("ニコニコ手芸部", "handcraft", RankingInstance));
            }

            if(Settings.Instance.RankingMaking) {

                RankingList.Add(new RankingEntryViewModel("作ってみた", "make", RankingInstance));
            }

            if(Settings.Instance.RankingAnimeGameIllust) {

                RankingList.Add(new RankingEntryViewModel("アニメ・ゲーム・絵", "g_culture2", RankingInstance));
            }

            if(Settings.Instance.RankingAnime) {

                RankingList.Add(new RankingEntryViewModel("アニメ", "anime", RankingInstance));

            }

            if(Settings.Instance.RankingGame) {

                RankingList.Add(new RankingEntryViewModel("ゲーム", "game", RankingInstance));
            }

            if(Settings.Instance.RankingTouhou) {

                RankingList.Add(new RankingEntryViewModel("東方", "toho", RankingInstance));
            }

            if(Settings.Instance.RankingIdolmaster) {

                RankingList.Add(new RankingEntryViewModel("アイドルマスター", "imas", RankingInstance));
            }

            if(Settings.Instance.RankingRadio) {

                RankingList.Add(new RankingEntryViewModel("ラジオ", "radio", RankingInstance));
            }

            if(Settings.Instance.RankingDrawing) {

                RankingList.Add(new RankingEntryViewModel("描いてみた", "draw", RankingInstance));
            }

            if(Settings.Instance.RankingOtherTotal) {

                RankingList.Add(new RankingEntryViewModel("その他合算", "g_other", RankingInstance));
            }

            if(Settings.Instance.RankingReinoAre) {

                RankingList.Add(new RankingEntryViewModel("例のアレ", "are", RankingInstance));
            }

            if(Settings.Instance.RankingDiary) {

                RankingList.Add(new RankingEntryViewModel("日記", "diary", RankingInstance));
            }

            if(Settings.Instance.RankingOther) {

                RankingList.Add(new RankingEntryViewModel("その他", "other", RankingInstance));
            }
        }

        public void Refresh() {

            Initialize();

        }




    }
}
