using System;
using Lykke.Job.SolarCoinQueueHandler.Core.Services;

namespace Lykke.Job.SolarCoinQueueHandler.Services
{
    public class HealthService : IHealthService
    {
        public DateTime LastMessageProcessingStartedMoment { get; private set; }
        public TimeSpan LastMessageProcessingDuration { get; private set; }
        public TimeSpan MaxHealthyMessageProcessingDuration { get; }
        public int MaxHealthyMessageProcessingFailedInARow { get; }
        public TimeSpan MaxHealthyMessageProcessingIdleDuration { get; }
        public int MessageProcessingFailedInARow { get; private set; }
        public TimeSpan MessageProcessingIdleDuration => DateTime.UtcNow - LastMessageProcessingStartedMoment;

        private bool WasLastMessageProcessingCompleted { get; set; }
        private bool WasMessageProcessingEverStarted { get; set; }

        private readonly bool _allowIdling;

        public HealthService(bool allowIdling, TimeSpan maxHealthyMessageProcessingDuration, int maxHealthyMessageProcessingFailedInARow, TimeSpan maxHealthyMessageProcessingIdleDuration)
        {
            _allowIdling = allowIdling;
            MaxHealthyMessageProcessingDuration = maxHealthyMessageProcessingDuration;
            MaxHealthyMessageProcessingFailedInARow = maxHealthyMessageProcessingFailedInARow;
            MaxHealthyMessageProcessingIdleDuration = maxHealthyMessageProcessingIdleDuration;
        }

        public string GetHealthViolationMessage()
        {
            var failedInARow = MessageProcessingFailedInARow;
            if (failedInARow > MaxHealthyMessageProcessingFailedInARow)
            {
                return $"Message processing has failed last {failedInARow} times in a row";
            }

            var lastDuration = LastMessageProcessingDuration;
            if (lastDuration > MaxHealthyMessageProcessingDuration)
            {
                return $"Last message processing was lasted for {lastDuration}, which is too long";
            }

            if (!_allowIdling)
            {
                if (!WasMessageProcessingEverStarted)
                {
                    return "Waiting for first message processing started";
                }

                if (!WasLastMessageProcessingCompleted && MessageProcessingFailedInARow == 0 && WasMessageProcessingEverStarted)
                {
                    return
                        $"Waiting {DateTime.UtcNow - LastMessageProcessingStartedMoment} for first message processing completed";
                }

                var idleDuration = MessageProcessingIdleDuration;
                if (idleDuration > MaxHealthyMessageProcessingIdleDuration)
                {
                    return $"Message processing is idle for {idleDuration}, which is too long";
                }
            }

            return null;
        }

        public void TraceMessageProcessingStarted()
        {
            LastMessageProcessingStartedMoment = DateTime.UtcNow;
            WasMessageProcessingEverStarted = true;
        }

        public void TraceMessageProcessingCompleted()
        {
            LastMessageProcessingDuration = DateTime.UtcNow - LastMessageProcessingStartedMoment;
            MessageProcessingFailedInARow = 0;
            WasLastMessageProcessingCompleted = true;
        }

        public void TraceMessageProcessingFailed()
        {
            MessageProcessingFailedInARow++;
            WasLastMessageProcessingCompleted = false;
        }
    }
}