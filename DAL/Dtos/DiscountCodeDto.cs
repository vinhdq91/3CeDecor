using DAL.Models;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Dtos
{
    public class DiscountCodeDto : AuditableEntity
    {
        public int Id { get; set; }
        public string DiscountCodeName { get; set; }
        public int DiscountRate { get; set; }
        public DateTime ExpireDate { get; set; }
        public int NumberLeft { get; set; }
        public int Status { get; set; }

        public virtual List<OrderEntity> Orders { get; set; }
    }
}
