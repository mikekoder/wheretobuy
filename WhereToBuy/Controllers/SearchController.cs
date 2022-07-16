using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WhereToBuy.Foodie;
using WhereToBuy.KRuoka;
using WhereToBuy.Models;

namespace WhereToBuy.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        private readonly IKRuokaClient kruokaClient;
        private readonly IFoodieClient foodieClient;

        public SearchController(IKRuokaClient kruokaClient, IFoodieClient foodieClient)
        {
            this.kruokaClient = kruokaClient;
            this.foodieClient = foodieClient;
        }
        [HttpGet("")]
        public async Task<IActionResult> Search(string text, string location)
        {
            var clients = new IStoreClient[] { kruokaClient, foodieClient };
            var results = new System.Collections.Concurrent.ConcurrentBag<SearchResponse>();
            var tasks = new List<Task>();
            foreach(var client in clients)
            {
                var stores = await client.FindStores(location);
                foreach (var store in stores)
                {
                    tasks.Add(Task.Factory.StartNew(() => {
                        var products = client.FindProducts(text, store.Id).Result;
                        if (products.Any(p => p.Price > 0m))
                        {
                            results.Add(new SearchResponse
                            {
                                StoreName = store.Name,
                                Products = products.Select(p => new SearchResponse.Product
                                {
                                    Ean = p.Ean,
                                    Name = p.Name,
                                    Price = p.Price
                                }).ToArray()
                            });
                        }
                    }));
                    
                }
            }
            Task.WaitAll(tasks.ToArray());

            return Ok(results);
        }
    }
}
