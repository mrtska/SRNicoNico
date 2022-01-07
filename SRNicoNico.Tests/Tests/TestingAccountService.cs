using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SRNicoNico.Entities;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.Tests {
    public class TestingAccountService : IAccountService {
        public Task<bool> AddMutedAccountAsync(AccountType type, string accountId) {
            throw new NotImplementedException();
        }

        public void FlushCache() {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MutedAccount>> GetMutedAccountsAsync() {
            throw new NotImplementedException();
        }

        public bool IsMuted(VideoItem item) {
            return false;
        }

        public bool IsMutedChannel(string channelId) {
            return false;
        }

        public bool IsMutedCommunity(string communityId) {
            return false;
        }

        public bool IsMutedUser(string userId) {
            return false;
        }

        public Task RemoveMutedAccountAsync(AccountType type, string accountId) {
            throw new NotImplementedException();
        }
    }
}
