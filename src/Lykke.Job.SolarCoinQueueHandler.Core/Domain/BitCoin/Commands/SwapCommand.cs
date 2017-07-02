namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin.Commands
{
    public class SwapCommand : BaseCommand
    {
        public string MultisigCustomer1 { get; set; }
        public double Amount1 { get; set; }
        public string Asset1 { get; set; }

        public string MultisigCustomer2 { get; set; }
        public double Amount2 { get; set; }
        public string Asset2 { get; set; }

        public override CommandType Type => CommandType.Swap;
    }
}