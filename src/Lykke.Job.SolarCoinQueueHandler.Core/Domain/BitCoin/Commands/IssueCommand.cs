namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin.Commands
{
    public class IssueCommand : BaseCommand
    {
        public string Multisig { get; set; }
        public string AssetId { get; set; }
        public double Amount { get; set; }

        public override CommandType Type => CommandType.Issue;
    }
}