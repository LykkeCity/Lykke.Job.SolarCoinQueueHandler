using System;
using System.Threading.Tasks;
using Common;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin.Commands;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin.TransactionContextModels;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.CachOperations;
using Lykke.MatchingEngine.Connector.Abstractions.Services;

namespace Lykke.Job.SolarCoinQueueHandler.Services.Exchange
{
    public class ExchangeOperationsService
    {
        private readonly IMatchingEngineConnector _matchingEngineConnector;
        private readonly ITransferEventsRepository _transferEventsRepository;
        private readonly IBitcoinCommandSender _bitcoinCommandSender;
        private readonly IBitCoinTransactionsRepository _bitCoinTransactionsRepository;


        public ExchangeOperationsService(IMatchingEngineConnector matchingEngineConnector,
            ITransferEventsRepository transferEventsRepository, IBitcoinCommandSender bitcoinCommandSender,
            IBitCoinTransactionsRepository bitCoinTransactionsRepository)
        {
            _matchingEngineConnector = matchingEngineConnector;
            _transferEventsRepository = transferEventsRepository;
            _bitcoinCommandSender = bitcoinCommandSender;
            _bitCoinTransactionsRepository = bitCoinTransactionsRepository;
        }

        public async Task<ResultModel> IssueAsync(string clientId, string assetId, double amount)
        {
            var transactionId = Guid.NewGuid().ToString();

            //create transaction and place context
            var contextData = new IssueContextData
            {
                Amount = amount,
                AssetId = assetId,
                ClientId = clientId,
                SignsClientIds = new[] { clientId }
            };

            await _bitCoinTransactionsRepository.CreateAsync(transactionId, BitCoinCommands.Issue,
                "", contextData.ToJson(), "");

            //Send to Matchin Engine
            var result = await _matchingEngineConnector
                .CashInOutAsync(transactionId, clientId, assetId, Math.Abs(amount));

            return result.ToDomainModel();
        }

        public async Task StartCashOutAsync(string offchainTransferId, string clientId, string address, double amount, string assetId,
            CashOutContextData.AdditionalData additionalData = null, string blockchainHash = null)
        {
            //create transaction and place context
            var contextData = CashOutContextData.Create(clientId, assetId, address, amount, null, additionalData);

            await _bitCoinTransactionsRepository.CreateAsync(offchainTransferId, BitCoinCommands.CashOut,
                "", contextData.ToJson(), "", blockchainHash);
        }

        public async Task<ResultModel> FinishCashOutAsync(string offchainTransferId, string clientId, double amount, string assetId)
        {
            //Send to Matchin Engine
            var result = await _matchingEngineConnector.CashInOutAsync(offchainTransferId, clientId, assetId, -Math.Abs(amount));

            return result.ToDomainModel();
        }

        public async Task<ResultModel> CashOutAsync(string clientId, string address, double amount, string assetId,
            CashOutContextData.AdditionalData additionalData = null, string blockchainHash = null, Guid? txId = null)
        {
            var transactionId = txId?.ToString() ?? Guid.NewGuid().ToString();

            //create transaction and place context
            var contextData = CashOutContextData
                .Create(clientId, assetId, address, amount, null, additionalData);

            await _bitCoinTransactionsRepository.CreateAsync(transactionId, BitCoinCommands.CashOut,
                "", contextData.ToJson(), "", blockchainHash);

            //Send to Matchin Engine
            var result = await _matchingEngineConnector
                .CashInOutAsync(transactionId, clientId, assetId, -Math.Abs(amount));

            return result.ToDomainModel();
        }

        public async Task<ResultModel> TransferConvertedBetweenClientsWithNotification(string destClientId, string sourceClientId,
            double amount, string assetId, double price, double amountFrom, string fromAssetId)
        {
            var additionalActionsDest = new TransferContextData.AdditionalActions
            {
                CashInConvertedOkEmail = new TransferContextData.ConvertedOkEmailAction(fromAssetId, price, amountFrom, amount),
                PushNotification = new TransferContextData.PushNotification(assetId, amount)
            };

            return await GenerateTransfer(destClientId, sourceClientId, amount, assetId, additionalActionsDest);
        }

        public async Task<ResultModel> TransferBetweenClientsWithNotification(string destClientId, string sourceClientId,
            double amount, string assetId)
        {
            var additionalActionsDest = new TransferContextData.AdditionalActions
            {
                SendTransferEmail = new TransferContextData.EmailAction(assetId, amount),
                PushNotification = new TransferContextData.PushNotification(assetId, amount)
            };

            return await GenerateTransfer(destClientId, sourceClientId, amount, assetId, additionalActionsDest);
        }

        public async Task<ResultModel> GenerateTransfer(string destClientId, string sourceClientId, double amount,
            string assetId,
            TransferContextData.AdditionalActions additionalActionsDest = null,
            TransferContextData.AdditionalActions additionalActionsSrc = null, TransferType type = TransferType.Common)
        {
            amount = Math.Abs(amount);
            var transactionId = Guid.NewGuid().ToString();

            //create transaction and place context
            var contextData = TransferContextData.Create(sourceClientId, new TransferContextData.TransferModel
            {
                ClientId = destClientId,
                Actions = additionalActionsDest
            }, new TransferContextData.TransferModel
            {
                ClientId = sourceClientId,
                Actions = additionalActionsSrc
            });

            contextData.TransferType = type;

            await _bitCoinTransactionsRepository.CreateAsync(transactionId, BitCoinCommands.Transfer,
                "", contextData.ToJson(), "");

            //Send to Matchin Engine
            var meResponse = await _matchingEngineConnector
                .TransferAsync(transactionId, sourceClientId, destClientId, assetId, amount);

            var result = meResponse.ToDomainModel();
            result.TransactionId = transactionId;

            return result;
        }

        public async Task StartTransferFromClientAsync(string offchainTransferId, string destClientId, string sourceClientId,
            TransferContextData.AdditionalActions additionalActionsDest = null,
            TransferContextData.AdditionalActions additionalActionsSrc = null, TransferType type = TransferType.Common)
        {
            //create transaction and place context
            var contextData = TransferContextData.Create(sourceClientId, new TransferContextData.TransferModel
            {
                ClientId = destClientId,
                Actions = additionalActionsDest
            }, new TransferContextData.TransferModel
            {
                ClientId = sourceClientId,
                Actions = additionalActionsSrc
            });

            contextData.TransferType = type;

            await _bitCoinTransactionsRepository.CreateAsync(offchainTransferId, BitCoinCommands.Transfer, "", contextData.ToJson(), "");
        }

        public async Task<ResultModel> FinishTransferFromClientAsync(string offchainTransferId, string sourceClient, string destClient, double amount, string assetId)
        {
            //Send to Matchin Engine
            var result = await _matchingEngineConnector.TransferAsync(offchainTransferId, sourceClient, destClient, assetId, amount);

            return result.ToDomainModel();
        }

        public async Task TransferAllAssetsToAddress(string clientId, string srcAddress, string destAddress)
        {
            var transactionId = Guid.NewGuid();

            var transfer =
                await
                    _transferEventsRepository.RegisterAsync(TransferEvent.CreateNewTransferAll(clientId, transactionId.ToString(),
                        srcAddress));

            var context = TransferContextData.Create(clientId, new TransferContextData.TransferModel
            {
                ClientId = clientId,
                OperationId = transfer.Id
            });

            await _bitCoinTransactionsRepository.CreateAsync(transactionId.ToString(), BitCoinCommands.Transfer,
                "", context.ToJson(), "");

            await _bitcoinCommandSender.SendCommand(new TransferAllCommand
            {
                Context = context.ToJson(),
                SourceAddress = srcAddress,
                DestinationAddress = destAddress,
                TransactionId = transactionId
            });
        }

        public async Task<ResultModel> DestroyAsync(string clientId,
            string addressFrom, string addressTo, double amount, string assetId)
        {
            var transactionId = Guid.NewGuid().ToString();

            //create transaction and place context
            var contextData = new UncolorContextData
            {
                ClientId = clientId,
                AssetId = assetId,
                Amount = amount,

                AddressFrom = addressFrom,
                AddressTo = addressTo,
                SignsClientIds = new[] { clientId }
            };
            await _bitCoinTransactionsRepository.CreateAsync(transactionId, BitCoinCommands.Destroy,
                "", contextData.ToJson(), "");

            //Send to Matching Engine
            var meResponse = await _matchingEngineConnector
                .CashInOutAsync(transactionId, clientId, assetId, -Math.Abs(amount));

            var result = meResponse.ToDomainModel();
            result.TransactionId = transactionId;

            return result;
        }
    }
}