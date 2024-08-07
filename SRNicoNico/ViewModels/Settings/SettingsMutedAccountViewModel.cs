using System.Collections.Generic;
using FastEnumUtility;
using Livet;
using SRNicoNico.Entities;
using SRNicoNico.Models;
using SRNicoNico.Services;

namespace SRNicoNico.ViewModels {
    /// <summary>
    /// 設定ページのミュートタブのViewModel
    /// </summary>
    public class SettingsMutedAccountViewModel : TabItemViewModel {

        /// <summary>
        /// ミュートされた動画のプレースホルダーを表示しない
        /// </summary>
        public bool HideMutedVideo {
            get { return Settings.HideMutedVideo; }
            set {
                if (Settings.HideMutedVideo == value)
                    return;
                Settings.HideMutedVideo = value;
                Settings.ChangeMutedAccount();
                RaisePropertyChanged();
            }
        }


        private AccountType _AccountType;
        /// <summary>
        /// 設定追加用のアカウントタイプ
        /// </summary>
        public AccountType AccountType {
            get { return _AccountType; }
            set { 
                if (_AccountType == value)
                    return;
                _AccountType = value;
                RaisePropertyChanged();
            }
        }

        private string _AccountId = string.Empty;
        /// <summary>
        /// 設定追加用のアカウントタイプ
        /// </summary>
        public string AccountId {
            get { return _AccountId; }
            set { 
                if (_AccountId == value)
                    return;
                _AccountId = value;
                RaisePropertyChanged();
            }
        }
        public static IEnumerable<AccountType> AccountTypes { get; } = FastEnum.GetValues<AccountType>();

        private ObservableSynchronizedCollection<MutedAccount> _MutedAccount = new ObservableSynchronizedCollection<MutedAccount>();

        public ObservableSynchronizedCollection<MutedAccount> MutedAccount {
            get { return _MutedAccount; }
            set { 
                if (_MutedAccount == value)
                    return;
                _MutedAccount = value;
                RaisePropertyChanged();
            }
        }

        private readonly ISettings Settings;
        private readonly IAccountService AccountService;

        public SettingsMutedAccountViewModel(ISettings settings, IAccountService accountService) : base("ミュート設定") {

            Settings = settings;
            AccountService = accountService;
        }

        public async void Loaded() {

            var result = await AccountService.GetMutedAccountsAsync();

            MutedAccount.Clear();
            foreach (var account in result) {

                MutedAccount.Add(account);
            }
        }

        public async void AddEntry() {

            if (string.IsNullOrEmpty(AccountId)) {
                Status = "IDを入力してください";
                return;
            }
            if (await AccountService.AddMutedAccountAsync(AccountType, AccountId)) {

                Status = string.Empty;
                AccountId = string.Empty;
                Reload();
            } else {

                Status = "既に同じ設定が登録済みです";
            }

        }

        public async void DeleteEntry(MutedAccount account) {

            await AccountService.RemoveMutedAccountAsync(account.AccountType, account.AccountId);
            Reload();
        }

        public void Reload() {

            Loaded();
        }
    }
}
