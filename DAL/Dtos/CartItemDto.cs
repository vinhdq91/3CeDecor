using DAL.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models.Application
{
    public class CartItemDto : AuditableEntity
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public virtual ProductDto Product { get; set; }
    }
}
