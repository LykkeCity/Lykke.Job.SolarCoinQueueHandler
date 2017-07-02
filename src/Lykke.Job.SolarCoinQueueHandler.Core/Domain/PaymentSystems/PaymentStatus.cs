namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.PaymentSystems
{
    public enum PaymentStatus
    {
        Created,
        NotifyProcessed,
        NotifyDeclined,
        Processing
    }
}