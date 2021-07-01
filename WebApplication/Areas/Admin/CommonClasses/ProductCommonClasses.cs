using DAL.Core.Enums;
using DAL.Core.Utilities;
using DAL.Dtos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Areas.Admin.CommonClasses
{
    public static class ProductCommonClasses
    {
        public static void SaveImageToTopic(int productType, List<ImageDto> imageIds)
        {
            switch (productType)
            {
                case (int)ProductTypeEnum.Featured:
                    foreach (var imgItem in imageIds)
                    {
                        if (!string.IsNullOrEmpty(imgItem.ImageName))
                        {
                            Bitmap bitmap = ImageResize.ResizeImage(imgItem.ImageName, 262, 345);
                            ImageResize.SaveImgToFolder("wwwroot/Resource/Featured", imgItem.ImageName, bitmap);
                        }
                    }
                    break;
                case (int)ProductTypeEnum.BestSelling:
                    foreach (var imgItem in imageIds)
                    {
                        if (!string.IsNullOrEmpty(imgItem.ImageName))
                        {
                            Bitmap bitmap = ImageResize.ResizeImage(imgItem.ImageName, 270, 360);
                            ImageResize.SaveImgToFolder("wwwroot/Resource/BestSelling", imgItem.ImageName, bitmap);
                        }
                    }
                    break;
                case (int)ProductTypeEnum.TopRate:
                    foreach (var imgItem in imageIds)
                    {
                        if (!string.IsNullOrEmpty(imgItem.ImageName))
                        {
                            Bitmap bitmap = ImageResize.ResizeImage(imgItem.ImageName, 126, 166);
                            ImageResize.SaveImgToFolder("wwwroot/Resource/TopRate", imgItem.ImageName, bitmap);
                        }
                    }
                    break;
                case (int)ProductTypeEnum.TopNew:
                    foreach (var imgItem in imageIds)
                    {
                        if (!string.IsNullOrEmpty(imgItem.ImageName))
                        {
                            Bitmap bitmap = ImageResize.ResizeImage(imgItem.ImageName, 126, 166);
                            ImageResize.SaveImgToFolder("wwwroot/Resource/TopNew", imgItem.ImageName, bitmap);
                        }
                    }
                    break;
                case (int)ProductTypeEnum.Related:
                    foreach (var imgItem in imageIds)
                    {
                        if (!string.IsNullOrEmpty(imgItem.ImageName))
                        {
                            Bitmap bitmap = ImageResize.ResizeImage(imgItem.ImageName, 126, 166);
                            ImageResize.SaveImgToFolder("wwwroot/Resource/Related", imgItem.ImageName, bitmap);
                        }
                    }
                    break;
                case (int)ProductTypeEnum.ProductList:
                    foreach (var imgItem in imageIds)
                    {
                        if (!string.IsNullOrEmpty(imgItem.ImageName))
                        {
                            Bitmap bitmap = ImageResize.ResizeImage(imgItem.ImageName, 262, 345);
                            ImageResize.SaveImgToFolder("wwwroot/Resource/ProductList", imgItem.ImageName, bitmap);
                        }
                    }
                    break;
            }
        }
    }
}
