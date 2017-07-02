using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.Assets
{
    public interface IAssetsRepository
    {
        Task RegisterAssetAsync(IAsset asset);
        Task EditAssetAsync(string id, IAsset asset);
        Task<IEnumerable<IAsset>> GetAssetsAsync();
        Task<IAsset> GetAssetAsync(string id);
        Task<IEnumerable<IAsset>> GetAssetsForCategoryAsync(string category);
        Task SetDisabled(string id, bool value);
    }
}