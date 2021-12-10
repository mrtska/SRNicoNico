using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SRNicoNico.Entities;
using SRNicoNico.Models;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// IAccountServiceのNicoNicoViewer実装
    /// </summary>
    public class NicoNicoViewerAccountService : IAccountService {

        // Initializeがコンストラクタから呼ばれるのでnullableではない
        private List<string> UserCache = default!;
        private List<string> CommunityCache = default!;
        private List<string> ChannelCache = default!;

        private readonly ViewerDbContext DbContext;

        public NicoNicoViewerAccountService(ViewerDbContext dbContext) {

            DbContext = dbContext;
            Initialize();
        }

        private void Initialize() {

            UserCache = DbContext.MutedAccounts.AsNoTracking().Where(w => w.AccountType == AccountType.User).Select(s => s.AccountId).ToList();
            CommunityCache = DbContext.MutedAccounts.AsNoTracking().Where(w => w.AccountType == AccountType.Community).Select(s => s.AccountId).ToList();
            ChannelCache = DbContext.MutedAccounts.AsNoTracking().Where(w => w.AccountType == AccountType.Channel).Select(s => s.AccountId).ToList();
        }

        /// <inheritdoc />
        public bool IsMuted(VideoItem item) {

            if (item == null) {
                return false;
            }

            if (item.OwnerType == "user") {
                return IsMutedUser(item.OwnerId!);
            }
            if (item.OwnerType == "channel") {
                return IsMutedChannel(item.OwnerId!);
            }
            if (item.OwnerType == "community") {
                return IsMutedCommunity(item.OwnerId!);
            }

            return false;
        }

        /// <inheritdoc />
        public bool IsMutedChannel(string channelId) {

            if (string.IsNullOrEmpty(channelId)) {
                throw new ArgumentNullException(nameof(channelId));
            }

            return ChannelCache.Contains(channelId);
        }

        /// <inheritdoc />
        public bool IsMutedCommunity(string communityId) {

            if (string.IsNullOrEmpty(communityId)) {
                throw new ArgumentNullException(nameof(communityId));
            }
        
            return CommunityCache.Contains(communityId);
        }

        /// <inheritdoc />
        public bool IsMutedUser(string userId) {

            if (string.IsNullOrEmpty(userId)) {
                throw new ArgumentNullException(nameof(userId));
            }

            return UserCache.Contains(userId);
        }

        /// <inheritdoc />
        public void FlushCache() {

            // もう一回初期化するだけ
            Initialize();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<MutedAccount>> GetMutedAccountsAsync() {

            return await DbContext.MutedAccounts.AsNoTracking().ToListAsync();
        }

        /// <inheritdoc />
        public async Task AddMutedAccountAsync(AccountType type, string accountId) {

            var result = await DbContext.MutedAccounts.AsNoTracking().AnyAsync(s => s.AccountType == type && s.AccountId == accountId).ConfigureAwait(false);
            if (!result) {

                DbContext.MutedAccounts.Add(new MutedAccount {
                    AccountType = type,
                    AccountId = accountId
                });
                await DbContext.SaveChangesAsync();
                FlushCache();
            }
        }

        /// <inheritdoc />
        public async Task RemoveMutedAccountAsync(AccountType type, string accountId) {

            var result = await DbContext.MutedAccounts.SingleOrDefaultAsync(s => s.AccountType == type && s.AccountId == accountId).ConfigureAwait(false);
            if (result != null) {

                DbContext.MutedAccounts.Remove(result);
                await DbContext.SaveChangesAsync();
                FlushCache();
            }
        }
    }
}
