using Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Job.SolarCoinQueueHandler.AzureRepositories.BitCoin
{
    public class WalletCredentialsEntity : TableEntity, IWalletCredentials
    {
        public static class BySolarCoinWallet
        {
            public static string GeneratePartitionKey()
            {
                return "SolarCoinWallet";
            }

            public static string GenerateRowKey(string address)
            {
                return address;
            }
        }

        public string ClientId { get; set; }
        public string Address { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string MultiSig { get; set; }
        public string ColoredMultiSig { get; set; }
        public bool PreventTxDetection { get; set; }
        public string EncodedPrivateKey { get; set; }
        public string BtcConvertionWalletPrivateKey { get; set; }
        public string BtcConvertionWalletAddress { get; set; }
        public string EthConversionWalletAddress { get; set; }
        public string EthAddress { get; set; }
        public string EthPublicKey { get; set; }
        public string SolarCoinWalletAddress { get; set; }
        public string ChronoBankContract { get; set; }
        public string QuantaContract { get; set; }
    }
}
