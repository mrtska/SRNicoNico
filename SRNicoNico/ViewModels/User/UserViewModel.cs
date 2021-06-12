using System.Linq;
using System.Windows.Input;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;
using Unity;
using Unity.Resolution;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ユーザーページのViewModel
    /// </summary>
    public class UserViewModel : TabItemViewModel {

        private ObservableSynchronizedCollection<TabItemViewModel> _UserItems = new ObservableSynchronizedCollection<TabItemViewModel>();
        /// <summary>
        /// ユーザーのタブのリスト
        /// </summary>
        public ObservableSynchronizedCollection<TabItemViewModel> UserItems {
            get { return _UserItems; }
            set {
                if (_UserItems == value)
                    return;
                _UserItems = value;
                RaisePropertyChanged();
            }
        }

        private TabItemViewModel? _SelectedItem;
        /// <summary>
        /// 現在選択されているタブ デフォルトはユーザーニコレポ
        /// </summary>
        public TabItemViewModel? SelectedItem {
            get { return _SelectedItem; }
            set {
                if (_SelectedItem == value)
                    return;
                _SelectedItem = value;
                RaisePropertyChanged();
            }
        }

        private UserDetails? _UserDetails;
        /// <summary>
        /// ユーザー詳細
        /// </summary>
        public UserDetails? UserDetails {
            get { return _UserDetails; }
            set { 
                if (_UserDetails == value)
                    return;
                _UserDetails = value;
                RaisePropertyChanged();
            }
        }

        private bool _IsFollow;
        /// <summary>
        /// フォローしているかどうか
        /// </summary>
        public bool IsFollow {
            get { return _IsFollow; }
            set { 
                if (_IsFollow == value)
                    return;
                _IsFollow = value;
                RaisePropertyChanged();
            }
        }

        private readonly IUnityContainer UnityContainer;
        private readonly IUserService UserService;

        private readonly string UserId;

        public UserViewModel(IUnityContainer unityContainer, IUserService userService, string userId) : base("ユーザー") {

            UnityContainer = unityContainer;
            UserService = userService;

            UserId = userId;
        }

        /// <summary>
        /// その他タブの一覧をインスタンス化する
        /// </summary>
        public async void Loaded() {

            IsActive = true;
            Status = "ユーザー情報を取得中";
            try {
                UserDetails = await UserService.GetUserAsync(UserId);
                IsFollow = UserDetails.IsFollowing;
                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"ユーザー詳細を取得出来ませんでした。 ステータスコード: {e.StatusCode}";
                return;
            } finally {
                IsActive = false;
            }

            UserItems.Clear();

            var po = new ParameterOverride("userId", UserId);
            UserItems.Add(UnityContainer.Resolve<UserNicoRepoViewModel>(po));
            UserItems.Add(UnityContainer.Resolve<UserFolloweeViewModel>(po));
            UserItems.Add(UnityContainer.Resolve<UserFollowerViewModel>(po));
            UserItems.Add(UnityContainer.Resolve<UserMylistViewModel>(po));
            UserItems.Add(UnityContainer.Resolve<UserVideoViewModel>(po));
            UserItems.Add(UnityContainer.Resolve<UserSeriesViewModel>(po));

            // 子ViewModelのStatusを監視する
            UserItems.ToList().ForEach(vm => {

                vm.PropertyChanged += (o, e) => {

                    var tabItem = (TabItemViewModel)o;
                    if (e.PropertyName == nameof(Status)) {

                        Status = tabItem.Status;
                    }
                };
            });

            // ニコレポをデフォルト値とする
            SelectedItem = UserItems.First();
        }

        /// <summary>
        /// フォローするまたは解除する
        /// </summary>
        public void ToggleFollow() {


        }

        /// <summary>
        /// ユーザー詳細を再読込する
        /// </summary>
        public void Reload() {

            Loaded();
        }

        /// <summary>
        /// タブを閉じる
        /// </summary>
        public void Close() {

            Dispose();
        }

        public override void KeyDown(KeyEventArgs e) {



            SelectedItem?.KeyDown(e);
        }
    }
}
