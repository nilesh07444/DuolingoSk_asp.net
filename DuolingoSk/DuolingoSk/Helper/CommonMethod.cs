using DuolingoSk.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DuolingoSk
{
    public static class CommonMethod
    {
        public static string ConvertFromUTC(DateTime? utcDateTime)
        {
            if (utcDateTime == null)
                return "";

            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime dateTimeAsTimeZone = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(utcDateTime), nzTimeZone);
            string dt = dateTimeAsTimeZone.ToString("dd/MM/yyyy hh:mm tt");
            return dt;
        }

        public static DateTime ConvertToUTC(DateTime dateTime, string timeZone)
        {
            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, nzTimeZone);
            return utcDateTime;
        }

        public static List<List<T>> Split<T>(this List<T> items, int sliceSize = 30)
        {
            List<List<T>> list = new List<List<T>>();
            for (int i = 0; i < items.Count; i += sliceSize)
                list.Add(items.GetRange(i, Math.Min(sliceSize, items.Count - i)));
            return list;
        }

        public static PaymentGatewayVM getPaymentGatewaykeys()
        {
            PaymentGatewayVM obj = new PaymentGatewayVM();

            bool IsPaymentLiveMode = Convert.ToBoolean(ConfigurationManager.AppSettings["IsPaymentLiveMode"]);

            if (IsPaymentLiveMode)
            {
                obj.PaymentAPIKey = Convert.ToString(ConfigurationManager.AppSettings["PaymentLiveKey"]);
                obj.PaymentSecretKey = Convert.ToString(ConfigurationManager.AppSettings["PaymentLiveAPISecret"]);
                obj.PaymentLayerUrl = Convert.ToString(ConfigurationManager.AppSettings["PaymentLiveLayerUrl"]);
                obj.PaymentPaymentTokenUrl = Convert.ToString(ConfigurationManager.AppSettings["PaymentLivePaymentTokenUrl"]);
            }
            else
            {
                obj.PaymentAPIKey = Convert.ToString(ConfigurationManager.AppSettings["PaymentTestKey"]);
                obj.PaymentSecretKey = Convert.ToString(ConfigurationManager.AppSettings["PaymentTestAPISecret"]);
                obj.PaymentLayerUrl = Convert.ToString(ConfigurationManager.AppSettings["PaymentTestLayerUrl"]);
                obj.PaymentPaymentTokenUrl = Convert.ToString(ConfigurationManager.AppSettings["PaymentTestPaymentTokenUrl"]);
            }

            return obj;
        }

        public static List<List<tbl_Package>> GetPackagesForSlider()
        {
            DuolingoSk_Entities _db = new DuolingoSk_Entities();

            List<tbl_Package> lstPackages = _db.tbl_Package.Where(x => x.IsActive && !x.IsDeleted).ToList();

            List<List<tbl_Package>> dataPackagesVMs = Split(lstPackages, 3);

            return dataPackagesVMs;
        }

    }
}