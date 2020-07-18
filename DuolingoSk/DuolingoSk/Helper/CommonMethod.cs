using System;
using System.Collections.Generic;
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
          
    }
}