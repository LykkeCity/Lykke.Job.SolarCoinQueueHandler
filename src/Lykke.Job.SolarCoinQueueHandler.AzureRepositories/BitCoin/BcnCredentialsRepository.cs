using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Job.SolarCoinQueueHandler.AzureRepositories.BitCoin
{
    public class BcnCredentialsRecordEntity : TableEntity, IBcnCredentialsRecord
    {
        public static class ByClientId
        {
            public static string GeneratePartition(string clientId)
            {
                return clientId;
            }

            public static string GenerateRowKey(string assetId)
            {
                return assetId;
            }

            public static BcnCredentialsRecordEntity Create(IBcnCredentialsRecord record)
            {
                return new BcnCredentialsRecordEntity
                {
                    Address = record.Address,
                    AssetAddress = record.AssetAddress,
                    AssetId = record.AssetId,
                    ClientId = record.ClientId,
                    EncodedKey = record.EncodedKey,
                    PublicKey = record.PublicKey,
                    PartitionKey = GeneratePartition(record.ClientId),
                    RowKey = GenerateRowKey(record.AssetId)
                };
            }
        }


        public static class ByAssetAddress
        {
            public static string GeneratePartition()
            {
                return "ByAssetAddress";
            }

            public static string GenerateRowKey(string assetAddress)
            {
                return assetAddress;
            }

            public static BcnCredentialsRecordEntity Create(IBcnCredentialsRecord record)
            {
                return new BcnCredentialsRecordEntity
                {
                    Address = record.Address,
                    AssetAddress = record.AssetAddress,
                    AssetId = record.AssetId,
                    ClientId = record.ClientId,
                    EncodedKey = record.EncodedKey,
                    PublicKey = record.PublicKey,
                    PartitionKey = GeneratePartition(),
                    RowKey = GenerateRowKey(record.AssetAddress)
                };
            }
        }


        public string Address { get; set; }
        public string EncodedKey { get; set; }
        public string PublicKey { get; set; }
        public string ClientId { get; set; }
        public string AssetAddress { get; set; }
        public string AssetId { get; set; }
    }

    public class BcnClientCredentialsRepository : IBcnClientCredentialsRepository
    {
        private readonly INoSQLTableStorage<BcnCredentialsRecordEntity> _tableStorage;

        public BcnClientCredentialsRepository(INoSQLTableStorage<BcnCredentialsRecordEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task<IBcnCredentialsRecord> GetByAssetAddressAsync(string assetAddress)
        {
            return await _tableStorage.GetDataAsync(BcnCredentialsRecordEntity.ByAssetAddress.GeneratePartition(),
                BcnCredentialsRecordEntity.ByAssetAddress.GenerateRowKey(assetAddress));
        }
    }
}
