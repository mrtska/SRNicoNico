using System.Windows.Input;
using Livet;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// ユーザーページの公開マイリストのViewModel
    /// </summary>
    public class UserMylistViewModel : TabItemViewModel {

        private ObservableSynchronizedCollection<MylistListEntry> _MylistItems = new ObservableSynchronizedCollection<MylistListEntry>();
        /// <summary>
        /// マイリスト一覧
        /// </summary>
        public ObservableSynchronizedCollection<MylistListEntry> MylistItems {
            get { return _MylistItems; }
            set { 
                if (_MylistItems == value)
                    return;
                _MylistItems = value;
                RaisePropertyChanged();
            }
        }

        private readonly IMylistService MylistService;
        private readonly string UserId;

        public UserMylistViewModel(IMylistService mylistService, string userId) : base("マイリスト") {

            MylistService = mylistService;
            UserId = userId;
        }

        public async void Loaded() {

            IsActive = true;
            Status = "マイリストを取得中";
            MylistItems.Clear();
            try {

                await foreach (var mylist in MylistService.GetUserPublicMylistAsync(UserId, 3)) {
                    // 改行をスペースに置換する
                    mylist.Description = mylist.Description!.Replace("\r\n", " ");
                    MylistItems.Add(mylist);
                }

                Status = string.Empty;
            } catch (StatusErrorException e) {

                Status = $"マイリストを取得出来ませんでした。 ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
            }
        }

        /// <summary>
        /// 再読み込み
        /// </summary>
        public void Reload() {

            Loaded();
        }

        public override void KeyDown(KeyEventArgs e) {

            // F5で更新
            if (e.Key == Key.F5) {

                Reload();
                e.Handled = true;
            }
        }
    }
}
