namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin.Commands
{
    public class TransferAllCommand : BaseCommand
    {
        public string SourceAddress { get; set; }
        public string DestinationAddress { get; set; }

        public override CommandType Type => CommandType.TransferAll;
    }
}