using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace WhereToBuy.Foodie
{
    public class FoodieClient : IFoodieClient
    {
        private readonly string baseUrl = "https://www.foodie.fi";
        private readonly IMemoryCache cache;
        public FoodieClient(IMemoryCache cache)
        {
            this.cache = cache;
        }
        public async Task<IEnumerable<Store>> FindStores(string location)
        {
            using (var client = await CreateHttpClient())
            {
                var response = await client.GetAsync($"{baseUrl}/stores/?query={HtmlEncoder.Default.Encode(location)}");
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<StoreSearchResponse>(json);
                return result.Data.Stores.Select(s => new Store
                {
                    Id = s.Id,
                    Name = s.Name,
                    Address = s.StreetAddress,
                    PostalCode = s.PostalCode,
                    City = s.City,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude
                }).ToArray();
            }
        }
        public async Task<IEnumerable<Product>> FindProducts(string text, string storeId)
        {
            using (var client = await GetStoreClient(storeId))
            {
                var response = await client.GetAsync($"{baseUrl}/products/search2?term={HtmlEncoder.Default.Encode(text)}");
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ProductSearchResponse>(json);
                return result.Data.Entries.Select(e => new Product
                {
                    Ean = e.Ean,
                    Name = e.Name,
                    Quantity = e.Quantity,
                    Unit = e.Unit,
                    Price = e.Price,
                    ComparisonPrice = e.ComparisonPrice,
                    ComparisonUnit = e.ComparisonUnit
                });
            }
        }
        private async Task<HttpClient> GetStoreClient(string storeId)
        {
            var key = "foodie-session-store-" + storeId;
            if(cache.TryGetValue(key, out SessionDetails session))
            {
                return await CreateHttpClient(session);
            }
            session = await StartSession();

            cache.Set(key, session, TimeSpan.FromHours(12));

            var client = await CreateHttpClient(session, true);
            var response = await client.PostAsync($"{baseUrl}/store/select_store/{storeId}", new StringContent(""));

            return client;
        }
        private async Task<HttpClient> CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            return client;
        }
        private async Task<HttpClient> CreateHttpClient(SessionDetails session, bool addCsrfToken = false)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            client.DefaultRequestHeaders.Add("Cookie", session.Cookie);
            if (addCsrfToken)
            {
                client.DefaultRequestHeaders.Add("X-CSRF-Token", session.CsrfToken);
            }
            return client;
        }
        private async Task<SessionDetails> StartSession()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(baseUrl);
                var cookies = response.Headers.GetValues("Set-Cookie");
                var body = await response.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(body);
                var metas = doc.DocumentNode.SelectNodes("//meta");
                var csrfToken = metas.Where(m => m.GetAttributeValue("name", "") == "csrf-token").FirstOrDefault()?.GetAttributeValue("content", "");
                return new SessionDetails
                {
                    Cookie = cookies.FirstOrDefault(),
                    CsrfToken = csrfToken
                };
            }
        }

        
        
    }
}
