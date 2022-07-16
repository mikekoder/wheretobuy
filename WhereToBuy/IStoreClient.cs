using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhereToBuy
{
    public interface IStoreClient
    {
        Task<IEnumerable<Store>> FindStores(string location);
        Task<IEnumerable<Product>> FindProducts(string text, string storeId);
    }
}
