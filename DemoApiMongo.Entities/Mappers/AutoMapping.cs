using AutoMapper;
using DemoApiMongo.Entities.DataModels;
using DemoApiMongo.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApiMongo.Entities.Mappers
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<ProductDetailModel, ProductDetails>()
                 .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
                 .ReverseMap().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProductName));
        }

    }
}
