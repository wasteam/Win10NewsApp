using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows10News
{
    public static class DateTimeExtensions
    {
        public static DateTime SafeType(this DateTime? dt)
        {
            if (dt.HasValue)
            {
                return dt.Value;
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        public static DateTime SafeType(this DateTime dt)
        {
            return dt;
        }

        public static string ToString(this DateTime? dt, DateTimeFormat format)
        {
            if (dt.HasValue)
            {
                return dt.Value.ToString(format);
            }
            else
            {
                return string.Empty;
            }
        }

        public static string ToString(this DateTime dt, DateTimeFormat format)
        {
            if (dt == DateTime.MinValue || dt == DateTime.MaxValue)
            {
                return string.Empty;
            }
            switch (format)
            {
                case DateTimeFormat.ShortDate:
                    return dt.ToString("g");
                case DateTimeFormat.LongDate:
                    return dt.ToString("D");
                case DateTimeFormat.Date:
                    return dt.ToString("d");
                case DateTimeFormat.Time:
                    return dt.ToString("t");
                case DateTimeFormat.DayOfWeek:
                    return dt.GetLocalizedDayOfWeek();
                case DateTimeFormat.CardDate:
                    return dt.ToString("dd MMM");
                case DateTimeFormat.CardTime:
                    return dt.ToString("HH mm");
                default:
                    return string.Empty;
            }
        }

        public static string GetLocalizedDayOfWeek(this DateTime dt)
        {
            return CultureInfo.CurrentUICulture.DateTimeFormat.DayNames[(int)dt.DayOfWeek];
        }
    }

    public enum DateTimeFormat
    {
        ShortDate,
        LongDate,
        Date,
        Time,
        DayOfWeek,
        CardDate,
        CardTime
    }
}
