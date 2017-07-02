using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.CachOperations
{
    public interface ITransferEventsRepository
    {
        Task<ITransferEvent> RegisterAsync(ITransferEvent transferEvent);

        Task<IEnumerable<ITransferEvent>> GetAsync(string clientId);
        Task<ITransferEvent> GetAsync(string clientId, string id);

        Task UpdateBlockChainHashAsync(string clientId, string id, string blockChainHash);

        Task SetBtcTransactionAsync(string clientId, string id, string btcTransaction);
        Task SetIsSettledIfExistsAsync(string clientId, string id, bool offchain);

        Task<IEnumerable<ITransferEvent>> GetByHashAsync(string blockchainHash);
        Task<IEnumerable<ITransferEvent>> GetByMultisigAsync(string multisig);
        Task<IEnumerable<ITransferEvent>> GetByMultisigsAsync(string[] multisigs);
        Task GetDataByChunksAsync(Func<IEnumerable<ITransferEvent>, Task> chunk);
    }

}