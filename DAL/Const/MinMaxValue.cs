using System;

namespace DAL.Const
{
    /// <summary>
    /// Đối tượng đóng gói các thông báo khi thêm, sửa, xóa...
    /// </summary>
    /// <modified>
    /// Author				    created date					comments
    /// duynv					05/07/2016				        Tạo mới
    ///</modified>
    public class MinMaxValue
    {
		public const long MinValuePrice = 0;
		public const long MaxValuePrice = 99000000000;
		public const int MinValueYear = 0;
        public static int MaxValueYear = DateTime.Now.Year;
        public const long StepCovertedPrice = 10000000;
    }
}