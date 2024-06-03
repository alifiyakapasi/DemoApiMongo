using DemoApiMongo.Entities;
using DemoApiMongo.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace DemoApiMongo.Repository
{
    public class ProductRepo : IProductRepo
    {
       private readonly IMongoCollection<ProductDetails> _productCollection;

        public ProductRepo(
         IOptions<ProductDBSettings> productDatabaseSetting)
        {
            var mongoClient = new MongoClient(productDatabaseSetting.Value.ConnectionString);
            var database = mongoClient.GetDatabase(productDatabaseSetting.Value.DatabaseName);
           // _productCollection = database.GetCollection<ProductDetails>(productDatabaseSetting.Value.CollectionName);
            _productCollection = database.GetCollection<ProductDetails>("ProductDetails");
        }

        public async Task<List<ProductDetails>> ProductListAsync()
        {
            return await _productCollection.Find(_ => true).ToListAsync();
        }

        public async Task<ProductDetails> GetProductDetailByIdAsync(string productId)
        {
            return await _productCollection.Find(x => x.Id == productId).FirstOrDefaultAsync();
        }

        public async Task<List<ProductDetails>> GetProductDetailsByNameAsync(string name)
        {
            var filter = Builders<ProductDetails>.Filter.Regex(p => p.ProductName, name.ToLower());

            var result = await _productCollection.Find(filter).ToListAsync();
            return result;
        }

        public async Task AddProductAsync(ProductDetails productDetails)
        {
            await _productCollection.InsertOneAsync(productDetails);
        }

        public async Task UpdateProductAsync(string productId, ProductDetails productDetails)
        {
            await _productCollection.ReplaceOneAsync(x => x.Id == productId, productDetails);
        }

        public async Task<ProductDetails> UpdatePartialProductAsync(string productId, [FromBody] JsonPatchDocument<ProductDetails> patchDocument)
        {
            var filter = Builders<ProductDetails>.Filter.Eq(x => x.Id, productId);
            var update = Builders<ProductDetails>.Update.Combine((IEnumerable<UpdateDefinition<ProductDetails>>)patchDocument);

            var result = await _productCollection.FindOneAndUpdateAsync(filter, update);

            return result;
        }


        public async Task DeleteProductAsync(string productId)
        {
            await _productCollection.DeleteOneAsync(x => x.Id == productId);
        }
    }
}
