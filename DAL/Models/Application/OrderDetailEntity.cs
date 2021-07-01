using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.Application
{
    public class OrderDetailEntity : AuditableEntity
    {
        public int Id { get; set; }
        [StringLength(110)]
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int Discount { get; set; }

        public int? ProductEntityId { get; set; }
        public int OrderEntityId { get; set; }
    }
}
