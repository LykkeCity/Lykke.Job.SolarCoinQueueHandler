namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.CachOperations
{
    public interface ITransferEvent : IBaseCashBlockchainOperation
    {
        string FromId { get; }
    }
}