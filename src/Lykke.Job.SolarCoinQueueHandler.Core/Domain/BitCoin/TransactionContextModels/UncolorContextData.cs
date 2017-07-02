namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin.TransactionContextModels
{
    public class UncolorContextData : BaseContextData
    {
        public string ClientId { get; set; }
        public string AssetId { get; set; }
        public double Amount { get; set; }
        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }

        public string CashOperationId { get; set; }
    }
}