using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin
{
    public interface IWalletCredentialsHistoryRepository
    {
        Task InsertHistoryRecord(IWalletCredentials oldWalletCredentials);
        Task<IEnumerable<string>> GetPrevMultisigsForUser(string clientId);
    }
}