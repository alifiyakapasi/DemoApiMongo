using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApiMongo.Entities.ViewModels
{
    public class ProductDetailModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("ProductName")]
        public string Name { get; set; }
        public string? ProductDescription { get; set; }
        public int ProductPrice { get; set; }
        public int ProductQuantity { get; set; }
    }
}
