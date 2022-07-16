using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace WhereToBuy.KRuoka
{
    public class KRuokaClient : IKRuokaClient
    {
        private readonly string baseUrl = "https://www.k-ruoka.fi/kr-api";
        public async Task<IEnumerable<Store>> FindStores(string location)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{baseUrl}/stores/search?query={HtmlEncoder.Default.Encode(location)}&onlyPickups=false");
                //var json = await response.Content.ReadAsStringAsync();
                //var stores = JsonConvert.DeserializeObject<StoreResponse[]>(json);
                var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                var stores = json["results"].ToObject<StoreResponse[]>();
                
                return stores.Select(s => new Store
                {
                    Id = s.Id,
                    Name = s.Name,
                    Latitude = s.Geo.Latitude,
                    Longitude = s.Geo.Longitude
                }).ToArray();
            }
        }
        public async Task<IEnumerable<Product>> FindProducts(string text, string storeId)
        {
            var products = new List<ProductResponse>();
            using (var client = new HttpClient())
            {
                while (true)
                {
                    var response = await client.PostAsync($"{baseUrl}/product-search/{text}?storeId={storeId}&offset={products.Count}", new StringContent(""));
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ProductSearchResult>(json);
                    products.AddRange(result.Result);
                    if (products.Count >= result.TotalHits)
                    {
                        break;
                    }
                }
            }

            foreach (var product in products)
            {
                try
                {
                    decimal.Parse(product.VisiblePrice.Value, CultureInfo.InvariantCulture);
                    decimal.Parse(product.ReferencePrice.Value, CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    ;
                }
            }
            return products.Select(p => new Product
            {
                Ean = p.Ean,
                Name = p.Name,
                Price = p.VisiblePrice != null && !string.IsNullOrWhiteSpace(p.VisiblePrice.Value) ?  decimal.Parse(p.VisiblePrice.Value, CultureInfo.InvariantCulture) : 0m,
                Unit = p.ContentUnit,
                Quantity = p.ContentSize,
                ComparisonPrice = p.ReferencePrice != null && !string.IsNullOrWhiteSpace(p.ReferencePrice.Value) ? decimal.Parse(p.ReferencePrice.Value, CultureInfo.InvariantCulture) : 0m,
                ComparisonUnit = p.ContentUnit
            }).ToArray();
        }
        private class ProductSearchResult
        {
            public ProductResponse[] Result { get; set; }
            public int TotalHits { get; set; }
        }
    }
    
}
