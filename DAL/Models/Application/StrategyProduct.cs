using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models.Application
{
    public class StrategyProduct
    {
        public int ProductId { get; set; }
        public ProductEntity Product { get; set; }

        public int StrategyId { get; set; }
        public StrategyEntity Strategy { get; set; }
    }
}
