using DemoApiMongo.Entities.DataModels;
using DemoApiMongo.Entities.ViewModels;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApiMongo.Repository
{
    public interface IProductRepo
    {
        public Task<List<ProductDetails>> ProductListAsync();

        public Task<ProductDetails> GetProductDetailByIdAsync(string productId);

        public Task<List<ProductDetails>> GetProductDetailsByNameAsync(string name);

        public Task AddProductAsync(ProductDetailModel productDetails);

        public Task UpdateProductAsync(string productId, ProductDetails productDetails);

        // public Task UpdatePartialProductAsync(string productId, ProductDetails productDetails);
       public Task<ProductDetails> UpdatePartialProductAsync(string productId, JsonPatchDocument<ProductDetails> patchDocument);


        public Task DeleteProductAsync(String productId);
    }
}
