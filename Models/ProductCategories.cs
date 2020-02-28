using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IShops.Models
{
    public class ProductCategories
    {
        public int ProductId { get; set; }
        public ProductsModel Product { get; set; }

        public int CategoryId { get; set; }
        public Category Category {get; set;}
    }
}
