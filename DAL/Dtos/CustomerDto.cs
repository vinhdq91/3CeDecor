using DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Dtos
{
    public class CustomerDto : AuditableEntity
    {
        public string Name { get; set; }
        public int Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string District { get; set; }
        public string Province { get; set; }

        public virtual List<OrderDto> Orders { get; set; }
        public virtual List<ImageDto> ImageIds { get; set; }
    }
}
