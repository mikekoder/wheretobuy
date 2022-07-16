using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhereToBuy
{
    public class Product
    {
        public string Ean { get; set; }
        public string Name { get; set; }

        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }

        public decimal ComparisonPrice { get; set; }
        public string ComparisonUnit { get; set; }

        // Osasto
        // Sijainti (hyllyväli)
    }
}
