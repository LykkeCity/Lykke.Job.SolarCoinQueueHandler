using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.Assets
{
    public static class AssetsRepositoryExt
    {
        public static async Task<IAsset> FindAssetByBlockchainAssetIdAsync(this IAssetsRepository assetsRepository, string blockchainAssetId)
        {
            if (blockchainAssetId == null)
                return await assetsRepository.GetAssetAsync(LykkeConstants.BitcoinAssetId);


            var assets = await assetsRepository.GetAssetsAsync();
            return assets.FirstOrDefault(itm => itm.BlockChainAssetId == blockchainAssetId || itm.Id == blockchainAssetId);
        }

        public static bool IsColoredAssetId(this string assetId)
        {
            return assetId != null && assetId != LykkeConstants.BitcoinAssetId;
        }

        public static string GetFirstAssetId(this IEnumerable<IAsset> assets)
        {
            return assets.OrderBy(x => x.DefaultOrder).First().Id;
        }

        public static IAsset GetAssetByBcnId(this IEnumerable<IAsset> assets, string bcnId)
        {
            return string.IsNullOrEmpty(bcnId)
                ? assets.FirstOrDefault(x => x.Id == LykkeConstants.BitcoinAssetId)
                : assets.FirstOrDefault(x => x.BlockChainAssetId == bcnId);
        }

        public static bool IsBitcoinAsset(this IEnumerable<IAsset> assets, string assetId)
        {
            var asset = assets.FirstOrDefault(x => x.Id == assetId);

            if (asset == null)
                throw new ArgumentException("Unknown asset");

            return asset.Blockchain == Blockchain.Bitcoin;
        }
    }
}