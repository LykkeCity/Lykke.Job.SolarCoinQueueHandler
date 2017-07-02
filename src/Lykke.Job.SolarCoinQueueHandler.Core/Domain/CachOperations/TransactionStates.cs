namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.CachOperations
{
    public enum TransactionStates
    {
        InProcessOnchain,
        SettledOnchain,
        InProcessOffchain,
        SettledOffchain
    }
}