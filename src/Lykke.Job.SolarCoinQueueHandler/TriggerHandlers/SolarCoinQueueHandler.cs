using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Job.SolarCoinQueueHandler.Contract;
using Lykke.Job.SolarCoinQueueHandler.Core;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.PaymentSystems;
using Lykke.Job.SolarCoinQueueHandler.Services.Exchange;
using Lykke.JobTriggers.Triggers.Attributes;

namespace Lykke.Job.SolarCoinQueueHandler.TriggerHandlers
{
    public class SolarCoinQueueHandler
    {
        private readonly IWalletCredentialsRepository _walletCredentialsRepository;
        private readonly ILog _log;
        private readonly ExchangeOperationsService _exchangeOperationsService;
        private readonly IPaymentSystemsRawLog _paymentSystemsRawLog;
        private readonly IPaymentTransactionsRepository _paymentTransactionsRepository;

        public SolarCoinQueueHandler(IWalletCredentialsRepository walletCredentialsRepository, ILog log,
            ExchangeOperationsService exchangeOperationsService, IPaymentSystemsRawLog paymentSystemsRawLog,
            IPaymentTransactionsRepository paymentTransactionsRepository)
        {
            _walletCredentialsRepository = walletCredentialsRepository;
            _log = log;
            _exchangeOperationsService = exchangeOperationsService;
            _paymentSystemsRawLog = paymentSystemsRawLog;
            _paymentTransactionsRepository = paymentTransactionsRepository;
        }

        [QueueTrigger("solar-in")]
        public async Task ProcessInMessage(SolarCashInMsg msg)
        {
            var logTask = _log.WriteInfoAsync("SolarCoinQueueHandler", "ProcessInMessage", msg.ToJson(),
                "Message received");

            try
            {
                var walletCreds = await _walletCredentialsRepository.GetBySolarCoinWalletAsync(msg.Address);
                if (walletCreds == null)
                {
                    await
                        _log.WriteWarningAsync("SolarCoinQueueHandler", "ProcessInMessage", msg.ToJson(),
                            "Solar wallet not found");
                    return;
                }

                await
                    _paymentSystemsRawLog.RegisterEventAsync(
                        PaymentSystemRawLogEvent.Create(CashInPaymentSystem.SolarCoin, "Msg received",
                            msg.ToJson()));

                var txId = $"{msg.TxId}_{msg.Address}";

                var pt = await _paymentTransactionsRepository.TryCreateAsync(PaymentTransaction.Create(
                    txId, CashInPaymentSystem.SolarCoin, walletCreds.ClientId, msg.Amount,
                    LykkeConstants.SolarAssetId, LykkeConstants.SolarAssetId, null));

                if (pt == null)
                {
                    await
                        _log.WriteWarningAsync("SolarCoinQueueHandler", "ProcessInMessage", msg.ToJson(),
                            "Transaction already handled");
                    //return if was handled previously
                    return;
                }

                var result = await
                    _exchangeOperationsService.IssueAsync(walletCreds.ClientId, LykkeConstants.SolarAssetId,
                        msg.Amount);

                if (!result.IsOk())
                {
                    await
                        _log.WriteWarningAsync("SolarCoinQueueHandler", msg.ToJson(), result.ToJson(),
                            "ME error");
                    return;
                }

                await
                    _paymentTransactionsRepository.SetAsOkAsync(pt.Id, msg.Amount, null);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync("SolarCoinQueueHandler", "ProcessInMessage", msg.ToJson(), ex);
            }
            finally
            {
                await logTask;
            }
        }
    }
}