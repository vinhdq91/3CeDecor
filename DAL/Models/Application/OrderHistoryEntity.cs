using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models.Application
{
    public class OrderHistoryEntity
    {
        public int Id { get; set; }
        [MaxLength(256)]
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int ActionId { get; set; }

        [StringLength(510)]
        public string Description { get; set; }
    }
}
