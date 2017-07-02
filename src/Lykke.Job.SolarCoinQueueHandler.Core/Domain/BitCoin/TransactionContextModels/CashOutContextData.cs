namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin.TransactionContextModels
{
    public class CashOutContextData : BaseContextData
    {
        public string ClientId { get; set; }
        public string AssetId { get; set; }
        public string Address { get; set; }
        public double Amount { get; set; }
        public string CashOperationId { get; set; }
        public AdditionalData AddData { get; set; }

        public static CashOutContextData Create(string clientId, string assetId, string address, double amount,
            string cashOpId, AdditionalData additionalData = null)
        {
            return new CashOutContextData
            {
                ClientId = clientId,
                AssetId = assetId,
                Amount = amount,
                Address = address,
                CashOperationId = cashOpId,
                AddData = additionalData,
                SignsClientIds = new[] { clientId }
            };
        }

        #region Additional data

        public class AdditionalData
        {
            public SwiftData SwiftData { get; set; }
            public ForwardWithdrawal ForwardWithdrawal { get; set; }
        }

        public class SwiftData
        {
            public string CashOutRequestId { get; set; }
        }

        public class ForwardWithdrawal
        {
            public string Id { get; set; }
        }

        #endregion

    }
}