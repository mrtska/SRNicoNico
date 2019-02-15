using Livet;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class RankingViewModel : TabItemViewModel {

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

        #region RankingList変更通知プロパティ
        private ObservableSynchronizedCollection<RankingEntryViewModel> _RankingList;

        public ObservableSynchronizedCollection<RankingEntryViewModel> RankingList {
            get { return _RankingList; }
            set { 
                if (_RankingList == value)
                    return;
                _RankingList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public RankingViewModel() : base("ランキング") {

            RankingList = new ObservableSynchronizedCollection<RankingEntryViewModel>();
        }

        public void Initialize() {

            RankingList.Clear();

            if (Settings.Instance.RankingCategoryTotal) {

                RankingList.Add(new RankingEntryViewModel(this, "カテゴリ合算", "all"));
            }

            if (Settings.Instance.RankingEntameMusic) {

                RankingList.Add(new RankingEntryViewModel(this, "エンタメ・音楽", "g_ent2"));
            }

            if (Settings.Instance.RankingEntertainment) {

                RankingList.Add(new RankingEntryViewModel(this, "エンターテイメント", "ent"));
            }

            if (Settings.Instance.RankingMusic) {

                RankingList.Add(new RankingEntryViewModel(this, "音楽", "music"));
            }

            if (Settings.Instance.RankingSingaSong) {

                RankingList.Add(new RankingEntryViewModel(this, "歌ってみた", "sing"));
            }

            if (Settings.Instance.RankingPlayaMusic) {

                RankingList.Add(new RankingEntryViewModel(this, "演奏してみた", "play"));
            }

            if (Settings.Instance.RankingDancing) {

                RankingList.Add(new RankingEntryViewModel(this, "踊ってみた", "dance"));
            }

            if (Settings.Instance.RankingVOCALOID) {

                RankingList.Add(new RankingEntryViewModel(this, "VOCALOID", "vocaloid"));
            }

            if (Settings.Instance.RankingIndies) {

                RankingList.Add(new RankingEntryViewModel(this, "ニコニコインディーズ", "nicoindies"));
            }
            
            if (Settings.Instance.RankingASMR) {

                RankingList.Add(new RankingEntryViewModel(this, "ASMR", "asmr"));
            }

            if (Settings.Instance.RankingMMD) {

                RankingList.Add(new RankingEntryViewModel(this, "MMD", "mmd"));
            }

            if (Settings.Instance.RankingVirtual) {

                RankingList.Add(new RankingEntryViewModel(this, "バーチャル", "virtual"));
            }

            if (Settings.Instance.RankingLifeSports) {

                RankingList.Add(new RankingEntryViewModel(this, "生活・一般・スポ", "g_life2"));
            }

            if (Settings.Instance.RankingAnimal) {

                RankingList.Add(new RankingEntryViewModel(this, "動物", "animal"));
            }

            if (Settings.Instance.RankingCooking) {

                RankingList.Add(new RankingEntryViewModel(this, "料理", "cooking"));
            }

            if (Settings.Instance.RankingNature) {

                RankingList.Add(new RankingEntryViewModel(this, "自然", "nature"));
            }

            if (Settings.Instance.RankingTravel) {

                RankingList.Add(new RankingEntryViewModel(this, "旅行", "travel"));
            }

            if (Settings.Instance.RankingSports) {

                RankingList.Add(new RankingEntryViewModel(this, "スポーツ", "sport"));
            }

            if (Settings.Instance.RankingNicoNicoDougaLecture) {

                RankingList.Add(new RankingEntryViewModel(this, "ニコニコ動画講座", "lecture"));
            }

            if (Settings.Instance.RankingDriveVideo) {

                RankingList.Add(new RankingEntryViewModel(this, "車載動画", "drive"));
            }

            if (Settings.Instance.RankingHistory) {

                RankingList.Add(new RankingEntryViewModel(this, "歴史", "history"));
            }

            if (Settings.Instance.RankingTrain) {

                RankingList.Add(new RankingEntryViewModel(this, "鉄道", "train"));
            }

            if (Settings.Instance.RankingPolitics) {

                RankingList.Add(new RankingEntryViewModel(this, "政治", "g_politics"));
            }

            if (Settings.Instance.RankingScienceTech) {

                RankingList.Add(new RankingEntryViewModel(this, "科学・技術", "g_tech"));
            }

            if (Settings.Instance.RankingScience) {

                RankingList.Add(new RankingEntryViewModel(this, "科学", "science"));
            }

            if (Settings.Instance.RankingNicoNicoTech) {

                RankingList.Add(new RankingEntryViewModel(this, "ニコニコ技術部", "tech"));
            }

            if (Settings.Instance.RankingHandicraft) {

                RankingList.Add(new RankingEntryViewModel(this, "ニコニコ手芸部", "handcraft"));
            }

            if (Settings.Instance.RankingMaking) {

                RankingList.Add(new RankingEntryViewModel(this, "作ってみた", "make"));
            }

            if (Settings.Instance.RankingAnimeGameIllust) {

                RankingList.Add(new RankingEntryViewModel(this, "アニメ・ゲーム・絵", "g_culture2"));
            }

            if (Settings.Instance.RankingAnime) {

                RankingList.Add(new RankingEntryViewModel(this, "アニメ", "anime"));
            }

            if (Settings.Instance.RankingGame) {

                RankingList.Add(new RankingEntryViewModel(this, "ゲーム", "game"));
            }

            if (Settings.Instance.RankingJikkyo) {

                RankingList.Add(new RankingEntryViewModel(this, "実況プレイ動画", "jikkyo"));
            }

            if (Settings.Instance.RankingTouhou) {

                RankingList.Add(new RankingEntryViewModel(this, "東方", "toho"));
            }

            if (Settings.Instance.RankingIdolmaster) {

                RankingList.Add(new RankingEntryViewModel(this, "アイドルマスター", "imas"));
            }

            if (Settings.Instance.RankingRadio) {

                RankingList.Add(new RankingEntryViewModel(this, "ラジオ", "radio"));
            }

            if (Settings.Instance.RankingDrawing) {

                RankingList.Add(new RankingEntryViewModel(this, "描いてみた", "draw"));
            }

            if (Settings.Instance.RankingTRPG) {

                RankingList.Add(new RankingEntryViewModel(this, "TRPG", "trpg"));
            }

            if (Settings.Instance.RankingOtherTotal) {

                RankingList.Add(new RankingEntryViewModel(this, "その他合算", "g_other"));
            }

            if (Settings.Instance.RankingReinoAre) {

                RankingList.Add(new RankingEntryViewModel(this, "例のアレ", "are"));
            }

            if (Settings.Instance.RankingDiary) {

                RankingList.Add(new RankingEntryViewModel(this, "日記", "diary"));
            }

            if (Settings.Instance.RankingOther) {

                RankingList.Add(new RankingEntryViewModel(this, "その他", "other"));
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
