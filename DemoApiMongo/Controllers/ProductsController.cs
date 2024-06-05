using DemoApiMongo.Filter;
using DemoApiMongo.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;
using System.Collections;
using DemoApiMongo.Entities.DataModels;
using DemoApiMongo.Entities.ViewModels;
using Microsoft.Extensions.Caching.Memory;

namespace DemoApiMongo.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [LogActionFilter]

    public class ProductsController : ControllerBase
    {

        private readonly IProductRepo productService;
        private readonly ILogger<ProductsController> logger;
        private readonly IMemoryCache _memoryCache;
        public string cacheKey = "product";

        public ProductsController(IProductRepo productRepo, ILogger<ProductsController> logger, IMemoryCache memoryCache)
        {
            this.productService = productRepo;
            this.logger = logger;
            this._memoryCache = memoryCache;
        }


        [HttpGet]
        public async Task<List<ProductDetails>> Get()
        {
            /// Below All are Exception Methods

            /// DivideByZeroException: Attempted to divide by zero
            //var x = 1 / Convert.ToInt32("0", CultureInfo.InvariantCulture); 

            /// Throws NullReferenceException
            //string myString = null;
            //string result = myString.ToUpper(); 

            /// ArgumentOutOfRangeException - max length of array is 1, but we try to print index 2 so exception occurs. 
            //ArrayList lis = new ArrayList();
            //lis.Add("A");
            //lis.Add("B");
            //Console.WriteLine(lis[2]);

            /// FormatException is thrown when compiled since we have passed a value other than integer
            //string str = "3.5";
            //int res = int.Parse(str);


            ///OverflowException is thrown since we have passed a value that is out of integer(Int32) range.
            //string str = "757657657657657";
            //int res = int.Parse(str);


            /// InvalidCastException - cast operation was not successful because the data types are incompatible.
            //object obj = new object();
            //int i = (int)obj;


            /// KeyNotFoundException is thrown when a key you are finding is not available in the Dictionary collection.
            //var dict = new Dictionary<string, string>() {
            //{"TV", "Electronics"},
            //{"Laptop", "Computers"},
            //};
            //Console.WriteLine(dict["Pen Drive"]);


            logger.LogInformation("Getting All Data");
            

            // Cached Memory 
            List<ProductDetails> list;

            // Cache Service 
            if (!_memoryCache.TryGetValue(cacheKey, out list))
            {
                list = await productService.ProductListAsync();

                _memoryCache.Set(cacheKey, list,
                    new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(1))); // For 1 Minute data will be stored in cached memory
            }
            return list;

            //return await productService.ProductListAsync();

        }


        [HttpGet("{productId:length(24)}")]
        public async Task<ActionResult<ProductDetails>> Get(string productId)
        {
            try
            {
                var productDetails = await productService.GetProductDetailByIdAsync(productId);

                //if (productDetails is null)
                //{
                //    return NotFound();
                //}
                logger.LogInformation("Getting Searched Data");
                return productDetails;
            }

            catch (Exception ex)
            {
                logger.LogError(ex, $"Error retrieving product with ID ");
                return NotFound(ex.Message);
            }
        }


        [HttpGet("{name}")]
        public async Task<IActionResult> SearchProductsByName(string name)
        {
            var products = await productService.GetProductDetailsByNameAsync(name);
            logger.LogInformation("Getting Searched Data");
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProductDetailModel model)
        {
            await productService.AddProductAsync(model);
            logger.LogInformation("Data Added");
            return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }

        [HttpPut("{productId:length(24)}")]
        public async Task<IActionResult> Update(string productId, ProductDetails productDetails)
        {
            var productDetail = await productService.GetProductDetailByIdAsync(productId);

            if (productDetail is null)
            {
                logger.LogError("Data Not Found");
                return NotFound();
            }

            productDetails.Id = productDetail.Id;

            await productService.UpdateProductAsync(productId, productDetails);
            logger.LogInformation("Updated Data");
            return Ok();
        }

        [HttpPatch("{productId}")]
        public async Task<IActionResult> UpdateProductAsync(string productId, [FromBody] JsonPatchDocument<ProductDetails> patchDocument)
        {
            var result = await productService.UpdatePartialProductAsync(productId, patchDocument);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


        [HttpDelete("{productId:length(24)}")]
        public async Task<IActionResult> Delete(string productId)
        {
            var productDetails = await productService.GetProductDetailByIdAsync(productId);

            if (productDetails is null)
            {
                return NotFound();
            }

            await productService.DeleteProductAsync(productId);
            logger.LogInformation("Data Deleted Successfully");
            return Ok();
        }


    }
}
