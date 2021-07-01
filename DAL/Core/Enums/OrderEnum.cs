using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.Enums
{
    public enum OrderStatusEnum
    {
        NotConfirm = 0,  // Đã xác nhận
        Confirm = 1      // Chưa xác nhận
    }

    public enum OrderHistoryActionEnum
    {
        Confirm = 1,   // Thao tác xác nhận
        DisConfirm = 2,  // Thao tác bỏ xác nhận
        Remove = 3    // Thao tác xóa
    }
}
