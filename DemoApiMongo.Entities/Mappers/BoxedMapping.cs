using Boxed.Mapping;
using DemoApiMongo.Entities.DataModels;
using DemoApiMongo.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApiMongo.Entities.Mappers
{
    public class BoxedMapping : IMapper<ProductDetailModel, ProductDetails>
    {
        public void Map(ProductDetailModel source, ProductDetails destination)
        {
            destination.Id = source.Id;
            destination.ProductName = source.Name;
            destination.ProductDescription = source.ProductDescription;
            destination.ProductPrice = source.ProductPrice;
            destination.ProductQuantity = source.ProductQuantity;
        }
    }
}
