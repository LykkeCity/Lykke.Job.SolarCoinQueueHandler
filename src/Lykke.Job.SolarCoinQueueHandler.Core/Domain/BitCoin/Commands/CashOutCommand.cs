namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin.Commands
{
    public class CashOutCommand : BaseCommand
    {
        public string SourceAddress { get; set; }
        public string DestinationAddress { get; set; }
        public string AssetId { get; set; }
        public double Amount { get; set; }

        public override CommandType Type => CommandType.CashOut;
    }
}