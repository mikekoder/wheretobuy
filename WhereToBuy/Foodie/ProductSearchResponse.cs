using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhereToBuy.Foodie
{
    internal class ProductSearchResponse
    {
        public ProductSearchResponseData Data { get; set; }
    }
    internal class ProductSearchResponseData
    {
        public ProductResponse[] Entries { get; set; }
    }
    internal class ProductResponse
    {
        [JsonProperty("ean")]
        public string Ean { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("subname")]
        public string Subname { get; set; }
        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }
        [JsonProperty("unit_name")]
        public string Unit { get; set; }
        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("comp_price")]
        public decimal ComparisonPrice { get; set; }
        [JsonProperty("comp_unit")]
        public string ComparisonUnit { get; set; }
    }
}
