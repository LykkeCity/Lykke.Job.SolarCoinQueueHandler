using System.Threading.Tasks;

namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin
{
    public interface IWalletCredentialsRepository
    {
        Task<IWalletCredentials> GetBySolarCoinWalletAsync(string address);
    }
}