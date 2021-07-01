using DAL.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Dtos
{
    public class OrderHistoryDto
    {
        [MaxLength(256)]
        public int Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int ActionId { get; set; }
        public string ActionName
        {
            get
            {
                string actionName = string.Empty;
                if (ActionId > 0)
                {
                    switch (ActionId)
                    {
                        case (int)OrderHistoryActionEnum.Confirm:
                            actionName = "Đã xác nhận";
                            break;
                        case (int)OrderHistoryActionEnum.DisConfirm:
                            actionName = "Bỏ xác nhận";
                            break;
                        case (int)OrderHistoryActionEnum.Remove:
                            actionName = "Đã xóa";
                            break;
                    }
                }
                return actionName;
            }
        }
        public string Description { get; set; }
    }
}
