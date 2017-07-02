using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using AzureStorage.Tables.Templates.Index;
using Common.Log;
using Lykke.Job.SolarCoinQueueHandler.AzureRepositories.Assets;
using Lykke.Job.SolarCoinQueueHandler.AzureRepositories.BitCoin;
using Lykke.Job.SolarCoinQueueHandler.AzureRepositories.CacheOperations;
using Lykke.Job.SolarCoinQueueHandler.AzureRepositories.PaymentSystems;
using Lykke.Job.SolarCoinQueueHandler.Core;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.Assets;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.BitCoin;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.CachOperations;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.PaymentSystems;
using Lykke.Job.SolarCoinQueueHandler.Core.Services;
using Lykke.Job.SolarCoinQueueHandler.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Job.SolarCoinQueueHandler.Modules
{
    public class JobModule : Module
    {
        private readonly AppSettings.SolarCoinQueueHandlerSettings _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public JobModule(AppSettings.SolarCoinQueueHandlerSettings settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settings)
                .SingleInstance();

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(TimeSpan.FromSeconds(30)));

            // NOTE: You can implement your own poison queue notifier. See https://github.com/LykkeCity/JobTriggers/blob/master/readme.md
            // builder.Register<PoisionQueueNotifierImplementation>().As<IPoisionQueueNotifier>();

            RegisterAzureRepositories(builder, _settings.Db);

            builder.Populate(_services);
        }

        private void RegisterAzureRepositories(ContainerBuilder builder, AppSettings.DbSettings settings)
        {
            builder.RegisterInstance<IAssetsRepository>(
                new AssetsRepository(new AzureTableStorage<AssetEntity>(settings.DictsConnString, "Dictionaries", _log)));

            builder.RegisterInstance<IBitCoinTransactionsRepository>(
                new BitCoinTransactionsRepository(
                    new AzureTableStorage<BitCoinTransactionEntity>(settings.BitCoinQueueConnectionString, "BitCoinTransactions", _log)));

            builder.RegisterInstance<IWalletCredentialsHistoryRepository>(
                new WalletCredentialsHistoryRepository(
                    new AzureTableStorage<WalletCredentialsHistoryRecord>(settings.ClientPersonalInfoConnString, "WalletCredentialsHistory", _log)));

            builder.RegisterInstance<IWalletCredentialsRepository>(
                new WalletCredentialsRepository(
                    new AzureTableStorage<WalletCredentialsEntity>(settings.ClientPersonalInfoConnString, "WalletCredentials", _log)));

            builder.RegisterInstance<ITransferEventsRepository>(
                new TransferEventsRepository(
                    new AzureTableStorage<TransferEventEntity>(settings.ClientPersonalInfoConnString, "Transfers", _log),
                    new AzureTableStorage<AzureIndex>(settings.ClientPersonalInfoConnString, "Transfers", _log)));

            builder.RegisterInstance<IPaymentTransactionsRepository>(
                new PaymentTransactionsRepository(
                    new AzureTableStorage<PaymentTransactionEntity>(settings.ClientPersonalInfoConnString, "PaymentTransactions", _log), 
                    new AzureTableStorage<AzureMultiIndex>(settings.ClientPersonalInfoConnString, "PaymentTransactions", _log)));
        }
    }
}