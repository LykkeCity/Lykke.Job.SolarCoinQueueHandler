﻿using System;
using Lykke.SettingsReader.Attributes;
using Lykke.Service.ExchangeOperations.Client;

namespace Lykke.Job.SolarCoinQueueHandler
{
    public class AppSettings
    {
        public SolarCoinQueueHandlerSettings SolarCoinQueueHandlerJob { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public ExchangeOperationsServiceClientSettings ExchangeOperationsServiceClient { get; set; }

        public class SolarCoinQueueHandlerSettings
        {
            public DbSettings Db { get; set; }
            public HealthSettings Health { get; set; }
            [AzureQueueCheckAttribute]
            public string TriggerQueueConnectionString { get; set; }
        }

        public class DbSettings
        {
            [AzureTableCheck]
            public string LogsConnString { get; set; }
            [AzureTableCheck]
            public string BitCoinQueueConnectionString { get; set; }
            [AzureTableCheck]
            public string ClientPersonalInfoConnString { get; set; }
        }

        public class HealthSettings
        {
            public TimeSpan MaxMessageProcessingDuration { get; set; }
            public int MaxMessageProcessingFailedInARow { get; set; }
        }

        public class SlackNotificationsSettings
        {
            public AzureQueueSettings AzureQueue { get; set; }

            public int ThrottlingLimitSeconds { get; set; }
        }

        public class AzureQueueSettings
        {
            public string ConnectionString { get; set; }

            public string QueueName { get; set; }
        }
    }

    
}