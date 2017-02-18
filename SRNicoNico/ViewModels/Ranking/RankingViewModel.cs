using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class RankingViewModel : TabItemViewModel {


        private NicoNicoRanking RankingInstance;


        #region RankingList変更通知プロパティ
        private DispatcherCollection<RankingResultViewModel> _RankingList = new DispatcherCollection<RankingResultViewModel>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<RankingResultViewModel> RankingList {
            get { return _RankingList; }
            set {
                if(_RankingList == value)
                    return;
                _RankingList = value;
                RaisePropertyChanged();
            }
        }
        #endregion


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

        #region SelectedIndex変更通知プロパティ
        private int _SelectedIndex = 0;

        public int SelectedIndex {
            get { return _SelectedIndex; }
            set { 
                if(_SelectedIndex == value)
                    return;
                _SelectedIndex = value;
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

                RankingList.Add(new RankingResultViewModel(this, "カテゴリ合算", "all", RankingInstance));
            }

            if(Settings.Instance.RankingEntameMusic) {

                RankingList.Add(new RankingResultViewModel(this, "エンタメ・音楽", "g_ent2", RankingInstance));
            }

            if(Settings.Instance.RankingEntertainment) {

                RankingList.Add(new RankingResultViewModel(this, "エンターテイメント", "ent", RankingInstance));
            }

            if(Settings.Instance.RankingMusic) {

                RankingList.Add(new RankingResultViewModel(this, "音楽", "music", RankingInstance));
            }

            if(Settings.Instance.RankingSingaSong) {

                RankingList.Add(new RankingResultViewModel(this, "歌ってみた", "sing", RankingInstance));
            }

            if(Settings.Instance.RankingPlayaMusic) {

                RankingList.Add(new RankingResultViewModel(this, "演奏してみた", "play", RankingInstance));
            }

            if(Settings.Instance.RankingDancing) {

                RankingList.Add(new RankingResultViewModel(this, "踊ってみた", "dance", RankingInstance));
            }

            if(Settings.Instance.RankingVOCALOID) {

                RankingList.Add(new RankingResultViewModel(this, "VOCALOID", "vocaloid", RankingInstance));
            }

            if(Settings.Instance.RankingIndies) {

                RankingList.Add(new RankingResultViewModel(this, "ニコニコインディーズ", "nicoindies", RankingInstance));
            }

            if(Settings.Instance.RankingLifeSports) {

                RankingList.Add(new RankingResultViewModel(this, "生活・一般・スポ", "g_life2", RankingInstance));
            }

            if(Settings.Instance.RankingAnimal) {

                RankingList.Add(new RankingResultViewModel(this, "動物", "animal", RankingInstance));
            }

            if(Settings.Instance.RankingCooking) {

                RankingList.Add(new RankingResultViewModel(this, "料理", "cooking", RankingInstance));
            }

            if(Settings.Instance.RankingNature) {

                RankingList.Add(new RankingResultViewModel(this, "自然", "nature", RankingInstance));
            }

            if(Settings.Instance.RankingTravel) {

                RankingList.Add(new RankingResultViewModel(this, "旅行", "travel", RankingInstance));
            }

            if(Settings.Instance.RankingSports) {

                RankingList.Add(new RankingResultViewModel(this, "スポーツ", "sport", RankingInstance));
            }

            if(Settings.Instance.RankingNicoNicoDougaLecture) {

                RankingList.Add(new RankingResultViewModel(this, "ニコニコ動画講座", "lecture", RankingInstance));
            }

            if(Settings.Instance.RankingDriveVideo) {

                RankingList.Add(new RankingResultViewModel(this, "車載動画", "drive", RankingInstance));
            }

            if(Settings.Instance.RankingHistory) {

                RankingList.Add(new RankingResultViewModel(this, "歴史", "history", RankingInstance));
            }

            if(Settings.Instance.RankingPolitics) {

                RankingList.Add(new RankingResultViewModel(this, "政治", "g_politics", RankingInstance));
            }

            if(Settings.Instance.RankingScienceTech) {

                RankingList.Add(new RankingResultViewModel(this, "科学・技術", "g_tech", RankingInstance));
            }

            if(Settings.Instance.RankingScience) {

                RankingList.Add(new RankingResultViewModel(this, "科学", "science", RankingInstance));
            }

            if(Settings.Instance.RankingNicoNicoTech) {

                RankingList.Add(new RankingResultViewModel(this, "ニコニコ技術部", "tech", RankingInstance));
            }

            if(Settings.Instance.RankingHandicraft) {

                RankingList.Add(new RankingResultViewModel(this, "ニコニコ手芸部", "handcraft", RankingInstance));
            }

            if(Settings.Instance.RankingMaking) {

                RankingList.Add(new RankingResultViewModel(this, "作ってみた", "make", RankingInstance));
            }

            if(Settings.Instance.RankingAnimeGameIllust) {

                RankingList.Add(new RankingResultViewModel(this, "アニメ・ゲーム・絵", "g_culture2", RankingInstance));
            }

            if(Settings.Instance.RankingAnime) {

                RankingList.Add(new RankingResultViewModel(this, "アニメ", "anime", RankingInstance));

            }

            if(Settings.Instance.RankingGame) {

                RankingList.Add(new RankingResultViewModel(this, "ゲーム", "game", RankingInstance));
            }

            if(Settings.Instance.RankingJikkyo) {

                RankingList.Add(new RankingResultViewModel(this, "実況プレイ動画", "jikkyo", RankingInstance));
            }

            if(Settings.Instance.RankingTouhou) {

                RankingList.Add(new RankingResultViewModel(this, "東方", "toho", RankingInstance));
            }

            if(Settings.Instance.RankingIdolmaster) {

                RankingList.Add(new RankingResultViewModel(this, "アイドルマスター", "imas", RankingInstance));
            }

            if(Settings.Instance.RankingRadio) {

                RankingList.Add(new RankingResultViewModel(this, "ラジオ", "radio", RankingInstance));
            }

            if(Settings.Instance.RankingDrawing) {

                RankingList.Add(new RankingResultViewModel(this, "描いてみた", "draw", RankingInstance));
            }

            if(Settings.Instance.RankingOtherTotal) {

                RankingList.Add(new RankingResultViewModel(this, "その他合算", "g_other", RankingInstance));
            }

            if(Settings.Instance.RankingReinoAre) {

                RankingList.Add(new RankingResultViewModel(this, "例のアレ", "are", RankingInstance));
            }

            if(Settings.Instance.RankingDiary) {

                RankingList.Add(new RankingResultViewModel(this, "日記", "diary", RankingInstance));
            }

            if(Settings.Instance.RankingOther) {

                RankingList.Add(new RankingResultViewModel(this, "その他", "other", RankingInstance));
            }

            SelectedIndex = 0;
        }
        public void Refresh() {

            Initialize();
        }

        public override void KeyDown(KeyEventArgs e) {

            if(e.Key == Key.F5) {

                if(e.KeyboardDevice.Modifiers == ModifierKeys.Control) {

                    Refresh();
                } else {

                    RankingList[SelectedIndex]?.Refresh();
                }
                e.Handled = true;
            }
        }
    }
}
