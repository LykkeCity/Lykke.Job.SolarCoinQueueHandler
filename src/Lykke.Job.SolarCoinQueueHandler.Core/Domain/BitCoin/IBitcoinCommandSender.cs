using System.Threading.Tasks;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin.Commands;

namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin
{
    public interface IBitcoinCommandSender
    {
        Task SendCommand(BaseCommand command);
    }
}
