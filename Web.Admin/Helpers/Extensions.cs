#region

using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

#endregion

namespace Web.Admin.Helpers
{
    public static class Extensions
    {
        public static string ImageOf(this HtmlHelper h, string name)
        {
            string path = VirtualPathUtility.ToAbsolute("~/content/");
            return path + name;
        }

        public static string GetText(this SelectList list, string value)
        {
            SelectListItem item = list.SingleOrDefault(x => x.Value == value);
            return item == null ? "" : item.Text;
        }

        public static DateTime Lo(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }

        public static DateTime Hi(this DateTime date)
        {
            return date.Lo().AddDays(1);
        }

        public static DateTime MonthLo(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day).AddDays(-(date.Day - 1));
        }

        public static DateTime MonthHi(this DateTime dateTime)
        {
            return dateTime.MonthLo().AddMonths(1);
        }

        public static DateTime YearLo(this DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }

        public static DateTime YearHi(this DateTime dateTime)
        {
            return dateTime.YearLo().AddYears(1);
        }

        public static string Difference(this DateTime date)
        {
            TimeSpan ts = DateTime.Now - date;
            return string.Format("{0} days {1} hours {2} minutes {3} seconds", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
        }

        public static string TextSummary(this string s, int len)
        {
            return s.Length > len ? s.Substring(0, len) + "..." : s;
        }
    }
}