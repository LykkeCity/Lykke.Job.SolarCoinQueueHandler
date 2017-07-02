namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin.TransactionContextModels
{
    public class TransferContextData : BaseContextData
    {
        public string SourceClient { get; set; }
        public TransferType TransferType { get; set; }

        public class TransferModel
        {
            public string ClientId { get; set; }
            public string OperationId { get; set; }
            public AdditionalActions Actions { get; set; }

            public static TransferModel Create(string clientId, string operationId)
            {
                return new TransferModel
                {
                    ClientId = clientId,
                    OperationId = operationId
                };
            }
        }

        public TransferModel[] Transfers { get; set; }


        public static TransferContextData Create(string srcClientId, params TransferModel[] transfers)
        {
            return new TransferContextData
            {
                SourceClient = srcClientId,
                Transfers = transfers,
                SignsClientIds = new[] { srcClientId }
            };
        }

        #region Actions

        public class AdditionalActions
        {
            /// <summary>
            /// If set, then transfer complete email with conversion to LKK will be sent on successful resonse from queue
            /// </summary>
            public ConvertedOkEmailAction CashInConvertedOkEmail { get; set; }

            /// <summary>
            /// If set, then push notification will be sent when transfer detected and confirmed
            /// </summary>
            public PushNotification PushNotification { get; set; }

            /// <summary>
            /// If set, transfer complete email will be sent
            /// </summary>
            public EmailAction SendTransferEmail { get; set; }

            /// <summary>
            /// If set, then another transfer will be generated on successful resonse from queue
            /// </summary>
            public GenerateTransferAction GenerateTransferAction { get; set; }

            /// <summary>
            /// For margin wallet deposit
            /// </summary>
            public UpdateMarginBalance UpdateMarginBalance { get; set; }
        }

        public class ConvertedOkEmailAction
        {
            public ConvertedOkEmailAction(string assetFromId, double price, double amountFrom, double amountLkk)
            {
                AssetFromId = assetFromId;
                Price = price;
                AmountFrom = amountFrom;
                AmountLkk = amountLkk;
            }

            public string AssetFromId { get; set; }
            public double Price { get; set; }
            public double AmountFrom { get; set; }
            public double AmountLkk { get; set; }
        }

        public class EmailAction
        {
            public EmailAction(string assetId, double amount)
            {
                AssetId = assetId;
                Amount = amount;
            }

            public string AssetId { get; set; }
            public double Amount { get; set; }
        }

        public class PushNotification
        {
            public PushNotification(string assetId, double amount)
            {
                AssetId = assetId;
                Amount = amount;
            }

            /// <summary>
            /// Id of credited asset
            /// </summary>
            public string AssetId { get; set; }

            public double Amount { get; set; }
        }

        public class GenerateTransferAction
        {
            public string DestClientId { get; set; }
            public string SourceClientId { get; set; }
            public double Amount { get; set; }
            public string AssetId { get; set; }
            public double Price { get; set; }
            public double AmountFrom { get; set; }
            public string FromAssetId { get; set; }
        }

        public class UpdateMarginBalance
        {
            public string AccountId { get; set; }
            public double Amount { get; set; }

            public UpdateMarginBalance(string account, double amount)
            {
                AccountId = account;
                Amount = amount;
            }
        }

        #endregion
    }
}