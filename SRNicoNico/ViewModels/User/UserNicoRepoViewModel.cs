using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows.Input;

namespace SRNicoNico.ViewModels {
    public class UserNicoRepoViewModel : TabItemViewModel {

        #region Filter変更通知プロパティ
        private string _Filter;

        public string Filter {
            get { return _Filter; }
            set {
                if (_Filter == value)
                    return;
                _Filter = value;
                Model.SetFilter(value);
            }
        }
        #endregion

        #region Closed変更通知プロパティ
        private bool _Closed;

        public bool Closed {
            get { return _Closed; }
            set {
                if(_Closed == value)
                    return;
                _Closed = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public NicoNicoNicoRepo Model { get; set; }

        private readonly UserViewModel Owner;
        public UserNicoRepoViewModel(UserViewModel user) : base("ニコレポ") {

            Owner = user;
            Model = new NicoNicoNicoRepo(user.Model.UserInfo.UserId);
        }

        public void Initialize() {

            Closed = false;
            GetMore();
        }

        public async void GetMore() {

            if (IsActive) {

                return;
            }

            IsActive = true;

            Owner.Status = "ニコレポ取得中：" + Name;
            Owner.Status = await Model.GetNicoRepoAsync();

            if(Model.NicoRepoList.Count == 0) {

                Closed = true;
            }

            IsActive = false;
        }

        public void Refresh() {

            Model.Reset();
            Initialize();
        }

        public override void KeyDown(KeyEventArgs e) {

            if(e.Key == Key.F5) {

                Refresh();
            }
            if(e.Key == Key.Space) {

                GetMore();
            }
        }
    }
}
