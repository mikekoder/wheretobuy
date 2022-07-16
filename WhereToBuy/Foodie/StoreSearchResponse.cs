using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhereToBuy.Foodie
{
    internal class StoreSearchResponse
    {
        public StoreSearchResponseData Data { get; set; }
    }
    internal class StoreSearchResponseData
    {
        public StoreResponse[] Stores { get; set; }
    }
    internal class StoreResponse
    {
        [JsonProperty("guid")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("lat")]
        public decimal Latitude { get; set; }
        [JsonProperty("lon")]
        public decimal Longitude { get; set; }
        [JsonProperty("street_address")]
        public string StreetAddress { get; set; }
        [JsonProperty("post_code")]
        public string PostalCode { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("chain_name")]
        public string Chain { get; set; }
    }
}
