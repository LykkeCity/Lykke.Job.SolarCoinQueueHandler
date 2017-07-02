using System;

namespace Lykke.Job.SolarCoinQueueHandler.Core.Domain.PaymentSystems
{
    public static class PaymentTransactionExt
    {

        public static object GetInfo(this IPaymentTransaction src, Type expectedType = null, bool throwExeption = false)
        {

            if (!PaymentSystemsAndOtherInfo.PsAndOtherInfoLinks.ContainsKey(src.PaymentSystem))
            {
                if (throwExeption)
                    throw new Exception("Unsupported payment system for reading other info: transactionId:" + src.Id);

                return null;
            }


            var type = PaymentSystemsAndOtherInfo.PsAndOtherInfoLinks[src.PaymentSystem];

            if (expectedType != null)
            {
                if (type != expectedType)
                    throw new Exception("Payment system and Other info does not match for transactionId:" + src.Id);
            }


            return Newtonsoft.Json.JsonConvert.DeserializeObject(src.Info, type);

        }

        public static T GetInfo<T>(this IPaymentTransaction src)
        {
            return (T)GetInfo(src, typeof(T), true);
        }


        public static bool AreMoneyOnOurAccount(this IPaymentTransaction src)
        {
            return src.Status == PaymentStatus.NotifyProcessed;
        }
    }
}