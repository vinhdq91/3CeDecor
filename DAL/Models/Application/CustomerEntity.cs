using DAL.Core.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.Application
{
    public class CustomerEntity: AuditableEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [DisplayName("Name")]
        [StringLength(160)]
        public string Name { get; set; }
        public int Gender { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [DisplayName("Email Address")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
        ErrorMessage = "Email is is not valid.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(70)]
        public string Address { get; set; }

        [Required(ErrorMessage = "District is required")]
        [StringLength(70)]
        public string DistrictCode { get; set; }

        [Required(ErrorMessage = "Province is required")]
        [StringLength(70)]
        public string ProvinceCode { get; set; }

        [StringLength(70)]
        public string WardCode { get; set; }

        [StringLength(6)]
        public string DiscountCode { get; set; }
        public decimal ShippingFee { get; set; }

        public virtual List<OrderEntity> Orders { get; set; }
        public virtual List<ImageEntity> ImageIds { get; set; }

    }
}
