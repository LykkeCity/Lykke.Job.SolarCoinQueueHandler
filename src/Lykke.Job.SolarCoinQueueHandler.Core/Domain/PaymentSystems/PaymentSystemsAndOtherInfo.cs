using System;
using System.Collections.Generic;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.PaymentSystems.CreaditVaucher;

namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.PaymentSystems
{
    public static class PaymentSystemsAndOtherInfo
    {

        public static readonly Dictionary<CashInPaymentSystem, Type> PsAndOtherInfoLinks = new Dictionary<CashInPaymentSystem, Type>
        {
            [CashInPaymentSystem.CreditVoucher] = typeof(CreditVoucherOtherPaymentInfo)
        };
    }
}