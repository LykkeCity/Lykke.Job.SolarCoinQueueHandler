namespace Lykke.Job.SolarCoinQueueHandler.Contract
{
    public class SolarCashInMsg
    {
        public string Address { get; set; }
        public double Amount { get; set; }
        public string TxId { get; set; }
    }
}