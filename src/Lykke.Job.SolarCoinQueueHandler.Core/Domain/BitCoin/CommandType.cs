namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin
{
    public enum CommandType
    {
        Unknown,
        Issue,
        CashOut,
        Transfer,
        TransferAll,
        Destroy,
        Swap
    }
}