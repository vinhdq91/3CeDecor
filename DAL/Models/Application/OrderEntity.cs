using DAL.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models.Application
{
    public class OrderEntity : AuditableEntity
    {
        public int Id { get; set; }

        [StringLength(110)]
        public string OrderCode { get; set; }
        public int Discount { get; set; }

        public string DiscountCode { get; set; }

        [StringLength(110)]
        public string Comments { get; set; }

        [StringLength(110)]
        public string PaymentMethod { get; set; }

        [StringLength(110)]
        public string BankCode { get; set; }
        public int Status { get; set; }

        [StringLength(510)]
        public string Description { get; set; }

        public int CashierId { get; set; }
        //public int CashierId1 { get; set; }   // Tạo nhầm, không xóa được vì lỗi MySqlException
        public ApplicationUser Cashier { get; set; }

        public int CustomerId { get; set; }
        public CustomerEntity Customer { get; set; }

        public int DiscountCodeId { get; set; }
        public DiscountCodeEntity DiscountCodeEntity { get; set; }
        public virtual List<OrderDetailEntity> OrderDetails { get; set; }
    }
}
