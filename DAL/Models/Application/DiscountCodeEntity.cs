using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models.Application
{
    public class DiscountCodeEntity : AuditableEntity
    {
        public int Id { get; set; }

        [StringLength(20)]
        public string DiscountCodeName { get; set; }
        public int DiscountRate { get; set; }
        public DateTime ExpireDate { get; set; }
        public int NumberLeft { get; set; }
        public int Status { get; set; }

        [StringLength(510)]
        public string Description { get; set; }
        public virtual List<OrderEntity> Orders { get; set; }
    }
}
