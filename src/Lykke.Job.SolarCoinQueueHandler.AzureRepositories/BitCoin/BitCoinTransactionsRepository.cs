using System.Threading.Tasks;
using AzureStorage;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin;

namespace Lykke.Job.SolarCoinQueueHandler.AzureRepositories.BitCoin
{
    public class BitCoinTransactionsRepository : IBitCoinTransactionsRepository
    {
        private readonly INoSQLTableStorage<BitCoinTransactionEntity> _tableStorage;

        public BitCoinTransactionsRepository(INoSQLTableStorage<BitCoinTransactionEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task CreateAsync(string transactionId, string commandType,
            string requestData, string contextData, string response, string blockchainHash = null)
        {
            var newEntity = BitCoinTransactionEntity.ByTransactionId.CreateNew(transactionId, commandType, requestData, contextData, response, blockchainHash);
            await _tableStorage.InsertAsync(newEntity);
        }
    }
}