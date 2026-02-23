using AutoMapper;
using GeekShoping.ProductAPI.Data.ValueObjects;
using GeekShoping.ProductAPI.Model;


namespace GeekShoping.ProductAPI.Config
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(c =>
            {
                c.CreateMap<ProductVO, Product>();
                c.CreateMap<Product, ProductVO>();
            }, 
            new LoggerFactory() );
            return mappingConfig;
        }
    }
}
