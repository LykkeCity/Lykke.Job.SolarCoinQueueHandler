using System;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.CachOperations;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Job.SolarCoinQueueHandler.AzureRepositories.CacheOperations
{
    public class TransferEventEntity : TableEntity, ITransferEvent
    {
        public string Id => RowKey;
        public DateTime DateTime { get; set; }
        public bool IsHidden { get; set; }
        public string FromId { get; set; }
        public string AssetId { get; set; }
        public double Amount { get; set; }
        public string BlockChainHash { get; set; }
        public string Multisig { get; set; }
        public string TransactionId { get; set; }
        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }
        public bool? IsSettled { get; set; }
        public string StateField { get; set; }
        public TransactionStates State
        {
            get
            {
                TransactionStates type = TransactionStates.InProcessOnchain;
                if (!string.IsNullOrEmpty(StateField))
                {
                    Enum.TryParse(StateField, out type);
                }
                return type;
            }
            set { StateField = value.ToString(); }
        }
        public string ClientId { get; set; }

        public static class ByClientId
        {
            public static string GeneratePartitionKey(string clientId)
            {
                return clientId;
            }

            public static string GenerateRowKey(string id)
            {
                return id;
            }

            public static TransferEventEntity Create(ITransferEvent src)
            {
                return new TransferEventEntity
                {
                    PartitionKey = GeneratePartitionKey(src.ClientId),
                    DateTime = src.DateTime,
                    Amount = src.Amount,
                    AssetId = src.AssetId,
                    FromId = src.FromId,
                    BlockChainHash = src.BlockChainHash,
                    TransactionId = src.TransactionId,
                    IsHidden = src.IsHidden,
                    AddressFrom = src.AddressFrom,
                    AddressTo = src.AddressTo,
                    Multisig = src.Multisig,
                    ClientId = src.ClientId,
                    IsSettled = src.IsSettled,
                    State = src.State
                };
            }
        }

        public static class ByMultisig
        {
            public static string GeneratePartitionKey(string multisig)
            {
                return multisig;
            }

            public static string GenerateRowKey(string id)
            {
                return id;
            }

            public static TransferEventEntity Create(ITransferEvent src, string id)
            {
                return new TransferEventEntity
                {
                    PartitionKey = GeneratePartitionKey(src.Multisig),
                    RowKey = id,
                    DateTime = src.DateTime,
                    Amount = src.Amount,
                    AssetId = src.AssetId,
                    FromId = src.FromId,
                    BlockChainHash = src.BlockChainHash,
                    TransactionId = src.TransactionId,
                    IsHidden = src.IsHidden,
                    AddressFrom = src.AddressFrom,
                    AddressTo = src.AddressTo,
                    Multisig = src.Multisig,
                    ClientId = src.ClientId,
                    IsSettled = src.IsSettled,
                    State = src.State
                };
            }
        }
    }
}