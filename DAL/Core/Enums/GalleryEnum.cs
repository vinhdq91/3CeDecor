using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.Enums
{
    public enum GalleryWidthEnum
    {
        Width1 = 360,
    }

    public enum GalleryHeightEnum
    {
        Height1 = 493,
        Height2 = 246
    }

    public enum GalleryType
    {
        GalleryImage = 1,
        HomeSlide = 2,
        HomeVideo = 3
    }

    public enum GalleryStatus
    {
        NotActive = 0,
        Active = 1
    }
}
