using Livet;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class RankingEntryViewModel : TabItemViewModel {

        public NicoNicoRanking Model { get; set; }
        private readonly string Category;

        #region Page変更通知プロパティ
        private string _Page = "1-100";

        public string Page {
            get { return _Page; }
            set {
                if (_Page == value)
                    return;
                _Page = value;
                RaisePropertyChanged();
                Refresh();
            }
        }
        #endregion

        private readonly RankingViewModel Owner;

        public RankingEntryViewModel(RankingViewModel owner, string name, string category) : base(name) {

            Owner = owner;
            Category = category;

            Model = new NicoNicoRanking(Owner.Period, Owner.Target);
        }

        public async void Initialize(int page) {

            IsActive = true;
            Owner.Status = "ランキング読み込み中:" + Name;
            Owner.Status = await Model.GetRankingAsync(Category, page);

            IsActive = false;
        }

        private int TransPage() {

            switch (Page) {
                case "1-100":
                    return 1;
                case "101-200":
                    return 2;
                case "201-300":
                    return 3;
                case "301-400":
                    return 4;
                case "401-500":
                    return 5;
                case "501-600":
                    return 6;
                case "601-700":
                    return 7;
                case "701-800":
                    return 8;
                case "801-900":
                    return 9;
                case "901-1000":
                    return 10;
                default:
                    return 1;
            }
        }
        public void Refresh() {

            Initialize(TransPage());
        }
    }
}
