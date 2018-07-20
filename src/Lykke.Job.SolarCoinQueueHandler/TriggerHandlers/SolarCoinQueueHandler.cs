using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.ExchangeOperations.Client;
using Lykke.Job.SolarCoinQueueHandler.Contract;
using Lykke.Job.SolarCoinQueueHandler.Core;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.PaymentSystems;
using Lykke.Job.SolarCoinQueueHandler.Core.Services;
using Lykke.JobTriggers.Triggers.Attributes;

namespace Lykke.Job.SolarCoinQueueHandler.TriggerHandlers
{
    public class SolarCoinQueueHandler
    {
        private readonly IWalletCredentialsRepository _walletCredentialsRepository;
        private readonly ILog _log;
        private readonly IExchangeOperationsServiceClient _exchangeOperationsService;
        private readonly IPaymentSystemsRawLog _paymentSystemsRawLog;
        private readonly IPaymentTransactionsRepository _paymentTransactionsRepository;
        private readonly IHealthService _healthService;
        private readonly IBcnClientCredentialsRepository _bcnClientCredentialsRepository;

        public SolarCoinQueueHandler(
            IWalletCredentialsRepository walletCredentialsRepository,
            ILog log,
            IExchangeOperationsServiceClient exchangeOperationsService,
            IPaymentSystemsRawLog paymentSystemsRawLog,
            IPaymentTransactionsRepository paymentTransactionsRepository,
            IHealthService healthService,
            IBcnClientCredentialsRepository bcnClientCredentialsRepository)
        {
            _walletCredentialsRepository = walletCredentialsRepository;
            _log = log;
            _exchangeOperationsService = exchangeOperationsService;
            _paymentSystemsRawLog = paymentSystemsRawLog;
            _paymentTransactionsRepository = paymentTransactionsRepository;
            _healthService = healthService;
            _bcnClientCredentialsRepository = bcnClientCredentialsRepository;
        }

        [QueueTrigger("solar-in")]
        public async Task ProcessInputMessage(SolarCashInMsg msg)
        {
            var logTask = _log.WriteInfoAsync("SolarCoinQueueHandler", "ProcessInMessage", msg.ToJson(), "Message received");
            
            try
            {
                _healthService.TraceMessageProcessingStarted();

                var bcnRecord = await _bcnClientCredentialsRepository.GetByAssetAddressAsync(msg.Address);
                var walletCreds = await _walletCredentialsRepository.GetBySolarCoinWalletAsync(msg.Address);
                string clientId = bcnRecord?.ClientId ?? walletCreds?.ClientId;
                if (string.IsNullOrEmpty(clientId))
                {
                    await _log.WriteWarningAsync("SolarCoinQueueHandler", "ProcessInMessage", msg.ToJson(), "Solar wallet not found");

                    _healthService.TraceMessageProcessingFailed();

                    return;
                }

                await _paymentSystemsRawLog.RegisterEventAsync(PaymentSystemRawLogEvent.Create(CashInPaymentSystem.SolarCoin, "Msg received", msg.ToJson()));

                var txId = $"{msg.TxId}_{msg.Address}";

                var pt = await _paymentTransactionsRepository.TryCreateAsync(PaymentTransaction.Create(
                    txId, CashInPaymentSystem.SolarCoin, clientId, msg.Amount,
                    LykkeConstants.SolarAssetId, LykkeConstants.SolarAssetId, null));

                if (pt == null)
                {
                    await _log.WriteWarningAsync("SolarCoinQueueHandler", "ProcessInMessage", msg.ToJson(), "Transaction already handled");

                    _healthService.TraceMessageProcessingFailed();

                    return;
                }

                var result = await _exchangeOperationsService.CashInAsync(clientId, LykkeConstants.SolarAssetId, msg.Amount);

                if (!result.IsOk())
                {
                    await _log.WriteWarningAsync("SolarCoinQueueHandler", msg.ToJson(), result.ToJson(), "ME error");

                    _healthService.TraceMessageProcessingFailed();

                    return;
                }

                await _paymentTransactionsRepository.SetAsOkAsync(pt.Id, msg.Amount, null);

                _healthService.TraceMessageProcessingCompleted();
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync("SolarCoinQueueHandler", "ProcessInMessage", msg.ToJson(), ex);

                _healthService.TraceMessageProcessingFailed();
            }
            finally
            {
                await logTask;
            }
        }
    }
}