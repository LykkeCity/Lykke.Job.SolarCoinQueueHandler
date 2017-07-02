using System.Threading.Tasks;

namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.PaymentSystems
{
    public interface IPaymentSystemsRawLog
    {
        Task RegisterEventAsync(IPaymentSystemRawLogEvent evnt);

    }
}