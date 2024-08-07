using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SRNicoNico.Entities;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Services;

namespace SRNicoNico.Tests {
    public class TestingHistoryService : IHistoryService {
        public Task<bool> DeleteAccountHistoryAsync(string videoId) {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteLocalHistoryAsync(string videoId) {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<HistoryVideoItem> GetAccountHistoryAsync() {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<LocalHistory> GetLocalHistoryAsync() {
            throw new NotImplementedException();
        }

        public Task<bool> HasWatchedAsync(string videoId) {
            return Task.FromResult(false);
        }

        public Task<bool> SyncLocalHistoryAsync(IEnumerable<HistoryVideoItem> histories) {
            throw new NotImplementedException();
        }
    }
}
