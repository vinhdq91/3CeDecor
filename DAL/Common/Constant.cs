using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Common
{
    public class Constant
    {
        public static string ADMIN_GROUP = "ADMIN";
        public static string USER_SESSION = "USER_SESSION";
        public static string SESSION_CREDENTIALS = "SESSION_CREDENTIALS";
        public static string CartSession = "CartSession";
        public static string CurrentCulture { set; get; }

        #region Product
        public static string PRICEMIN = "0";
        public static string PRICEMAX = "10000000";
        public static int RELATEDPRODUCTMAX = 6;
        #endregion
    }
}
