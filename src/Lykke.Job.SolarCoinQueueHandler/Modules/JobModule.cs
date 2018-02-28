using System;
using Autofac;
using AzureStorage.Tables;
using AzureStorage.Tables.Templates.Index;
using Common.Log;
using Lykke.SettingsReader;
using Lykke.Service.ExchangeOperations.Contracts;
using Lykke.Service.ExchangeOperations.Client;
using Lykke.Job.SolarCoinQueueHandler.AzureRepositories.BitCoin;
using Lykke.Job.SolarCoinQueueHandler.AzureRepositories.PaymentSystems;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.PaymentSystems;
using Lykke.Job.SolarCoinQueueHandler.Core.Services;
using Lykke.Job.SolarCoinQueueHandler.Services;

namespace Lykke.Job.SolarCoinQueueHandler.Modules
{
    public class JobModule : Module
    {
        private readonly AppSettings _settings;
        private readonly IReloadingManager<AppSettings.DbSettings> _dbManager;
        private readonly ILog _log;

        public JobModule(AppSettings settings, IReloadingManager<AppSettings> settingsManager, ILog log)
        {
            _settings = settings;
            _dbManager = settingsManager.Nested(s => s.SolarCoinQueueHandlerJob.Db);
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settings)
                .SingleInstance();

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterInstance<IHealthService>(new HealthService(
                allowIdling: true,
                maxHealthyMessageProcessingDuration: _settings.SolarCoinQueueHandlerJob.Health.MaxMessageProcessingDuration,
                maxHealthyMessageProcessingFailedInARow: _settings.SolarCoinQueueHandlerJob.Health.MaxMessageProcessingFailedInARow,
                maxHealthyMessageProcessingIdleDuration: TimeSpan.Zero));

            RegisterAzureRepositories(builder);
            RegisterServices(builder, _settings);
        }

        private static void RegisterServices(ContainerBuilder builder, AppSettings appSettings)
        {
            var exchangeOperationsService = new ExchangeOperationsServiceClient(appSettings.ExchangeOperationsServiceClient.ServiceUrl);
            builder.RegisterInstance(exchangeOperationsService).As<IExchangeOperationsServiceClient>().SingleInstance();
        }

        private void RegisterAzureRepositories(ContainerBuilder builder)
        {
            builder.RegisterInstance<IBitCoinTransactionsRepository>(
                new BitCoinTransactionsRepository(
                    AzureTableStorage<BitCoinTransactionEntity>.Create(_dbManager.ConnectionString(s => s.BitCoinQueueConnectionString), "BitCoinTransactions", _log)));

            builder.RegisterInstance<IWalletCredentialsRepository>(
                new WalletCredentialsRepository(
                    AzureTableStorage<WalletCredentialsEntity>.Create(_dbManager.ConnectionString(s => s.ClientPersonalInfoConnString), "WalletCredentials", _log)));


            builder.RegisterInstance<IPaymentTransactionsRepository>(
                new PaymentTransactionsRepository(
                    AzureTableStorage<PaymentTransactionEntity>.Create(_dbManager.ConnectionString(s => s.ClientPersonalInfoConnString), "PaymentTransactions", _log), 
                    AzureTableStorage<AzureMultiIndex>.Create(_dbManager.ConnectionString(s => s.ClientPersonalInfoConnString), "PaymentTransactions", _log)));

            builder.RegisterInstance<IPaymentSystemsRawLog>(
                new PaymentSystemsRawLog(
                    AzureTableStorage<PaymentSystemRawLogEventEntity>.Create(_dbManager.ConnectionString(s => s.LogsConnString), "PaymentSystemsLog", _log)));
        }
    }
}