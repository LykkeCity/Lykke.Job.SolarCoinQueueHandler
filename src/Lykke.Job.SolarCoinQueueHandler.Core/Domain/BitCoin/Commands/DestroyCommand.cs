namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin.Commands
{
    public class DestroyCommand : BaseCommand
    {
        public string Address { get; set; }
        public string AssetId { get; set; }
        public double Amount { get; set; }

        public override CommandType Type => CommandType.Destroy;
    }
}