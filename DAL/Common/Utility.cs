using DAL.Const;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DAL.Common
{
    public static class Common
    {
        public static string FomatPrice(decimal price)
        {
            if (price == MinMaxValue.MaxValuePrice) return string.Empty;
            string s = price.ToString();
            string stmp = s;
            int amount;
            amount = (int)(s.Length / 3);
            if (s.Length % 3 == 0)
                amount--;
            for (int i = 1; i <= amount; i++)
            {
                stmp = stmp.Insert(stmp.Length - 4 * i + 1, ".");
            }
            if (stmp.Contains("..00"))
                stmp = stmp.Replace("..00", "");
            return stmp + " VND";
        }

        public static string RemoveHTMLTag(string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }
    }
}
