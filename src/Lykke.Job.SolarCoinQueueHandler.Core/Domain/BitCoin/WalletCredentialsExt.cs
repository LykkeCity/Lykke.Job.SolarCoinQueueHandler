using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.Assets;

namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin
{
    public static class WalletCredentialsExt
    {
        public static async Task<string[]> GetAllClientMultisigs(this IWalletCredentialsRepository walletCredsRepo,
            IWalletCredentialsHistoryRepository walletHistoryCredsRepo, string clientId)
        {
            var currentCreds = await walletCredsRepo.GetAsync(clientId);
            var prevMultisigs = await walletHistoryCredsRepo.GetPrevMultisigsForUser(clientId);

            var res = new List<string>();
            res.Add(currentCreds.MultiSig);
            res.AddRange(prevMultisigs);

            return res.Distinct().ToArray();
        }

        public static string GetDepositAddressForAsset(this IWalletCredentials walletCredentials, IAsset asset)
        {
            if (asset.Blockchain == Blockchain.Ethereum)
                return null;

            switch (asset.Id)
            {
                case LykkeConstants.BitcoinAssetId:
                    return walletCredentials.MultiSig;
                case LykkeConstants.SolarAssetId:
                    return walletCredentials.SolarCoinWalletAddress;
                case LykkeConstants.ChronoBankAssetId:
                    return walletCredentials.ChronoBankContract;
                case LykkeConstants.QuantaAssetId:
                    return walletCredentials.QuantaContract;
                default:
                    return walletCredentials.ColoredMultiSig;
            }
        }
    }
}