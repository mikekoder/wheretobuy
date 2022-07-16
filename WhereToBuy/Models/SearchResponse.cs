using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhereToBuy.Models
{
    public class SearchResponse
    {
        public string StoreName { get; set; }
        public string StoreLocation { get; set; }
        public Product[] Products { get; set; }

        public class Product
        {
            public string Ean { get; set; }
            public string Name { get; set; }
            public decimal? Price { get; set; }
        }
    }
}
