using DAL.Models;
using DAL.Models.Account;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Dtos
{
    public class OrderDto : AuditableEntity
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public int Discount { get; set; }
        public string DiscountCode { get; set; }
        public string Comments { get; set; }
        public string PaymentMethod { get; set; }
        public string BankCode { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }

        public int CashierId { get; set; }
        public ApplicationUser Cashier { get; set; }

        public int CustomerId { get; set; }
        public CustomerEntity Customer { get; set; }

        public DiscountCodeEntity DiscountCodeEntity { get; set; }
        public virtual List<OrderDetailDto> OrderDetails { get; set; }
    }
}
