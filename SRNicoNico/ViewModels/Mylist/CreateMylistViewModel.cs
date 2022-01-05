using System.Collections.Generic;
using FastEnumUtility;
using Livet.Messaging.Windows;
using SRNicoNico.Models;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// マイリスト作成ウィンドウのViewModel
    /// </summary>
    public class CreateMylistViewModel : TabItemViewModel {

        private string _MylistName = string.Empty;
        /// <summary>
        /// マイリスト名
        /// </summary>
        public string MylistName {
            get { return _MylistName; }
            set { 
                if (_MylistName == value)
                    return;
                _MylistName = value;
                RaisePropertyChanged();
            }
        }

        private string _MylistDescription = string.Empty;
        /// <summary>
        /// マイリスト説明文
        /// </summary>
        public string MylistDescription {
            get { return _MylistDescription; }
            set { 
                if (_MylistDescription == value)
                    return;
                _MylistDescription = value;
                RaisePropertyChanged();
            }
        }

        private MylistSortKey _SelectedSortKey;
        /// <summary>
        /// ソートキー
        /// </summary>
        public MylistSortKey SelectedSortKey {
            get { return _SelectedSortKey; }
            set { 
                if (_SelectedSortKey == value)
                    return;
                _SelectedSortKey = value;
                RaisePropertyChanged();
            }
        }

        private bool _IsPublic = false;
        /// <summary>
        /// 公開するかどうか
        /// </summary>
        public bool IsPublic {
            get { return _IsPublic; }
            set { 
                if (_IsPublic == value)
                    return;
                _IsPublic = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// マイリストのソート順のリスト
        /// </summary>
        public IEnumerable<MylistSortKey> SortKeyItems { get; } = FastEnum.GetValues<MylistSortKey>();

        public bool IsCreated { get; private set; }

        private readonly IMylistService MylistService;

        public CreateMylistViewModel(IMylistService mylistService, MainWindowViewModel vm) : base("マイリスト作成") {

            MylistService = mylistService;
            PropertyChanged += (o, e) => {

                if (e.PropertyName == nameof(Status)) {
                    vm.Status = Status;
                }
            };
        }

        /// <summary>
        /// マイリストを作成する
        /// </summary>
        public async void Create() {

            IsActive = true;
            Status = "マイリストを作成中";
            try {

                if (await MylistService.CreateMylistAsync(MylistName, MylistDescription, IsPublic, SelectedSortKey)) {

                    IsCreated = true;
                    Messenger.Raise(new WindowActionMessage(WindowAction.Close));
                } else {

                    Status = "マイリストの作成に失敗しました";
                }
            } catch (StatusErrorException e) {

                Status = $"マイリストの作成に失敗しました。 ステータスコード: {e.StatusCode}";
            } finally {

                IsActive = false;
            }
        }
    }
}
