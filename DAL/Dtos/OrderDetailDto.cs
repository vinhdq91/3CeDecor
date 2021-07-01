using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Dtos
{
    public class OrderDetailDto : AuditableEntity
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int Discount { get; set; }

        public int ProductEntityId { get; set; }
        public int OrderEntityId { get; set; }
    }
}
