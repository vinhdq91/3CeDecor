using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.Enums
{
    public enum ProductTypeEnum
    {
        Normal = 0,
        Featured = 1,  // Sản phẩm nổi bật
        BestSelling = 2, // Sản phẩm bán chạy
        TopRate = 3,  // Sản phẩm được bình chọn
        TopNew = 4,  // Sản phẩm mới nổi bật
        Recent = 5,   // Sản phẩm mới nhất (ngoài nổi bật)
        Related = 6,   // Sản phẩm liên quan
        ProductList = 7  // Sản phẩm hiển thị ở trang danh sách
    }

    public enum SortProductEnum
    {
        All = 0,
        PublishDateASC = 1,
        PublishDateDESC = 2,
        PriceASC = 3,
        PriceDESC = 4,
        ViewCountASC = 5,
        ViewCountDESC = 6,
    }

    public enum ProductCategoryEnum
    {
        All = 0,
        HomeDecor = 1,
        RestaurantDecor = 2,
        KidToys = 3,
    }

    public enum ProductStatus
    {
        Active = 1, // Xuất bản
        UnActive = 2, // Chưa xuất bản
        OutDate = 3, // Hết hạn
    }
}
