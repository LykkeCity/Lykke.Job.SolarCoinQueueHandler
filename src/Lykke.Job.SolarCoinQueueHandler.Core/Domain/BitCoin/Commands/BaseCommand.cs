using System;

namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin.Commands
{
    public class BaseCommand
    {
        public virtual CommandType Type { get; set; }
        public string Context { get; set; }
        public Guid? TransactionId { get; set; }
    }
}