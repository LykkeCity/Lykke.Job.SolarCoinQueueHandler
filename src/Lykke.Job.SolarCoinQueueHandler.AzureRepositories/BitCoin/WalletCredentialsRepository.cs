using System.Threading.Tasks;
using AzureStorage;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin;

namespace Lykke.Job.SolarCoinQueueHandler.AzureRepositories.BitCoin
{
    public class WalletCredentialsRepository : IWalletCredentialsRepository
    {
        private readonly INoSQLTableStorage<WalletCredentialsEntity> _tableStorage;

        public WalletCredentialsRepository(INoSQLTableStorage<WalletCredentialsEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task<IWalletCredentials> GetBySolarCoinWalletAsync(string address)
        {
            var partitionKey = WalletCredentialsEntity.BySolarCoinWallet.GeneratePartitionKey();
            var rowKey = WalletCredentialsEntity.BySolarCoinWallet.GenerateRowKey(address);

            return await _tableStorage.GetDataAsync(partitionKey, rowKey);
        }
    }
}